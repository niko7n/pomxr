using System;
using UnityEngine;

public class STCStepSelector : MonoBehaviour
{
    #region Variables
    private float _yScale;
    public int _step, _from, _to, _previousStep, _minRange = 20, _maxRange = 100;
    public int _minFrom, _maxTo; // used for the constraint calculation
    public bool _isBottomSelector;
    #endregion

    #region Event
    public event Action<int> onStepChange;
    public void StepChanged()
    {
        onStepChange?.Invoke(_step);
    }
    #endregion

    void Update()
    {
        UpdateStep();
        UpdatePosition();

        if (_step != _previousStep)
        {
            ConstrainStep();
            _previousStep = _step;
            StepChanged();
        }

    }
    #region Public Methods
    public void Initialize(float xScale, float yScale, int from, int to, int currentStep, Material planeMaterial)
    {
        _yScale = yScale;
        _from = from;
        _to = to;
        _step = currentStep;

        _minFrom = _from;
        _maxTo = _to;

        UpdatePosition();
        _previousStep = _step;

        GeneratePlaneMesh(xScale, planeMaterial);
        StepChanged();
    }
    public void InitializeAsHeatmap(float xScale, float yScale, int from, int to, int currentStep, int[,] heatmapMatrix)
    {
        _yScale = yScale;
        _from = from;
        _to = to;
        _step = currentStep;

        _minFrom = _from;
        _maxTo = _to;

        UpdatePosition();
        _previousStep = _step;

        GenerateGridMesh(xScale, heatmapMatrix);
        StepChanged();
    }
    public int GetCurrentStep()
    {
        return _step;
    }
    public void SetOtherSelector(STCStepSelector other)
    {
        if (other.GetCurrentStep() > _step) _isBottomSelector = true;
        other.onStepChange += UpdateOtherStep;
        UpdateOtherStep(other.GetCurrentStep());
    }
    public void SetCurrentStep(int step)
    {
        _step = step;
        ConstrainStep();
        UpdatePosition();
    }
    #endregion

    #region Private Methods
    private void UpdateOtherStep(int otherStep)
    {
        if (_isBottomSelector)
        {
            _maxTo = otherStep - _minRange;
            _minFrom = otherStep - _maxRange;
        }
        else
        {
            _minFrom = otherStep + _minRange;
            _maxTo = otherStep + _maxRange;
        }
    }
    private void UpdateStep()
    {
        _step = CalculateCurrentStep();
    }
    private void UpdatePosition()
    {
        transform.localPosition = CalculateCurrentPosition();
    }
    private Vector3 CalculateCurrentPosition()
    {
        return new Vector3(0, (_step * _yScale) - (_from * _yScale), 0);
    }
    private int CalculateCurrentStep()
    {
        float yPosition = transform.localPosition.y + (_from * _yScale);
        return Mathf.RoundToInt(yPosition / _yScale);
    }
    private void ConstrainStep()
    {
        // Constrain towards the boundaries of the stc
        if (_step < _from) _step = _from;
        if (_step > _to - 1) _step = _to - 1;

        // Constrain towards the other selector
        if (_step < _minFrom) _step = _minFrom;
        if (_step > _maxTo - 1) _step = _maxTo - 1;
        UpdatePosition();
    }
    private void GeneratePlaneMesh(float xScale, Material material)
    {
        float zero = 0 - (xScale / 2);
        float width = (xScale * 11) - (xScale / 2);
        float height = 0.01f; // Set the height of the cuboid on the y-axis

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];

        vertices[0] = new Vector3(zero, (height / 2) - 0.005f, zero);
        vertices[1] = new Vector3(width, (height / 2) - 0.005f, zero);
        vertices[2] = new Vector3(width, (height / 2) - 0.005f, width);
        vertices[3] = new Vector3(zero, (height / 2) - 0.005f, width);

        vertices[4] = new Vector3(zero, (-height / 2) - 0.005f, zero);
        vertices[5] = new Vector3(width, (-height / 2) - 0.005f, zero);
        vertices[6] = new Vector3(width, (-height / 2) - 0.005f, width);
        vertices[7] = new Vector3(zero, (-height / 2) - 0.005f, width);

        int[] triangles = new int[36] {
            2, 1, 0, 2, 0, 3, // Top face
            4, 5, 6, 4, 6, 7, // Bottom face
            6, 5, 1, 6, 1, 2, // Left side
            0, 4, 7, 0, 7, 3, // Right side
            4, 0, 1, 4, 1, 5, // Front face
            6, 2, 3, 6, 3, 7  // Back face
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
    }
    private Color[,] CalculateCellColors(int[,] heatmapMatrix)
    {
        // Find the maximum value in heatmapMatrix
        int max = int.MinValue;
        foreach (int value in heatmapMatrix)
        {
            if (value > max) max = value;
        }

        // Initialize array for cell colors
        int size = heatmapMatrix.GetLength(0);
        Color[,] cellColors = new Color[size, size];

        // Calculate colors
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int value = heatmapMatrix[i, j];

                // Calculate color based on gradient from green to red
                float ratio = Mathf.Pow((float)value / max, 0.5f);
                Color color = Color.Lerp(new Color(.8f, .8f, .8f), new Color(.4f, 0, .6f), ratio);
                cellColors[i, j] = color;
            }
        }

        return cellColors;
    }
    private void GenerateGridMesh(float xScale, int[,] heatmapMatrix)
    {
        int boardSize = 11;
        float halfXScale = xScale / 2;

        int numCells = boardSize * boardSize;
        int numVertices = numCells * 4;
        int numTriangles = numCells * 6;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numTriangles];
        Color[] colors = new Color[numVertices];

        Color[,] cellColors = CalculateCellColors(heatmapMatrix);

        // Generate grid vertices and colors
        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                // Four vertices per cell
                float xPos = (x * xScale) - halfXScale;
                float zPos = (y * xScale) - halfXScale;

                vertices[vertexIndex] = new Vector3(xPos, 0, zPos);
                vertices[vertexIndex + 1] = new Vector3(xPos + xScale, 0, zPos);
                vertices[vertexIndex + 2] = new Vector3(xPos, 0, zPos + xScale);
                vertices[vertexIndex + 3] = new Vector3(xPos + xScale, 0, zPos + xScale);

                // Assign the same color to all four vertices of the cell
                colors[vertexIndex] = cellColors[x, y];
                colors[vertexIndex + 1] = cellColors[x, y];
                colors[vertexIndex + 2] = cellColors[x, y];
                colors[vertexIndex + 3] = cellColors[x, y];

                // Two triangles per cell
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 3;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex;
                triangles[triangleIndex + 4] = vertexIndex + 2;
                triangles[triangleIndex + 5] = vertexIndex + 3;

                vertexIndex += 4;
                triangleIndex += 6;
            }
        }

        // Create the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/VertexColorMaterial");
    }
    #endregion
}

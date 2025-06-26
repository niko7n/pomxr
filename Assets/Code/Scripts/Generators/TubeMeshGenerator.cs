using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TubeMeshGenerator : MonoBehaviour
{
    #region Variables
    public int TubeVertexCount;
    public float Radius;
    [SerializeField] private Vector3 TestRotation = new Vector3(-90, 0, 0);

    private Mesh _mesh;
    private Vector3[] _pathVertices;
    private Vector3[] _meshVertices;
    private int[] _meshTriangles;
    #endregion
    void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }
    #region Public Methods
    /// <summary>
    /// It calculates the mesh vertex and triangle points and calls UpdateMesh.
    /// </summary>
    public void Render()
    {
        _meshVertices = CalculateMeshPointsForPathPoints();
        _meshTriangles = GenerateMeshTriangles();

        UpdateMesh();
    }

    /// <summary>
    /// Updates the mesh of the tube.
    /// </summary>
    public void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _meshVertices;
        _mesh.triangles = _meshTriangles;

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    /// <summary>
    /// Sets the center points of the tube.
    /// </summary>
    /// <param name="points"></param>
    public void SetPlayerPathPoints(Vector3[] points, float yScale, float xScale)
    {
        _pathVertices = new Vector3[points.Length];
        float scaleBase = -yScale;
        for (int i = 0; i < points.Length; i++)
        {
            // Adjust the Y coordinate to increase with the scale.
            _pathVertices[i] = new Vector3(points[i].x * xScale, scaleBase + yScale, points[i].z * xScale);
            scaleBase = _pathVertices[i].y;
        }
    }

    /// <summary>
    /// Sets the material for the mesh renderer.
    /// </summary>
    /// <param name="mat"></param>
    public void SetMaterial(Material mat)
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = mat;
    }
    #endregion

    #region Private Methods
    // Generate points on a circle around the given Vector3 point
    private Vector3[] GeneratePointsOnCircle(float radius, int TubeVertexCount, Vector3 center, Vector3 rotation)
    {
        radius /= 10;

        Vector3[] vertices = new Vector3[TubeVertexCount];

        Quaternion qRotation = Quaternion.Euler(rotation);

        for (int i = 0; i < TubeVertexCount; i++)
        {
            // Calculate the point position
            double theta = 2 * Math.PI * ((float)i / TubeVertexCount);
            float x = radius * Convert.ToSingle(Math.Cos(theta));
            float y = radius * Convert.ToSingle(Math.Sin(theta));

            // Add point to vertex array
            Vector3 point = new Vector3(x, y, 0);
            point = qRotation * point;
            point += center;
            vertices[i] = point;
        }
        return vertices;
    }

    // Calculate the Mesh Points for every Path Point
    private Vector3[] CalculateMeshPointsForPathPoints()
    {
        List<Vector3> meshVerticesList = new List<Vector3>();

        for (int i = 0; i < _pathVertices.Length; i++)
        {
            Vector3[] pointsOnCircle = GeneratePointsOnCircle(Radius, TubeVertexCount, _pathVertices[i], TestRotation);
            meshVerticesList.AddRange(pointsOnCircle);
        }

        return meshVerticesList.ToArray();
    }

    // Generating the Triangles
    private int[] GenerateMeshTriangles()
    {
        List<int> triangleIndices = new List<int>();
        int n = TubeVertexCount;

        // Calculate the tube sides
        for (int circle = 0; circle < _pathVertices.Length - 1; circle++)
        {
            for (int i = 0; i < n; i++)
            {
                // Calculate indices of first circle
                int firstOfFirstCircle = circle * n;
                int firstIndex = firstOfFirstCircle + i;
                int secondIndex = firstOfFirstCircle + (i + 1) % n;

                // Calculate indices of second circle
                int firstOfSecondCircle = ((circle + 1) % _pathVertices.Length) * n;
                int thirdIndex = firstOfSecondCircle + i;
                int fourthIndex = firstOfSecondCircle + (i + 1) % n;

                // Triangle 1
                triangleIndices.Add(firstIndex);
                triangleIndices.Add(fourthIndex);
                triangleIndices.Add(thirdIndex);

                // Triangle 2
                triangleIndices.Add(firstIndex);
                triangleIndices.Add(secondIndex);
                triangleIndices.Add(fourthIndex);
            }
        }

        // Calculate the top
        int firstOfLastCirlce = (_pathVertices.Length - 1) * n;
        int triangleAmount = TubeVertexCount - 2;
        for (int i = 0; i < triangleAmount; i++)
        {
            int firstIndex = firstOfLastCirlce;
            int secondIndex = firstOfLastCirlce + i + 1;
            int thirdIndex = firstOfLastCirlce + i + 2;

            triangleIndices.Add(firstIndex);
            triangleIndices.Add(secondIndex);
            triangleIndices.Add(thirdIndex);
        }
        return triangleIndices.ToArray();
    }
    #endregion


    // ===== Helper =====
    /*
    private void OnDrawGizmos()
    {
        if (_meshVertices != null)
        {
            for (int i = 0; i < _meshVertices.Length; i++)
            {
                Gizmos.DrawSphere(_meshVertices[i], .1f);
            }
        }

        if (_pathVertices != null)
        {
            Color originalColor = Gizmos.color;
            Gizmos.color = Color.green;
            for (int i = 0; i < _pathVertices.Length; i++)
            {
                Gizmos.DrawSphere(_pathVertices[i], .1f);
            }
            Gizmos.color = originalColor;
        }
    }
    */
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class STCItemSpawner : MonoBehaviour
{
    #region Variables
    private GameObject _item, _holderObject;
    private STCManager _parentSTC;
    private Dictionary<int, List<Vector3>> _itemDictionary;
    private int _player, _from, _to;
    #endregion

    #region Public Functions
    public void InitializeBomb(int player, int from, int to, STCManager parentSTC)
    {
        _player = player;
        _from = from;
        _to = to;
        _parentSTC = parentSTC;
        _item = _parentSTC.Properties.BombModel;
        _itemDictionary = PomDataHandler.GetAllPlayerBombsFromTo(_player, _from, _to, _parentSTC.PomData);

        // Create the object that holds the combined meshes and the line Render
        if (_holderObject != null) Destroy(_holderObject);
        _holderObject = new GameObject($"{_item.name} holder for Player {_player}");
        _holderObject.transform.parent = _parentSTC.transform;
        _holderObject.transform.localPosition = Vector3.zero;

        DrawLines();
        DrawCombinedItems(PrepareBombCombine(), _parentSTC.Properties.BombMaterials[_player]);
        ResetRotation();
    }

    public void InitializeFlames(int from, int to, STCManager parentSTC)
    {
        _from = from;
        _to = to;
        _parentSTC = parentSTC;
        _item = _parentSTC.Properties.FlameModel;
        _itemDictionary = PomDataHandler.GetAllFlamesPosFromTo(_from, _to, _parentSTC.PomData);

        // Create the object that holds the combined meshes and the line Render
        if (_holderObject != null) Destroy(_holderObject);
        _holderObject = new GameObject($"{_item.name} holder for Player {_player}");
        _holderObject.transform.parent = _parentSTC.transform;
        _holderObject.transform.localPosition = Vector3.zero;

        DrawCombinedItems(PrepareFlameCombine(), _parentSTC.Properties.FlameMaterial);
        ResetRotation();
    }
    public void DestroyObjectHolder()
    {
        Destroy(_holderObject);
    }
    #endregion

    #region Private Functions
    // Draws lines between items for a given list
    private void DrawLines()
    {
        foreach (List<Vector3> list in _itemDictionary.Values)
        {
            if (list.Count <= 1) continue;

            GameObject newLineHolder = new GameObject("Item Connection Lines");
            newLineHolder.transform.parent = _holderObject.transform;
            newLineHolder.transform.localPosition = Vector3.zero;

            LineRenderer lineRenderer = newLineHolder.AddComponent<LineRenderer>();
            lineRenderer.positionCount = list.Count;
            lineRenderer.startWidth = _parentSTC.GetXScale() * .1f;
            lineRenderer.useWorldSpace = false;
            lineRenderer.material = Resources.Load<Material>("Materials/BombLine");

            int index = 0;
            foreach (Vector3 position in list)
            {
                lineRenderer.SetPosition(index, CalculatePosition(position));
                index++;
            }
        }
    }

    // Draws a given CombineInstace[] into the STC
    private void DrawCombinedItems(CombineInstance[] combine, Material mat)
    {
        // Combine meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Use UInt32 index format
        combinedMesh.CombineMeshes(combine, true, true);

        // Add to _holderObject
        _holderObject.AddComponent<MeshRenderer>().sharedMaterial = mat;
        MeshFilter combinedMeshFilter = _holderObject.AddComponent<MeshFilter>();

        combinedMeshFilter.mesh = combinedMesh;
        _holderObject.transform.localEulerAngles = Vector3.zero;
    }

    // Creates the CombineInstance[] for the bombs for each player
    private CombineInstance[] PrepareBombCombine()
    {
        // Calculate the total number of positions
        int totalPositions = _itemDictionary.Values.Sum(list => list.Count);

        float xScale = _parentSTC.GetXScale() * .35f;
        float yScale = Mathf.Min(_parentSTC.GetYScale() * .75f, .3f);

        // Prepare for the Mesh Combination
        CombineInstance[] combine = new CombineInstance[totalPositions];
        int index = 0;

        // Iterate over each item group
        foreach (List<Vector3> list in _itemDictionary.Values)
        {
            // Iterate over each item of the group
            foreach (Vector3 position in list)
            {
                GameObject newItem = Instantiate(_item, CalculatePosition(position), Quaternion.Euler(90, 0, 0));

                newItem.transform.localScale = new Vector3(xScale, xScale, yScale);
                MeshFilter meshFilter = newItem.GetComponent<MeshFilter>();

                combine[index].mesh = meshFilter.sharedMesh;
                combine[index].transform = meshFilter.transform.localToWorldMatrix;
                Destroy(newItem);
                index++;
            }
        }

        return combine;
    }

    private CombineInstance[] PrepareFlameCombine()
    {
        // Calculate the total number of positions
        int totalPositions = _itemDictionary.Values.Sum(list => list.Count);

        // Prepare for the Mesh Combination
        CombineInstance[] combine = new CombineInstance[totalPositions];
        int index = 0;

        float baseXScale = _parentSTC.GetXScale();
        float yScale = Mathf.Min(_parentSTC.GetYScale(), .3f);

        // Store precomputed xScale values for different life values
        Dictionary<int, float> xScaleDictionary = new Dictionary<int, float>
        {
            { 2, baseXScale * 0.9f },
            { 1, baseXScale * 0.6f },
            { 0, baseXScale * 0.3f }
        };

        foreach (int life in _itemDictionary.Keys)
        {
            List<Vector3> positions = _itemDictionary[life];

            // Set the xScale value based on the life left
            float xScale = xScaleDictionary.ContainsKey(life) ? xScaleDictionary[life] : baseXScale;

            foreach (Vector3 position in positions)
            {
                GameObject newItem = Instantiate(_item, CalculatePosition(position), Quaternion.identity);

                newItem.transform.localScale = new Vector3(xScale, yScale, xScale);
                MeshFilter meshFilter = newItem.GetComponent<MeshFilter>();

                combine[index].mesh = meshFilter.sharedMesh;
                combine[index].transform = meshFilter.transform.localToWorldMatrix;
                Destroy(newItem);
                index++;
            }
        }

        return combine;
    }

    private void ResetRotation()
    {
        _holderObject.transform.localEulerAngles = Vector3.zero;
    }

    private Vector3 CalculatePosition(Vector3 sourcePosition)
    {
        return new Vector3(sourcePosition.x * _parentSTC.GetXScale(), sourcePosition.y * _parentSTC.GetYScale() - (_from * _parentSTC.GetYScale()), sourcePosition.z * _parentSTC.GetXScale());
    }
    #endregion
}

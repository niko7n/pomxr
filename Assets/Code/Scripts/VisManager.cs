using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private STCProperties _stcProperties;
    [SerializeField] private STCProperties _stcSelectionProperties;
    [SerializeField] private List<TextAsset> _pommermanGameFileList;
    [Header("Space Time Cube Settings")]
    [SerializeField] private GameObject _stcBasePrefab;
    [SerializeField] private List<GameObject> _spaceTimeCubeBaseList;
    private PommermanData _pomData;
    private Vector3[] _spawnPoints;
    #endregion

    #region Initialize
    void Start()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;
        Vector3 centerPosition = mainCamera.transform.position;

        int totalCubes = _pommermanGameFileList.Count;
        _spawnPoints = GeneratePointsOnCircle(centerPosition, 6, totalCubes, 25, 155);

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            StartCoroutine(LoadGameData(_pommermanGameFileList[i]));
            _spaceTimeCubeBaseList.Add(CreateSpaceTimeCubeBase(_spawnPoints[i], _pomData));
        }
    }
    #endregion

    #region Private Methods
    private IEnumerator LoadGameData(TextAsset textAsset)
    {
        _pomData = JsonManager.LoadJsonFromTextAsset<PommermanData>(textAsset);
        yield return null;
    }
    private GameObject CreateSpaceTimeCubeBase(Vector3 spawnPosition, PommermanData data)
    {
        // Create STC Base
        GameObject stcBase = Instantiate(_stcBasePrefab, spawnPosition, Quaternion.identity);
        stcBase.transform.position = new Vector3(stcBase.transform.position.x, 0f, stcBase.transform.position.z);
        stcBase.transform.rotation = GetRotationTowardsCamera(stcBase.transform, Camera.main.transform);
        stcBase.transform.parent = transform;

        stcBase.GetComponent<STCBase>().Initialize(data, _stcProperties, _stcSelectionProperties);
        return stcBase;
    }
    // Calculate the possible Spawn Points
    private Vector3[] GeneratePointsOnCircle(Vector3 center, float radius, int numberOfPoints, float startAngle, float endAngle)
    {
        Vector3[] points = new Vector3[numberOfPoints];

        if (numberOfPoints == 1)
        {
            points[0] = new Vector3(0, 0, radius);
            return points;
        }

        float angleStep = (endAngle - startAngle) / (numberOfPoints - 1);

        for (int i = 0; i < numberOfPoints; i++)
        {
            float angleInDegrees = startAngle + i * angleStep;
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            float x = center.x + radius * Mathf.Cos(angleInRadians);
            float y = center.y;
            float z = center.z + radius * Mathf.Sin(angleInRadians);

            points[i] = new Vector3(x, y, z);
        }

        return points;
    }
    private Quaternion GetRotationTowardsCamera(Transform objectTransform, Transform cameraTransform)
    {
        Vector3 directionToCamera = cameraTransform.position - objectTransform.position;
        directionToCamera.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
        objectTransform.rotation = targetRotation;
        return targetRotation;
    }
    #endregion
}

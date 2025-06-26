using System.Collections.Generic;
using UnityEngine;

public class STCIconSpawner : MonoBehaviour
{
    #region Variables
    private STCManager _parentSTC;
    private List<GameObject> _markers;

    private int _from, _to;
    #endregion
    public void Initialize(int from, int to, STCManager parentSTC)
    {
        _markers ??= new List<GameObject>();
        CleanupMarkers();

        _from = from;
        _to = to;
        _parentSTC = parentSTC;

        CreateIconsForPowerUps(PomDataHandler.GetPickedUpItems(_from, _to, parentSTC.PomData));
    }

    public void CleanupMarkers()
    {
        if (_markers != null)
        {
            foreach (var marker in _markers)
                Destroy(marker);

            _markers.Clear();
        }
    }
    #region Private Methods
    private void CreateIconsForPowerUps(Dictionary<Vector3, int> powerUpDictionary)
    {
        Material mat = Resources.Load<Material>("Materials/UnlitWhite");
        Sprite bombSprite = Resources.Load<Sprite>("Sprites/bomb-icon");
        Sprite rangeSprite = Resources.Load<Sprite>("Sprites/range-icon");
        Sprite kickSprite = Resources.Load<Sprite>("Sprites/kick-icon");

        foreach (var powerUp in powerUpDictionary)
        {
            Vector3 goalPosition = new Vector3(
                   powerUp.Key.x * _parentSTC.GetXScale(),
                   (powerUp.Key.y * _parentSTC.GetYScale()) - (_from * _parentSTC.GetYScale()),
                   powerUp.Key.z * _parentSTC.GetXScale()
                );

            if (goalPosition.y >= 0)
                switch (powerUp.Value)
                {
                    case 6:
                        _markers.Add(CreateIndicator(goalPosition, bombSprite, mat));
                        break;
                    case 7:
                        _markers.Add(CreateIndicator(goalPosition, rangeSprite, mat));
                        break;
                    case 8:
                        _markers.Add(CreateIndicator(goalPosition, kickSprite, mat));
                        break;
                    default:
                        break;
                }
        }
    }

    private GameObject CreateIndicator(Vector3 goalPosition, Sprite sprite, Material material)
    {
        GameObject marker = new GameObject("Icon Indicator");
        marker.transform.parent = _parentSTC.transform;
        marker.transform.localPosition = Vector3.zero;

        STCEventMarker stcEventMarker = marker.AddComponent<STCEventMarker>();
        stcEventMarker.Initialize(_parentSTC.GetXScale(), goalPosition, sprite, material);

        return marker;
    }
    #endregion
}

using System.Collections.Generic;
using UnityEngine;

public class STCEvents : MonoBehaviour
{
    private STCManager _parentSTC;
    private int _from;
    private List<GameObject> _markers;
    private PommermanData _data;

    public void Initialize(int from, int to, STCManager parentSTC)
    {
        _markers ??= new List<GameObject>();
        CleanupMarkers();

        _parentSTC = parentSTC;
        _from = from;
        _data = _parentSTC.PomData;

        HashSet<int> deadPlayers = new HashSet<int>();

        for (int i = from; i <= to; i++)
        {
            HandleEventsForStep(i, deadPlayers);
        }
    }

    private void HandleEventsForStep(int step, HashSet<int> deadPlayers)
    {
        for (int playerID = 0; playerID < 4; playerID++)
        {
            if (deadPlayers.Contains(playerID)) continue;
            HandleDeathEvent(step, playerID, deadPlayers);
        }
    }

    private void HandleDeathEvent(int step, int playerID, HashSet<int> deadPlayers)
    {
        if (PomDataHandler.IsPlayerDead(playerID, step, _data))
        {
            if (deadPlayers.Add(playerID))
            {
                Vector3 dyingPosition = PomDataHandler.GetPlayerPosAt(playerID, step - 1, _data);
                Vector3 goalPosition = new Vector3(
                    dyingPosition.x * _parentSTC.GetXScale(),
                    ((step - 1) * _parentSTC.GetYScale()) - (_from * _parentSTC.GetYScale()),
                    dyingPosition.z * _parentSTC.GetXScale()
                );

                if (goalPosition.y >= 0)
                    _markers.Add(CreateIndicator(goalPosition, _parentSTC.Properties.AgentMaterials[playerID]));

            }
        }
    }

    private void CleanupMarkers()
    {
        if (_markers != null)
        {
            foreach (var marker in _markers)
                Destroy(marker);

            _markers.Clear();
        }
    }

    private GameObject CreateIndicator(Vector3 goalPosition, Material material)
    {
        GameObject marker = new GameObject("Indicator");
        marker.transform.parent = _parentSTC.transform;
        marker.transform.localPosition = Vector3.zero;
        STCEventMarker stcEventMarker = marker.AddComponent<STCEventMarker>();
        stcEventMarker.Initialize(goalPosition, 20, _parentSTC.Properties.Width, material);

        return marker;
    }
}

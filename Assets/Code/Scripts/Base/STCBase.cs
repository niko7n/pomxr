using UnityEngine;

public class STCBase : MonoBehaviour
{
    #region Variables
    private PommermanData _pomData;
    [Header("Base Settings")]
    [SerializeField] private Transform _stcSpawnPoint;
    [SerializeField] private Transform _stcPlatformCentrePoint;
    [SerializeField] private GameObject _infoScreen;

    private GameObject _stc, _stcSelection;
    private STCManager _stcManager, _stcSelectionManager;
    private STCRangeSelector _stcRangeSelector;
    private STCProperties _stcProperties, _stcSelectionProperties;
    private int _id;
    private bool _isAgentHighlighted, _showBombs, _showPickUps, _showInfoScreen;
    #endregion

    #region Public Methods
    public void Initialize(PommermanData data, STCProperties properties, STCProperties selectionProperties)
    {
        _pomData = data;
        _stcProperties = properties;
        _stcSelectionProperties = selectionProperties;
        _id = PomDataHandler.GetGameId(_pomData);
        _isAgentHighlighted = false;
        _showBombs = false;
        _showInfoScreen = false;

        // Set Agent Materials
        ResetAgentMaterials(ref _stcProperties);

        // Create STC
        _stc = new GameObject($"Space Time Cube {_id}");
        _stcManager = _stc.AddComponent<STCManager>();
        _stc.transform.parent = transform;
        _stc.transform.position = _stcSpawnPoint.position;
        _stcManager.ID = _id;
        _stcManager.Initialize(_pomData, _stcProperties, false);
        _stcManager.BuildSTC(0, _pomData.state.Length - 1, true, _showBombs, _showPickUps);
    }

    public void ToggleSelection()
    {
        if (_stcSelection == null)
        {
            // Create a Range Selector and make the board from the main STC unmoveable
            _stcRangeSelector = _stcManager.CreateRangeSelector();
            _stcRangeSelector.onRangeChanged += UpdateSelection;
            _stcManager.BuildSTC(0, _pomData.state.Length - 1, false, _showBombs, _showPickUps);

            int[] selection = _stcRangeSelector.GetRange(); // needed for initial creation
            CreateSelection();
            UpdateSelection(selection[0], selection[1]);
        }
        else
        {
            // Destroy the Selection STC and make the Board moveable again
            _stcRangeSelector.onRangeChanged -= UpdateSelection;
            _stcRangeSelector.DestroySelectors();
            Destroy(_stcSelection);
            Destroy(_stcRangeSelector);

            _stcManager.BuildSTC(0, _pomData.state.Length - 1, true, _showBombs, _showPickUps);
        }
    }
    public void ToggleHighlightAgent(int agent)
    {
        Material highlightMat = Resources.Load<Material>("Materials/CustomAgentHighlight");
        bool isAgentCurrentlyHighlighted = _isAgentHighlighted && _stcProperties.AgentMaterials[agent] == highlightMat;

        // Reset Materials
        ResetAgentMaterials(ref _stcProperties);
        ResetAgentMaterials(ref _stcSelectionProperties);

        // Set Material
        _isAgentHighlighted = !isAgentCurrentlyHighlighted;
        if (_isAgentHighlighted)
        {
            _stcProperties.AgentMaterials[agent] = highlightMat;
            _stcSelectionProperties.AgentMaterials[agent] = highlightMat;
        }

        // Update STCs
        if (_stcSelection == null)
        {
            _stcManager.BuildSTC(0, _pomData.state.Length - 1, true, _showBombs, _showPickUps);
        }
        else
        {
            _stcManager.BuildSTC(0, _pomData.state.Length - 1, false, _showBombs, _showPickUps);
            int[] selection = _stcRangeSelector.GetRange();
            UpdateSelection(selection[0], selection[1]);
        }
    }
    public void ToggleBombs()
    {
        _showBombs = !_showBombs;

        if (_stcSelection == null) _stcManager.BuildSTC(0, _pomData.state.Length - 1, true, _showBombs, _showPickUps);
        else _stcManager.BuildSTC(0, _pomData.state.Length - 1, false, _showBombs, _showPickUps);
    }
    public void TogglePickUps()
    {
        _showPickUps = !_showPickUps;

        if (_stcSelection == null) _stcManager.BuildSTC(0, _pomData.state.Length - 1, true, _showBombs, _showPickUps);
        else _stcManager.BuildSTC(0, _pomData.state.Length - 1, false, _showBombs, _showPickUps);
    }
    public void ToggleInfoScreen()
    {
        _showInfoScreen = !_showInfoScreen;

        if (_showInfoScreen)
        {
            _infoScreen = Instantiate(Resources.Load<GameObject>("Prefabs/InfoScreen"), transform);
            _infoScreen.transform.localPosition = new Vector3(3.25f, 2, -1);
            _infoScreen.transform.localRotation = Quaternion.Euler(25, 0, 0);
            _infoScreen.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            _infoScreen.GetComponent<InfoScreenManager>().Initialize(this);
        }
        else
        {
            Destroy(_infoScreen);
        }
    }
    #endregion
    #region Getter
    public STCProperties GetSTCProperties()
    {
        return _stcProperties;
    }
    public PommermanData GetPomData()
    {
        return _pomData;
    }
    #endregion
    #region Private Methods
    private void CreateSelection()
    {
        _stcSelection = new GameObject("Space Time Cube Selection");
        _stcSelection.transform.parent = transform;
        _stcSelection.transform.localPosition = new Vector3(_stcSelectionProperties.Width * 1.1f, _stcProperties.Height * 0.4f, -(_stcSelectionProperties.Width * 0.3f));

        _stcSelectionManager = _stcSelection.AddComponent<STCManager>();
        _stcSelectionManager.ID = 90;
        _stcSelectionManager.Initialize(_pomData, _stcSelectionProperties, true);

        // Make the board of the main STC non moveable
        _stcManager.BuildSTC(0, _pomData.state.Length - 1, false, _showBombs, _showPickUps);
    }
    private void UpdateSelection(int from, int to)
    {
        if (to < from) return;
        if (to == from) return;

        _stcSelectionManager.BuildSTC(from, to, true, true, true); // Currently bombs and pickups are hard-coded into the selection
    }
    private void ResetAgentMaterials(ref STCProperties properties)
    {
        for (int i = 0; i < properties.AgentMaterials.Length; i++)
        {
            Material a1 = Resources.Load<Material>("Materials/CustomAgent-One");
            Material a2 = Resources.Load<Material>("Materials/CustomAgent-Two");

            if (i % 2 == 0) properties.AgentMaterials[i] = a1;
            else properties.AgentMaterials[i] = a2;
        }
    }
    #endregion

}

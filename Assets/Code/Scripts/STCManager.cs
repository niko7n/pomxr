using UnityEngine;

public class STCManager : MonoBehaviour
{
    #region Variables
    private PommermanData _pomData;
    private STCProperties _properties;
    private GameObject _board, _agent1, _agent2, _agent3, _agent4;
    private STCItemSpawner[] _bombSpawners = new STCItemSpawner[4];
    private STCItemSpawner _flameSpawner;
    private STCIconSpawner _pickUpSpawner;
    private STCEvents _events;
    private int _id, _from, _to;
    private bool _drawBombs, _drawPickUps, _isBoardMoveable, _isSelection;
    private Camera _timeFlattingCamera;
    #endregion

    #region Getter and Setter
    public STCProperties Properties
    {
        get => _properties;
        set => _properties = value;
    }
    public int ID
    {
        get => _id;
        set => _id = value;
    }
    public PommermanData PomData
    {
        get => _pomData;
    }
    #endregion

    #region Public Methods
    public void Initialize(PommermanData data, STCProperties properties, bool isSelection)
    {
        _pomData = data;
        _properties = properties;
        _isSelection = isSelection;

        // Prepare for bomb and events creation
        for (int i = 0; i < _bombSpawners.Length; i++)
        {
            _bombSpawners[i] = gameObject.AddComponent<STCItemSpawner>();
        }
        _flameSpawner = gameObject.AddComponent<STCItemSpawner>();
        _pickUpSpawner = gameObject.AddComponent<STCIconSpawner>();

        _events = gameObject.AddComponent<STCEvents>();

        // Add Top Down Camera
        _timeFlattingCamera = Instantiate(Resources.Load<Camera>("Prefabs/TopDownCamera"), transform);
        if (isSelection)
            _timeFlattingCamera.transform.localPosition = new Vector3(1, properties.Height + 1, 1);
        else
            _timeFlattingCamera.transform.localPosition = new Vector3(.5f, properties.Height + 1, .5f);
    }
    public void BuildSTC(int from, int to, bool isBoardMoveable, bool drawBombs, bool drawPickUps)
    {
        _from = from;
        _to = to;
        _isBoardMoveable = isBoardMoveable;
        _drawBombs = drawBombs;
        _drawPickUps = drawPickUps;

        ClearCurrentSTC();
        _board = CreateBoardGameObject(from, to, 0, _isBoardMoveable);

        // Create Agents
        _agent1 = CreateAgentGameObject(from, to, 0);
        _agent2 = CreateAgentGameObject(from, to, 1);
        _agent3 = CreateAgentGameObject(from, to, 2);
        _agent4 = CreateAgentGameObject(from, to, 3);
        // Create Bombs if needed
        if (_drawBombs)
        {
            // Create Flames and Bombs
            for (int i = 0; i < 4; i++)
            {
                _bombSpawners[i].InitializeBomb(i, from, to, this);
            }
            _flameSpawner.InitializeFlames(from, to, this);
        }

        if (_drawPickUps) _pickUpSpawner.Initialize(from, to, this);
        else _pickUpSpawner.CleanupMarkers();

        // Events
        _events.Initialize(from, to, this);

        // Reset Rotation
        transform.localEulerAngles = Vector3.zero;

        // Top Down Camera
        if (_isSelection)
            _timeFlattingCamera.GetComponent<TopDownCameraLogic>().Initialize(1.1f, 2.5f);
        else
            _timeFlattingCamera.GetComponent<TopDownCameraLogic>().Initialize(.55f, 1.5f);
    }

    public STCRangeSelector CreateRangeSelector()
    {
        int fromStep = (int)Mathf.Round((_pomData.state.Length - 1) * 0.45f);
        int toStep = fromStep + 50; // HardCoded Range
        if (toStep > _pomData.state.Length - 1) toStep = _pomData.state.Length - 1;

        STCRangeSelector rangeSelectorScript = gameObject.AddComponent<STCRangeSelector>();
        rangeSelectorScript.Initialize(fromStep, toStep, this);

        return rangeSelectorScript;
    }

    public float GetYScale()
    {
        if (_isSelection)
            return Properties.Height / (_to - _from);
        else
            return 7.5f / 750;
    }

    public float GetXScale()
    {
        return Properties.Width / 10;
    }
    #endregion

    #region Private Methods
    private GameObject CreateBoardGameObject(int from, int to, int step, bool isMoveable)
    {
        GameObject board = new GameObject("Board");
        board.transform.parent = transform;
        board.transform.position = transform.position;
        BoardGenerator boardGenerator = board.AddComponent<BoardGenerator>();
        boardGenerator.StartBoardDrawing(from, to, this);

        if (isMoveable) boardGenerator.CreateSelector(from, to, step);
        else boardGenerator.BuildBoard(0);

        board.transform.localEulerAngles = Vector3.zero;

        return board;
    }
    private GameObject CreateAgentGameObject(int from, int to, int playerID)
    {
        if (playerID < 0 || playerID > 3)
        {
            Debug.LogError("playerID is out of bounce. Set to 0");
            playerID = 0;
        }

        if (PomDataHandler.IsPlayerDead(playerID, from, _pomData)) return null;

        GameObject agent = new GameObject("Agent " + playerID);
        agent.transform.parent = transform;
        agent.transform.position = transform.position;

        int layer = LayerMask.NameToLayer("Agents");
        agent.layer = layer;

        TubeMeshGenerator tube = agent.AddComponent<TubeMeshGenerator>();
        tube.TubeVertexCount = _properties.Roundness;
        tube.Radius = _properties.Thickness;
        tube.SetPlayerPathPoints(PomDataHandler.GetAllPlayerPosFromTo(playerID, from, to, _pomData), GetYScale(), GetXScale());
        tube.Render();
        tube.SetMaterial(_properties.AgentMaterials[playerID]);

        agent.transform.localEulerAngles = Vector3.zero;
        return agent;
    }
    private void ClearCurrentSTC()
    {
        Destroy(_board);
        Destroy(_agent1);
        Destroy(_agent2);
        Destroy(_agent3);
        Destroy(_agent4);

        foreach (STCItemSpawner spawner in _bombSpawners)
            spawner.DestroyObjectHolder();

        _flameSpawner.DestroyObjectHolder();
    }
    #endregion
}

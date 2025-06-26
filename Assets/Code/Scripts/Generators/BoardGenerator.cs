using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    #region Variables
    private GameObject _blockHolder, _stepSelectorObject;
    private STCStepSelector _stepSelector;
    private STCManager _parentSTC;
    private float _xScale, _yScale;
    private int[][][] _boardStates;
    private int _boardSize, _from, _step;
    #endregion

    /// <summary>
    /// Starts the Game Board drawing.
    /// </summary>
    /// <param name="step">The current step or level of the game board to build.</param>
    public void StartBoardDrawing(int from, int to, STCManager parentSTC)
    {
        _from = from;

        _parentSTC = parentSTC;
        _xScale = _parentSTC.GetXScale();
        _yScale = _parentSTC.GetYScale();
        _boardStates = PomDataHandler.GetBoardStatesFromTo(from, to, _parentSTC.PomData);
        _boardSize = PomDataHandler.GetBoardSize(_parentSTC.PomData);
    }

    public int GetCurrentStep()
    {
        return _step;
    }
    public void SetCurrentStep(int step)
    {
        _stepSelector.SetCurrentStep(step);
    }
    /// <summary>
    /// Constructs the game board for a given step by iterating through each cell
    /// and instantiating the appropriate game object based on the board state.
    /// </summary>
    /// <param name="step">The current step or level of the game board to build.</param>
    public void BuildBoard(int step)
    {
        _step = step;
        RecreateBlockHolder();

        // Create the Blocks
        for (int row = 0; row < _boardSize; row++)
        {
            for (int col = 0; col < _boardSize; col++)
            {
                Vector3 position = new Vector3(row * _xScale, CalculateCurrentPos(_step), col * _xScale);
                // Check what type the block is
                switch (_boardStates[step - _from][row][col])
                {
                    case 1:
                        CreateBlock(_parentSTC.Properties.RigidBlockModel, position, _xScale);
                        break;
                    case 2:
                        CreateBlock(_parentSTC.Properties.WoodenBlockModel, position, _xScale);
                        break;
                    default:
                        break;
                }
            }
        }
        _blockHolder.transform.localEulerAngles = Vector3.zero;
    }

    private void CreateBlock(GameObject prefab, Vector3 position, float xScale)
    {
        GameObject block = Instantiate(prefab, position + _blockHolder.transform.position, Quaternion.identity, _blockHolder.transform);
        block.transform.localScale = new Vector3(xScale, xScale, xScale);
        block.transform.localEulerAngles = Vector3.zero;
    }

    private void RecreateBlockHolder()
    {
        Destroy(_blockHolder);
        _blockHolder = new GameObject("Block Holder");
        _blockHolder.transform.parent = transform;
        _blockHolder.transform.localPosition = new Vector3(0, 0.0001f, 0);
    }

    private float CalculateCurrentPos(int step)
    {
        return (step * _yScale) - (_from * _yScale);
    }

    public void CreateSelector(int from, int to, int currentStep)
    {
        _stepSelectorObject = Instantiate(_parentSTC.Properties.StepSelectorObject, transform);
        _stepSelector = _stepSelectorObject.GetComponent<STCStepSelector>();
        _stepSelector.InitializeAsHeatmap(_xScale, _yScale, from, to, from, PomDataHandler.CalculateBombPlacementsPerCell(from, to, _parentSTC.PomData));
        _stepSelector.onStepChange += BuildBoard;
        BuildBoard(_stepSelector.GetCurrentStep());
    }
    public void RemoveSelector()
    {
        Destroy(_stepSelectorObject);
    }
}

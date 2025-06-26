using UnityEngine;
using System;

public class STCRangeSelector : MonoBehaviour
{
    private float _xScale, _yScale;
    private int[] _selectedRange = new int[2];
    private STCStepSelector _bottomSelector, _topSelector;
    private GameObject[] _selectors = new GameObject[3];
    private STCRangeCenterHandle _centerHandle;

    #region Event
    public event Action<int, int> onRangeChanged;
    public void RangeChanged()
    {
        onRangeChanged?.Invoke(_selectedRange[0], _selectedRange[1]);
    }
    #endregion
    public void Initialize(int from, int to, STCManager parent)
    {
        _xScale = parent.GetXScale();
        _yScale = parent.GetYScale();

        _selectedRange[0] = from;
        _selectedRange[1] = to;

        // Bottom Selector
        _selectors[0] = Instantiate(parent.Properties.StepSelectorObject, transform);
        _bottomSelector = _selectors[0].GetComponent<STCStepSelector>();
        _bottomSelector.Initialize(_xScale, _yScale, 0, parent.PomData.state.Length, _selectedRange[0], parent.Properties.RangeSelectorMaterial);
        _bottomSelector.onStepChange += UpdateFrom;

        // Top Selector
        _selectors[1] = Instantiate(parent.Properties.StepSelectorObject, transform);
        _topSelector = _selectors[1].GetComponent<STCStepSelector>();
        _topSelector.Initialize(_xScale, _yScale, 0, parent.PomData.state.Length, _selectedRange[1], parent.Properties.RangeSelectorMaterial);
        _topSelector.onStepChange += UpdateTo;

        // Link selectors
        _bottomSelector.SetOtherSelector(_topSelector);
        _topSelector.SetOtherSelector(_bottomSelector);

        CreateCenterHandler(parent);
        RangeChanged();
    }

    public int[] GetRange()
    {
        return _selectedRange;
    }

    public void DestroySelectors()
    {
        _bottomSelector.onStepChange -= UpdateFrom;
        _topSelector.onStepChange -= UpdateTo;
        _centerHandle.onStepChange -= UpdateTopAndBottomSelector;
        Destroy(_selectors[0]);
        Destroy(_selectors[1]);
        Destroy(_selectors[2]);
        Destroy(_centerHandle);
    }
    public int CalculateCurrentStep(float yPosition)
    {
        return Mathf.RoundToInt(yPosition / _yScale);
    }

    private void UpdateFrom(int step)
    {
        _selectedRange[0] = step;
        RangeChanged();
    }

    private void UpdateTo(int step)
    {
        _selectedRange[1] = step;
        RangeChanged();
    }

    private void CreateCenterHandler(STCManager parent)
    {
        // Center Movement Handle
        _selectors[2] = Instantiate(Resources.Load<GameObject>("Prefabs/STCCenterHandle"), transform);
        _centerHandle = _selectors[2].GetComponent<STCRangeCenterHandle>();
        _centerHandle.Initialize(_yScale, parent.PomData.state.Length - 1, this);
        _centerHandle.onStepChange += UpdateTopAndBottomSelector;
    }

    private void UpdateTopAndBottomSelector(int stepDelta)
    {
        int newTopStep = _topSelector.GetCurrentStep() + stepDelta;
        _topSelector.SetCurrentStep(newTopStep);
        int newBottomStep = _bottomSelector.GetCurrentStep() + stepDelta;
        _bottomSelector.SetCurrentStep(newBottomStep);
    }


}

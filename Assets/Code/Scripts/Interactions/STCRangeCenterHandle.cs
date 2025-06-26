using System;
using UnityEngine;

public class STCRangeCenterHandle : MonoBehaviour
{
    #region Variables
    private float _yScale;
    public int _step, _previousStep, _stepChange, _maxStep;
    public int[] _selectedRange = new int[2];
    #endregion

    #region Event
    public event Action<int> onStepChange;
    public void StepChanged()
    {
        onStepChange?.Invoke(_stepChange);
    }
    #endregion

    void Update()
    {
        _step = CalculateCurrentStep();
        if (_step == _previousStep) return;
        UpdatePosition();
        _stepChange = _step - _previousStep;

        // Prevent moving selectors down if bottom selector is at step 0
        if (_stepChange < 0 && _selectedRange[0] <= 0)
        {
            _step = _previousStep;
            UpdatePosition();
            return;
        }
        if (_stepChange > 0 && _selectedRange[1] >= _maxStep)
        {
            _step = _previousStep;
            UpdatePosition();
            return;
        }

        StepChanged();
        _previousStep = _step;
    }

    #region Public Methods
    public void Initialize(float yScale, int maxStep, STCRangeSelector parent)
    {
        _yScale = yScale;
        _maxStep = maxStep;

        parent.onRangeChanged += SelectedRangeChanged;
    }
    #endregion

    #region Private Methods

    // Update Position and Scale
    private void SelectedRangeChanged(int from, int to)
    {
        _selectedRange[0] = from;
        _selectedRange[1] = to;

        // Position in Center
        _step = (from + to) / 2;
        transform.localPosition = new Vector3(transform.localPosition.x, _step * _yScale, transform.localPosition.z);
        _previousStep = _step;

        // Update Y Scale
        float scale = (to - from) * _yScale;
        transform.localScale = new Vector3(1, scale, 1);
    }
    private void UpdatePosition()
    {
        transform.localPosition = CalculateCurrentPosition();
    }
    private int CalculateCurrentStep()
    {
        float yPosition = transform.localPosition.y;
        return Mathf.RoundToInt(yPosition / _yScale);
    }
    private Vector3 CalculateCurrentPosition()
    {
        return new Vector3(0, _step * _yScale, 0);
    }
    #endregion
}

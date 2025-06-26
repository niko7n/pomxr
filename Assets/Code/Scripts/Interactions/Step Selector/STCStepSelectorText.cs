using UnityEngine;
using TMPro;

[RequireComponent(typeof(STCStepSelector))]
public class STCStepSelectorText : MonoBehaviour
{
    [SerializeField] private TMP_Text _stepText;
    private STCStepSelector _stcSelector;

    void Start()
    {
        _stcSelector = GetComponent<STCStepSelector>();
        _stcSelector.onStepChange += OnStepChange;

        _stepText.text = _stcSelector.GetCurrentStep().ToString();
    }

    void onDisable()
    {
        _stcSelector.onStepChange -= OnStepChange;
        Destroy(this);
    }

    private void OnStepChange(int step)
    {
        _stepText.text = step.ToString();
    }
}

using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentText;
    [SerializeField] private TMP_Text _lowestText;
    [SerializeField] private int _interval;

    private int _lowest;
    private int _number;

    void Start()
    {
        _lowest = 2000;
        _number = 0;
    }

    void Update()
    {
        if (_number >= _interval)
        {
            _lowest = 2000;
            _number = 0;
        }

        int fps = (int)(1f / Time.unscaledDeltaTime);
        _currentText.text = fps.ToString();

        if (fps <= _lowest)
        {
            _lowest = fps;
            _lowestText.text = _lowest.ToString();
        }
        _number++;
    }
}

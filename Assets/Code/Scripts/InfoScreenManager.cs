using UnityEngine;
using TMPro;
public class InfoScreenManager : MonoBehaviour
{
    private STCBase _parentBase;

    #region UI Elements
    [Header("Text References")]
    [SerializeField] TMP_Text _curGame;
    [SerializeField] TMP_Text _gameLength;
    [SerializeField] TMP_Text _nameTeamOne;
    [SerializeField] TMP_Text _nameTeamTwo;

    private PommermanData _pomData;
    #endregion

    public void Initialize(STCBase parent)
    {
        _parentBase = parent;
        _pomData = _parentBase.GetPomData();

        SetText();
    }

    public void TestButton()
    {
        Debug.Log("BUTTON PRESSED");
    }
    public void ToggleBombs()
    {
        _parentBase.ToggleBombs();
    }
    public void TogglePickUps()
    {
        _parentBase.TogglePickUps();
    }
    private void SetText()
    {
        _curGame.text = $"Game #: {PomDataHandler.GetGameId(_pomData)}";
        _gameLength.text = _pomData.state.Length.ToString();
        _nameTeamOne.text = "hakozakijunctions";
        _nameTeamTwo.text = "navocado";
    }
}

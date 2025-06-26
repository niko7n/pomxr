using UnityEngine;

public class HighlightButtonController : MonoBehaviour
{
    [SerializeField] STCBase _stcBase;
    [SerializeField] MeshRenderer[] _btnMeshRenderers;
    [SerializeField] Material _highlightMaterial;
    private STCProperties _props;

    void Start()
    {
        _props = _stcBase.GetSTCProperties();
    }

    public void UpdateButtonMaterials()
    {
        for (int i = 0; i < _btnMeshRenderers.Length; i++)
        {
            _btnMeshRenderers[i].material = _props.AgentMaterials[i];
        }
    }

    public void HighlightButton(int id)
    {
        _btnMeshRenderers[id - 1].material = _highlightMaterial;
    }

    public void UnHighlightButton(int id)
    {
        _btnMeshRenderers[id - 1].material = _props.AgentMaterials[id - 1];
    }
}

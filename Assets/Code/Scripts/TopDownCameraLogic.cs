using UnityEngine;
using UnityEngine.UI;

public class TopDownCameraLogic : MonoBehaviour
{
    [SerializeField] private RawImage _viewer;
    [SerializeField] private Camera _camera;
    public RenderTexture _renderTexture;

    public void Initialize(float cameraSize, float viewerScale)
    {
        Destroy(_renderTexture);
        _viewer.texture = null;
        _viewer.transform.localScale = new Vector3(viewerScale, viewerScale, viewerScale);
        _camera.orthographicSize = cameraSize;
        CreateViewerTexture();
    }

    private void CreateViewerTexture()
    {
        // Create a new Render Texture with specified width, height, depth, and format
        _renderTexture = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGB32);
        _renderTexture.wrapMode = TextureWrapMode.Clamp;
        _renderTexture.filterMode = FilterMode.Bilinear;

        // Create the Render Texture
        _renderTexture.Create();

        // Assign the Render Texture to a camera
        _camera.targetTexture = _renderTexture;
        _camera.stereoTargetEye = StereoTargetEyeMask.None;
        _viewer.texture = _renderTexture;
    }
}

using UnityEngine;

public class STCEventMarker : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private GameObject _indicator;
    private Vector3 _stcCentrePosition, _goalPosition;
    private Material _material;
    private float _speed, _width;
    private bool _isOutside;

    void Update()
    {
        if (_indicator)
        {
            _stcCentrePosition = transform.TransformPoint(new Vector3(_width / 2, 0, _width / 2));
            _indicator.transform.RotateAround(_stcCentrePosition, Vector3.up, _speed * Time.deltaTime);
            _indicator.transform.LookAt(Camera.main.transform);
        }

        if (!_isOutside) return;

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, _indicator.transform.position);
        _lineRenderer.SetPosition(1, transform.TransformPoint(_goalPosition));
    }

    /// <summary>
    /// Initializes the class by setting the goal position, speed, width and adding necessary game components.
    /// </summary>
    public void Initialize(Vector3 goalPosition, float speed, float width, Material material)
    {
        _goalPosition = goalPosition;
        _material = material;
        _speed = speed;
        _width = width;
        _isOutside = true;

        CreateRotatingSpriteRenderer(_goalPosition.y, width * 1.25f);
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.02f;
        _lineRenderer.material = Resources.Load<Material>("Materials/UnlitWhite");

        // Reset Rotation
        transform.localEulerAngles = Vector3.zero;
    }

    // The goal Position must be scaled already
    public void Initialize(float xScale, Vector3 goalPosition, Sprite sprite, Material material)
    {
        _goalPosition = goalPosition;
        _material = material;
        _isOutside = false;

        CreateSpriteRenderer(xScale, sprite);

        // Reset Rotation
        transform.localEulerAngles = Vector3.zero;
    }

    private void CreateRotatingSpriteRenderer(float height, float distance)
    {
        _indicator = new GameObject("Event Marker");
        _indicator.transform.parent = transform;
        _indicator.transform.position = _stcCentrePosition;
        var localPositionOffset = new Vector3(distance, height, 0);
        _indicator.transform.localPosition = localPositionOffset;
        _indicator.transform.localScale = new Vector3(.05f, .05f, .05f);
        SpriteRenderer sr = _indicator.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/skull");
        sr.color = _material.GetColor("_BaseColor");
    }

    private void CreateSpriteRenderer(float xScale, Sprite sprite)
    {
        _indicator = new GameObject("Event Marker");
        _indicator.transform.parent = transform;
        _indicator.transform.localPosition = _goalPosition;
        xScale = xScale * 0.25f;
        _indicator.transform.localScale = new Vector3(xScale, xScale, xScale);
        SpriteRenderer sr = _indicator.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = _material.GetColor("_BaseColor");
    }
}

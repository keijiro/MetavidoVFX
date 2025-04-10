using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;
using Klak.Math;

public sealed class CameraController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] UIDocument _ui = null;
    [SerializeField] Camera _camera = null;
    [SerializeField] Transform _pivotNode = null;
    [SerializeField] Transform _slideNode = null;

    #endregion

    #region Camera controlling parameters

    [field:Space]
    [field:SerializeField] public float RotationSpeed { get; set; } = 2.3f;
    [field:SerializeField] public float PitchLimit { get; set; } = 1.1f;
    [field:Space]
    [field:SerializeField] public float ZoomSpeed { get; set; } = 0.5f;
    [field:SerializeField] public float2 DistanceRange { get; set; } = math.float2(2, 4);
    [field:Space]
    [field:SerializeField] public float2 FovRange { get; set; } = math.float2(41.5f, 55);
    [field:Space]
    [field:SerializeField] public float TweenSpeed { get; set; } = 5;
    [field:SerializeField] public float DelayToReset { get; set; } = 15;

    #endregion

    #region Public runtime properties

    public float ZoomParam => _zoom.current;

    #endregion

    #region Private members

    float2 _rotation;
    (float target, float current) _zoom;
    float _idleTime;

    #endregion

    #region TouchDragManipulator callbacks

    void OnDragging(float2 delta)
    {
        // Rotation
        _rotation += delta.yx * RotationSpeed;
        _rotation.x = math.clamp(_rotation.x, -PitchLimit, PitchLimit);

        // Pulling up to the minimum zoom value
        _zoom.target = math.max(_zoom.target, 0.3334f);

        // Not idle
        _idleTime = 0;
    }

    void OnScrolling(float delta)
    {
        // Zoom
        _zoom.target = math.saturate(_zoom.target - ZoomSpeed * delta);

        // Not idle
        _idleTime = 0;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        var drag = new TouchDragManipulator();
        drag.OnDragging += OnDragging;
        drag.OnScrolling += OnScrolling;

        _ui.rootVisualElement.AddManipulator(drag);
    }

    void Update()
    {
        var dt = Time.deltaTime;

        // Zoom-in by timeout
        if ((_idleTime += dt) > DelayToReset)
            _zoom.target = math.saturate(_zoom.target - dt);

        // Smooth zoom
        _zoom.current = ExpTween.Step(_zoom.current, _zoom.target, TweenSpeed);

        // Rotation reset on full zoom-in
        if (_zoom.current < 0.05f) _rotation = 0;

        // Rotation calculation
        var rot = quaternion.Euler(math.float3(_rotation, 0));
        var rot_amp = math.saturate(_zoom.current * 3);
        rot = math.slerp(quaternion.identity, rot, rot_amp);
        rot = ExpTween.Step(_pivotNode.localRotation, rot, TweenSpeed);

        // Distance calculation
        var dist = math.lerp(DistanceRange.x, DistanceRange.y, _zoom.current);

        // Field of view calculation
        var fov = math.lerp(FovRange.x, FovRange.y, _zoom.current);;

        // Scene object update
        _pivotNode.localRotation = rot;
        _slideNode.localPosition = math.float3(0, 0, -dist);
        _camera.fieldOfView = fov;
    }

    #endregion
}

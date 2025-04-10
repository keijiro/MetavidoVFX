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

    [Space]
    [field:SerializeField] public float RotationSpeed = 2;
    [field:SerializeField] public float PitchLimit = 1;
    [Space]
    [field:SerializeField] public float ZoomSpeed = 0.5f;
    [field:SerializeField] public float2 DistanceRange = math.float2(3, 6);
    [Space]
    [field:SerializeField] public float2 FovRange = math.float2(20, 30);
    [Space]
    [field:SerializeField] public float TweenSpeed = 5;
    [field:SerializeField] public float DelayToReset = 3;

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

        var area = _ui.rootVisualElement.Q("drag-area");
        area.AddManipulator(drag);
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

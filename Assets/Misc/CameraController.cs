using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;

public sealed class CameraController : MonoBehaviour
{
    [SerializeField] UIDocument _ui = null;
    [SerializeField] Transform _pivot = null;

    void Start()
    {
        var area = _ui.rootVisualElement.Q("drag-area");
        area.AddManipulator(new CameraControlDragger(_pivot));
    }
}

public class CameraControlDragger : PointerManipulator
{
    Transform _xform;
    int _id;
    float2 _prev;
    float _height;

    bool IsActive => _id >= 0;

    public CameraControlDragger(Transform xform)
    {
        _xform = xform;
        _id = -1;
        activators.Add
          (new ManipulatorActivationFilter{button = MouseButton.LeftMouse});
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerDown(PointerDownEvent e)
    {
        if (IsActive)
        {
            e.StopImmediatePropagation();
            return;
        }

        if (CanStartManipulation(e))
        {
            var pos = math.float3(e.localPosition).xy;
            (_id, _prev) = (e.pointerId, pos);
            _height = target.resolvedStyle.height;
            target.CapturePointer(_id);
            e.StopPropagation();
        }
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        if (!IsActive || !target.HasPointerCapture(_id)) return;

        var pos = math.float3(e.localPosition).xy;
        var delta = (pos - _prev) / _height;
        _prev = pos;

        var rot = (float3)_xform.eulerAngles;
        rot.x = (rot.x + 180) % 360 - 180;
        rot.x = math.clamp(rot.x - delta.y * 90, -80, 80);
        rot.y += delta.x * 90;
        _xform.eulerAngles = rot;

        e.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent e)
    {
        if (!IsActive || !target.HasPointerCapture(_id)) return;
        if (!CanStopManipulation(e)) return;

        _id = -1;
        target.ReleaseMouse();
        e.StopPropagation();
    }
}

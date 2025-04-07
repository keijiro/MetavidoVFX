using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;

public sealed class CameraController : MonoBehaviour
{
    [SerializeField] UIDocument _ui = null;
    [SerializeField] Transform _pivot = null;

    VisualElement _area;
    (int id, float2 prev) _drag = (-1, 0);

    bool IsDragActive => _drag.id >= 0;

    void Start()
    {
        _area = _ui.rootVisualElement.Q("drag-area");
        _area.RegisterCallback<PointerDownEvent>(OnPointerDown);
        _area.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _area.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerDown(PointerDownEvent e)
    {
        if (IsDragActive)
        {
            e.StopImmediatePropagation();
            return;
        }

        _drag = (e.pointerId, math.float3(e.localPosition).xy);
        _area.CapturePointer(_drag.id);
        e.StopPropagation();
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        if (!IsDragActive) return;
        if (!_area.HasPointerCapture(_drag.id)) return;

        var height = _area.resolvedStyle.height;

        var pos = math.float3(e.localPosition).xy;
        var delta = (pos - _drag.prev) / height;
        _drag.prev = pos;

        var rot = (float3)_pivot.localEulerAngles;
        var limit = math.float2(40, 60);
        rot.xy = (rot.xy + 180) % 360 - 180;
        rot.xy = math.clamp(rot.xy + delta.yx * 90, -limit, limit);
        _pivot.localEulerAngles = rot;

        e.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent e)
    {
        if (!IsDragActive) return;
        if (!_area.HasPointerCapture(_drag.id)) return;

        _drag.id = -1;
        _area.ReleaseMouse();

        e.StopPropagation();
    }
}

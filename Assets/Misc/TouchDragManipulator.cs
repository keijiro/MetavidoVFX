using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;
using System;

public class TouchDragManipulator : PointerManipulator
{
    #region Public callback events

    public event Action<float2> OnDragging;
    public event Action<float> OnScrolling;

    #endregion

    #region Touch points (up to two points)

    (int id, float2 pos) _p1;
    (int id, float2 pos) _p2;

    #endregion

    #region Initialization

    public TouchDragManipulator()
    {
        _p1.id = _p2.id = -1;
        activators.Add(new ManipulatorActivationFilter{button = MouseButton.LeftMouse});
    }

    #endregion

    #region Manipulator overrides

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<WheelEvent>(OnWheelScrolled);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<WheelEvent>(OnWheelScrolled);
    }

    #endregion

    #region Manipulator callbacks

    void OnPointerDown(PointerDownEvent e)
    {
        if (!CanStartManipulation(e)) return;

        var id = e.pointerId;
        var pos = math.float3(e.localPosition).xy;

        // Duplication rejection
        // Happens on touch input devices (iOS/Android.)
        if (_p1.id == id || _p2.id == id) return;

        // Slot allocation and initialization
        if (_p1.id < 0)
            _p1 = (id, pos);
        else if (_p2.id < 0)
            _p2 = (id, pos);
        else
            return;

        target.CapturePointer(id);
        e.StopPropagation();
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        var id = e.pointerId;
        var pos = math.float3(e.localPosition).xy;
        float2 delta;

        // Delta amount calculation and state update
        if (_p1.id == id)
            (_p1.pos, delta) = (pos, pos - _p1.pos);
        else if (_p2.id == id)
            (_p2.pos, delta) = (pos, pos - _p2.pos);
        else
            return;

        // Normalization by the target control size
        var style = target.resolvedStyle;
        delta /= math.min(style.width, style.height);

        // Callback invocation
        if (_p1.id < 0 || _p2.id < 0)
            OnDragging?.Invoke(delta);
        else
            OnScrolling?.Invoke(delta.y / 2);

        e.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent e)
    {
        var id = e.pointerId;

        if (_p1.id == id)
            _p1.id = -1;
        else if (_p2.id == id)
            _p2.id = -1;
        else
            return;

        target.ReleasePointer(id);
        e.StopPropagation();
    }

    void OnWheelScrolled(WheelEvent e)
    {
        var delta = e.delta.y / -1000;
#if UNITY_WEBGL && !UNITY_EDITOR
        delta *= 3;
#endif
        OnScrolling(delta);
        e.StopPropagation();
    }

    #endregion
}

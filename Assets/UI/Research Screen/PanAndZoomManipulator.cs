using UnityEngine;
using UnityEngine.UIElements;

public class PanAndZoomManipulator : PointerManipulator
{
    private readonly VisualElement _target;
    private bool _isDragging;
    private Vector2 _startPointerPosition;
    private Vector2 _startTargetPosition;

    private float _currentZoom = 1.0f;
    private const float MinZoom = 0.4f;
    private const float MaxZoom = 2.0f;

    public PanAndZoomManipulator(VisualElement target)
    {
        _target = target;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<WheelEvent>(OnWheel);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<WheelEvent>(OnWheel);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button == (int)MouseButton.LeftMouse || evt.button == (int)MouseButton.MiddleMouse)
        {
            _isDragging = true;
            _startPointerPosition = evt.position;
            _startTargetPosition = new Vector2(_target.transform.position.x, _target.transform.position.y);
            target.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        Vector2 delta = (Vector2)evt.position - _startPointerPosition;
        _target.transform.position = _startTargetPosition + delta;
        evt.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (_isDragging)
        {
            _isDragging = false;
            target.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }

    private void OnWheel(WheelEvent evt)
    {
        float zoomDelta = -evt.delta.y * 0.05f;
        _currentZoom = Mathf.Clamp(_currentZoom + zoomDelta, MinZoom, MaxZoom);
        _target.transform.scale = new Vector3(_currentZoom, _currentZoom, 1f);
        evt.StopPropagation();
    }
}
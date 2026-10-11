using UnityEngine;
using UnityEngine.UIElements;

public class CustomManipulator : PointerManipulator
{
    private bool _isDragging;
    private Vector2 _startPointerPosition;
    private Vector2 _startTargetPosition;
    private Vector2 _currentTranslation = Vector2.zero;

    public float CurrentZoom { get; private set; } = 1.0f;
    public float MinZoom { get; set; } = 0.6f;
    public float MaxZoom { get; set; } = 3.0f;
    public float ZoomSpeed { get; set; } = 0.05f;

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
            _startTargetPosition = _currentTranslation;
            
            target.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!_isDragging) return;

        Vector2 delta = (Vector2)evt.position - _startPointerPosition;
        ApplyTranslationWithClamp(_startTargetPosition.x + delta.x, _startTargetPosition.y + delta.y);
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
        if (target.parent == null) return;

        float parentW = target.parent.layout.width;
        float parentH = target.parent.layout.height;
        float targetW = target.layout.width;
        float targetH = target.layout.height;

        if (parentW == 0 || targetW == 0) return;

        float targetX = target.layout.x;
        float targetY = target.layout.y;

        float oldZoom = CurrentZoom;
        float zoomDelta = -evt.delta.y * ZoomSpeed;
        float newZoom = Mathf.Clamp(CurrentZoom + zoomDelta, MinZoom, MaxZoom);

        if (Mathf.Approximately(oldZoom, newZoom)) return;

        // Координати зміщення, при яких центр полотна точно співпадає з центром вікна перегляду
        float centerFocusX = (parentW - targetW) * 0.5f - targetX;
        float centerFocusY = (parentH - targetH) * 0.5f - targetY;

        // Масштабування позиції відносно центру вікна перегляду
        float zoomRatio = newZoom / oldZoom;
        float newDesiredX = (_currentTranslation.x - centerFocusX) * zoomRatio + centerFocusX;
        float newDesiredY = (_currentTranslation.y - centerFocusY) * zoomRatio + centerFocusY;

        CurrentZoom = newZoom;
        target.style.scale = new Scale(new Vector3(CurrentZoom, CurrentZoom, 1f));

        ApplyTranslationWithClamp(newDesiredX, newDesiredY);
        evt.StopPropagation();
    }

    private void ApplyTranslationWithClamp(float desiredX, float desiredY)
    {
        if (target.parent == null) return;

        float parentW = target.parent.layout.width;
        float parentH = target.parent.layout.height;
        float targetW = target.layout.width;
        float targetH = target.layout.height;

        if (parentW == 0 || targetW == 0) return;

        float targetX = target.layout.x;
        float targetY = target.layout.y;

        // --- Розрахунок осі X ---
        float scaledW = targetW * CurrentZoom;
        float deltaX = (scaledW - targetW) * 0.5f;

        float boundLeft = deltaX - targetX;
        float boundRight = parentW - targetX - targetW - deltaX;

        float clampedX;
        if (scaledW > parentW)
        {
            clampedX = Mathf.Clamp(desiredX, boundRight, boundLeft);
        }
        else
        {
            // Якщо полотно менше за екран — залишаємо рівно по центру
            clampedX = (parentW - targetW) * 0.5f - targetX;
        }

        // --- Розрахунок осі Y ---
        float scaledH = targetH * CurrentZoom;
        float deltaY = (scaledH - targetH) * 0.5f;

        float boundTop = deltaY - targetY;
        float boundBottom = parentH - targetY - targetH - deltaY;

        float clampedY;
        if (scaledH > parentH)
        {
            clampedY = Mathf.Clamp(desiredY, boundBottom, boundTop);
        }
        else
        {
            // Якщо полотно менше за екран — залишаємо рівно по центру
            clampedY = (parentH - targetH) * 0.5f - targetY;
        }

        _currentTranslation = new Vector2(clampedX, clampedY);
        target.style.translate = new Translate(clampedX, clampedY);
    }
}
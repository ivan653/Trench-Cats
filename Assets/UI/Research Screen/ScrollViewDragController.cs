using UnityEngine;
using UnityEngine.UIElements;

public class ScrollViewDragController : MonoBehaviour
{
    private ScrollView scrollView;
    private bool isDragging;
    private float startY;

    void Start()
    {
        // Reference your ScrollView by name or query
        scrollView = GetComponent<ScrollView>();
        
        // Register events with TrickleDown to capture events on children
        scrollView.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        scrollView.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
        scrollView.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        isDragging = true;
        // Store start position + current scroll offset to prevent jumping
        startY = evt.localMousePosition.y + scrollView.scrollOffset.y;
        evt.StopPropagation(); // Prevent default touch simulation if needed
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (isDragging)
        {
            // Calculate new offset based on mouse movement
            float newOffset = startY - evt.localMousePosition.y;
            scrollView.scrollOffset = new Vector2(0, newOffset);
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        isDragging = false;
    }

    void OnDestroy()
    {
        scrollView.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        scrollView.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
        scrollView.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
    }
}   
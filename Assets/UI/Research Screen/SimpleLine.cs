using UnityEngine;
using UnityEngine.UIElements;

public class SimpleLine : VisualElement
{
    private Vector2 _start;
    private Vector2 _end;

    public SimpleLine()
    {
        pickingMode = PickingMode.Ignore;
        generateVisualContent += Draw;
    }

    // Метод, який ми будемо смикати ззовні
    public void UpdatePoints(Vector2 start, Vector2 end)
    {
        _start = start;
        _end = end;

        // ГОЛОВНЕ: наказуємо Unity перемалювати елемент
        MarkDirtyRepaint();
    }

    private void Draw(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        painter.strokeColor = Color.white;
        painter.lineWidth = 4f;

        painter.BeginPath();
        painter.MoveTo(_start);
        painter.LineTo(_end);
        painter.Stroke();
    }
}
using Godot;
using System;

namespace MTG.Frontend;

public partial class TargetingLine : Line2D
{
    public Vector2 StartPosition { get; set; }
    public Vector2 TargetPosition { get; set; }
    public bool IsActive { get; set; }

    public override void _Ready()
    {
        Width = 4.0f;
        DefaultColor = new Color(1.0f, 0.3f, 0.2f, 0.9f); // Crimson red arrow
        ZIndex = 100;
        Visible = false;
    }

    public void StartTargeting(Vector2 startPos)
    {
        StartPosition = startPos;
        IsActive = true;
        Visible = true;
    }

    public void StopTargeting()
    {
        IsActive = false;
        Visible = false;
        ClearPoints();
    }

    public override void _Process(double delta)
    {
        if (!IsActive) return;

        TargetPosition = GetGlobalMousePosition();
        ClearPoints();

        // Generate curved quadratic bezier line
        int segments = 20;
        Vector2 controlPoint = (StartPosition + TargetPosition) / 2.0f + new Vector2(0, -60);

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector2 point = CalculateBezierPoint(t, StartPosition, controlPoint, TargetPosition);
            AddPoint(point);
        }
    }

    private static Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2.0f * u * t * p1) + (tt * p2);
    }
}

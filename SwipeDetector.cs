using Godot;
using System;

public class SwipeDetector : Node
{
    private Vector2? start_pos;
    private Vector2? end_pos;

    private Vector2 viewport;
    [Signal]
    public delegate void Swipe(Vector2 start, Vector2 end);

    private bool timeout;

    public override void _Ready()
    {
        viewport = GetViewport().Size;
    }

    public override void _Process(float delta)
    {
        if (start_pos != null && end_pos != null)
        {
            EmitSignal("Swipe", start_pos, end_pos);
            start_pos = null;
            end_pos = null;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (timeout)
            return;

        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (start_pos == null)
                start_pos = eventMouseButton.Position;
            else
                end_pos = eventMouseButton.Position;
        }
    }

    void _on_RoundTimer_timeout()
    {
        timeout = true;
    }

    void _on_RestartButton_pressed()
    {
        timeout = false;
        start_pos = null;
        end_pos = null;
    }
}

using Godot;
using System;

public class RoundTimer : Timer
{
    //на таймере указано время, в течении которго можно раскручивать барабан
    private bool started;

    [Signal]
    public delegate void RoundStarted();

    public void _on_SwipeDetector_Swipe(Vector2 start_pos, Vector2 end_pos)
    {
        if(started)
            return;
        
        Start();
        started = true;
        EmitSignal("RoundStarted");
    }

    void _on_RestartButton_pressed()
    {
        started = false;
    }
}

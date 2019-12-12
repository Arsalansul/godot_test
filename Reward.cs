using Godot;
using System;

public class Reward : Spatial
{
    [Export]
    public int amount;

    [Signal]
    public delegate void CalcReward(int value, CylinderType cylinderType);


    void _on_CSGCylinder_ClaimReward(Spatial spatial, CylinderType cylinderType)
    {
        if (spatial != this)
            return;
        
        EmitSignal("CalcReward", amount, cylinderType);
    }
}

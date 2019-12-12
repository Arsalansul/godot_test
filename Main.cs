using Godot;
using System;
using System.Collections.Generic;

public class Main : Spatial
{ 
    [Signal]
    public delegate void ChangeRewardText(int reward);
    private int rewardAmount;

    private int valueFromBigCylinder = -1;
    private int valueFromSmallCylinder = -1;
    void _on_Reward_CalcReward(int rewardValue, CylinderType cylinderType)
    {
        if (cylinderType == CylinderType.big)
        {
            valueFromBigCylinder = rewardValue;
            Check();
            return;
        }

        valueFromSmallCylinder = rewardValue;
        Check();
    }

    void Check()
    {
        if (valueFromSmallCylinder < 0 || valueFromBigCylinder < 0 )
            return;

        rewardAmount = valueFromSmallCylinder*valueFromBigCylinder;

        EmitSignal("ChangeRewardText", rewardAmount);

        valueFromBigCylinder = -1;
        valueFromSmallCylinder = -1;
    }
}

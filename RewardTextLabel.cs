using Godot;
using System;

public class RewardTextLabel : RichTextLabel
{
    private int amount;
    public override void _Ready()
    {
        Text = "Swipe to start the game";
    }
    void _on_Main_ChangeRewardText(int rewardAmount)
    {
        amount +=rewardAmount;
        Text = "Your bank:\n" + amount;
    }

}

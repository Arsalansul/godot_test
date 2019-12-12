using Godot;
using System;

public enum CylinderType{big, small}

public class CSGCylinder : Godot.CSGCylinder
{
    [Signal]
    public delegate void ClaimReward(int rewardSpatialIndex, CylinderType cylinderType);

    [Export]
    public CylinderType cylinderType;
    private float speed
    {
        get
        {
            if (timer < accelerationTime)
                return prev_speed + (max_speed - prev_speed)*timer/accelerationTime;
            
            return max_speed;
        }
    }

    private float max_speed;
    private float prev_speed;

    [Export]
    private float accelerationTime = 3f;  //за это время скорость достигнет значение max_speed
    private float timer;
    [Export]
    private float timeForStop = 10f;  //ориентировочное время остановки барабана

    private int desireRewardSpatial;

    private Spatial[] rewardSpatial;

    private bool end;

    private RandomNumberGenerator randomNumberGenerator;

    public override void _Ready()
    {
        rewardSpatial = new Spatial[GetChildCount()];

        var deltaAngle = 360f/GetChildCount();
        float angle = 0;
        for (int i = 0; i < GetChildCount(); i++)
        {
            var child = GetChild(i) as Spatial;
            child.RotationDegrees = new Vector3 (0, angle,0);
            angle +=deltaAngle;
            rewardSpatial[i] = child;
        }

        randomNumberGenerator = new RandomNumberGenerator();
    }
    public override void _Process(float delta)
    {
        timer += delta;
        Rotate(Vector3.Up, delta * speed);

        if (end && Mathf.Abs(speed) <=0.01f)
        {
            EmitSignal("ClaimReward", rewardSpatial[desireRewardSpatial], cylinderType);
            end = false;
        }

        if (!end && Mathf.Abs(speed) <=0.01f)
        {
            foreach(var sp in rewardSpatial)
            {
                randomNumberGenerator.Randomize();
                sp.Translation = sp.Translation.LinearInterpolate(
                    new Vector3 (randomNumberGenerator.Randf()*2 - 1, sp.Translation.y, randomNumberGenerator.Randf()*2 -1),
                    delta);
            }
        }
    }

    public void _on_SwipeDetector_Swipe(Vector2 start_pos, Vector2 end_pos)
    {
        prev_speed = speed;
        var swipeVector = end_pos - start_pos;

        max_speed += swipeVector.Normalized().Cross((start_pos - GetViewport().Size/2).Normalized());
        timer = 0;       
    }    

    void _on_RoundTimer_timeout()
    {
        prev_speed = speed;
        max_speed = 0;
        timer = 0;
        accelerationTime = GetTimeForStop(desireRewardSpatial,prev_speed);
        end = true;
    }

    void _on_RestartButton_pressed()
    {
        prev_speed = 0;
        max_speed = 0;
        timer = 0;
        accelerationTime = 3f;
        end = false;
    }

    private float GetTimeForStop(int indexOfDesireChild, float speed)
    {
        //это для того, чтобы остановка занимала примерно одно и тоже время
        var roundsForStop =(int) (Mathf.Abs(speed)*180/Mathf.Pi/2*timeForStop/360);
        
        var pathInDegrees = rewardSpatial[indexOfDesireChild].GlobalTransform.basis.GetEuler().y *180/ Mathf.Pi - (180 + 360 * roundsForStop) * Mathf.Sign(speed);

        return Mathf.Abs(pathInDegrees*2*Mathf.Pi/180/speed);
    }

    void _on_RoundTimer_RoundStarted()
    {
        randomNumberGenerator.Randomize();
        desireRewardSpatial = randomNumberGenerator.RandiRange(0, rewardSpatial.Length - 1);
        GD.Print(desireRewardSpatial);

        foreach(var sp in rewardSpatial)
        {
            randomNumberGenerator.Randomize();
            sp.Translation = new Vector3 (0, sp.Translation.y, 0);
        }
    }
}

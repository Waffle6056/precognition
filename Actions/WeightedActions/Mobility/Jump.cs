using Godot;
using System;

public partial class Jump : Roll
{

    [Export]
    public float JumpDistance = 15f;
    public Vector3 GlobalVelocity;
    public Vector3 upVelo; 
    public void SetVelocityParams(Vector3 currentVelocity, float gravity)
    {
        // used to adjust the dash to keep the current velocity of the player as well as counteract gravity

        GlobalVelocity = currentVelocity * new Vector3(1,0,1);
        if (JumpDistance > 0)
            upVelo = (float)Math.Sqrt(Math.Abs(-2 * gravity * JumpDistance)) * Vector3.Up;
        else
            upVelo = JumpDistance * Vector3.Up;
        LocalOffset = (GlobalVelocity * GlobalBasis + upVelo) * ActionProperties.ActingMaximumTime;
        //GD.Print("localOffset=" + LocalOffset);
        //GD.Print("GlobalVelocity=" + GlobalVelocity);
        //GD.Print("GlobalBasis=" + GlobalBasis);
        //GD.Print("upVelo=" + upVelo);
        //GD.Print("ActionProperties.ActingMaximumTime=" + ActionProperties.ActingMaximumTime);
        Distance = LocalOffset.Length();

    }
    public override void _PhysicsProcess(double delta)
    {
        LocalOffset = (GlobalVelocity * GlobalBasis + upVelo) * ActionProperties.ActingMaximumTime;
        //GD.Print(LocalOffset + " " + GlobalVelocity);
        base._PhysicsProcess(delta);
    }
}
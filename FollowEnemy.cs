using Godot;
using System;

public partial class FollowEnemy : Player
{
	// Called when the node enters the scene tree for the first time.
	protected override Vector3 checkMovement(String k, String op, int ind, Vector3 vec){
		
		float val = k.Equals("Up") || k.Equals("Down") ? TargetPos.Z : TargetPos.X;
		float playerval = k.Equals("Up") || k.Equals("Down") ? Player.Instance.TargetPos.Z : Player.Instance.TargetPos.X;
		if (k.Equals("Up") || k.Equals("Left")){
			float swap = val;
			val = playerval;
			playerval = swap;
		
		}
			
		if (playerval > val && CD[ind] <= 0 && !RewindController.Instance.IsPaused)
		{
			TargetPos += vec;
			CD[ind] = HoldMovementCD;
			return vec;
		}

		return Vector3.Zero;
	}
	public override void _Ready()
	{
	}
}

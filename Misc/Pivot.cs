using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class Pivot : Node3D
{
	[Export]
	public bool DirectionLocked = false;
	[Export]
	public Vector3[] Directions = {
		Vector3.Forward,
		Vector3.Left,
		Vector3.Back,
		Vector3.Right,
	};
	[Export]
	public bool FreezeOnPause = true;
	[Export]
	public bool FreezeOnActive = false;
	[Export]
	public Entity Root;
	[Export]
	public bool Freeze = false;
	[Export]
	public float SlerpWeight = 0.5f;
	[Export]
	public Vector3 Target = Vector3.Forward;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public virtual void UpdateTarget(){
        Vector3 nTarget = Player.Instance.GlobalPosition - GlobalPosition;
		if (!nTarget.IsEqualApprox(Vector3.Zero))
		{
			Target = nTarget;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Freeze || 
		FreezeOnPause && RewindController.Instance.IsPaused || 
		FreezeOnActive && Root.IsActing)
			return;
		
		UpdateTarget();

		//GD.Print(mousePos);
		//GD.Print(Target+"-");
		Basis next = Basis.LookingAt(Target);
		if (!DirectionLocked)
			GlobalBasis = GlobalBasis.Slerp(next, SlerpWeight);
		else {
			Basis closestBasis = next;
			float closestAngle = 360f;
			Quaternion target = next.GetRotationQuaternion();
			foreach (Vector3 Direction in Directions){
				Basis b = Basis.LookingAt(Direction.Normalized());
				float angle = b.GetRotationQuaternion().AngleTo(target);
				if (angle < closestAngle){
					closestAngle = angle;
					closestBasis = b;
				}
			}
			GlobalBasis = closestBasis;
		}
	}
}

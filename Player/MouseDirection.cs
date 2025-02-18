using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class MouseDirection : Node3D
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
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Freeze || 
		FreezeOnPause && RewindController.Instance.IsPaused || 
		FreezeOnActive && Root.IsActing)
			return;
		Viewport vp = GetViewport();
		Vector2 mousePos = vp.GetMousePosition();
		mousePos -= vp.GetVisibleRect().Size / 2;
		//GD.Print(mousePos);
		Basis next = Basis.LookingAt(new Vector3(mousePos.X,0,mousePos.Y));
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

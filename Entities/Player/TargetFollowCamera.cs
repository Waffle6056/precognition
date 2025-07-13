using Godot;
using System;


public partial class TargetFollowCamera : Camera3D
{
    [Export]
    public Node3D Target;
    [Export]
    public Area3D SelectionBox;
    [Export]
    public Node3D[] YPivot = new Node3D[0];
    [Export]
    public Node3D[] XPivot = new Node3D[0];
    [Export]
    public Node3D[] RPivot = new Node3D[0];

    public Basis TargetRotation = Basis.Identity;
    public Basis MouseRotation = Basis.Identity;
    [Export]
    public Vector3 IdleOffset = new Vector3(0, 10, 0);
    [Export]
    public Vector3 FocusOffset = new Vector3(0, 5, 0);
    [Export]
    public bool Focused;
    [Export]
    public float RotationWeight = .5f;
    [Export]
    public float FollowWeight = .5f;
    [Export]
    public float DPI = 1;

    [Export]
    public Node3D PivotIndicator;
    public Node3D pivot;
    public bool lockOn = false;
    private Vector3 _lastPivotDir = Vector3.Forward;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
        if (Input.IsActionJustPressed("LockOn"))
        {
            GD.Print("Lock on pressed");
            lockOn = !lockOn;
            pivot = selectPivot();
        }


        if (lockOn)
        {
            PivotIndicator.Visible = true;
            PivotIndicator.GlobalPosition = pivot.GlobalPosition;
            Vector3 targetDir = ((pivot.GlobalPosition - GlobalPosition) * new Vector3(1, 0, 1)).Normalized();
            Vector3 startDir = (-GlobalBasis[2]*new Vector3(1, 0, 1)).Normalized();
            TargetRotation = TargetRotation.Rotated(Vector3.Up, startDir.SignedAngleTo(targetDir,Vector3.Up));
            _lastPivotDir = targetDir;
            //GD.Print(startDir.SignedAngleTo(targetDir, Vector3.Up));
        }
        else
            PivotIndicator.Visible = false;

        TargetRotation = TargetRotation.Orthonormalized();


        //TargetRotation = TargetRotation.Clamp(-0.5f, 0.5f);
        //GD.Print(TargetRotation+" "+MouseVelo+" "+((float)(MouseVelo.X * delta)));

        GlobalBasis = GlobalBasis.Slerp(TargetRotation,RotationWeight);
		foreach (Node3D n in XPivot)
			n.GlobalRotation = new Vector3(GlobalRotation.X, 0, 0);
        foreach (Node3D n in YPivot)
            n.GlobalRotation = new Vector3(0, GlobalRotation.Y, 0);
        foreach (Node3D n in RPivot)
            n.GlobalRotation = GlobalRotation;
		//GD.Print("CHILD");
        Vector3 Offset = Focused ? FocusOffset : IdleOffset;
		GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition + GlobalBasis * Offset, FollowWeight);
	}
    Node3D selectPivot()
    {
        Node3D closest = null;
        float closestAngle = 0;
        foreach (Node3D n in SelectionBox.GetOverlappingBodies())
        {
            Vector3 tar = (n.GlobalPosition - GlobalPosition).Normalized();

            float angle = (-TargetRotation[2]).AngleTo(tar);
            if (closest == null || angle < closestAngle)
            {
                closest = n;
                closestAngle = angle;
            }
        }

        return closest;
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion)
            rotateCamera((@event as InputEventMouseMotion).ScreenRelative);

    }
    void rotateCamera(Vector2 motion)
    {
        Vector2 MouseDelta = motion * DPI;

        TargetRotation = TargetRotation.Rotated(Vector3.Up, -MouseDelta.X);

        float YDelta = -MouseDelta.Y;
        YDelta = Math.Clamp(YDelta, -(-TargetRotation[2]).AngleTo(Vector3.Down) + .1f, (-TargetRotation[2]).AngleTo(Vector3.Up) - .1f);
        //GD.Print(YDelta);
        TargetRotation = TargetRotation.Rotated(TargetRotation[0].Normalized(), YDelta);
    }
}

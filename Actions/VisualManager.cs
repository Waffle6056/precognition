using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class VisualManager : Node3D, RewindableObject
{
	[Export]
	public AnimationPlayer ActionAnimator{get;set; }
    [Export]
    public Node3D VisualRoot { get; set; }
    [Export]
    public ShaderMaterial GhostMaterial { get; set; }
    public double Hitstop = 0;
	public double ManagerTime = 0;
    [Export]
    public bool BulletTimeAffected = true;
    [Export]
    public bool CreateTrail = false;
    [Export]
    public float CreateTrailIntervals = .3f;
    public double createTrailTimer = 0;
    [Export]
    public float ColorChangeMul = 3f;
    //public double HitstopCache = 0;
    public void Play(String name)
	{
		if (ActionAnimator.IsPlaying())
		{
			EndAnimation();
			//GD.Print("a");
		}
		
		ActionAnimator.Play(name);
	}
    public void Play(String name, float offset)
    {
        if (ActionAnimator.IsPlaying())
            EndAnimation();
        //GD.Print(name);
        ActionAnimator.Play(name);
		ActionAnimator.Advance(offset);
    }


    public void EndAnimation()
	{
		ActionAnimator.Advance(ActionAnimator.CurrentAnimationLength - ActionAnimator.CurrentAnimationPosition);
	}
    
    static void applyMaterial(Node root, Material m)
	{
		if (root is GeometryInstance3D)
		{
			(root as GeometryInstance3D).MaterialOverride = m;
            (root as GeometryInstance3D).SetLayerMaskValue(layerNumber:2, true);
            (root as GeometryInstance3D).SetLayerMaskValue(layerNumber:1, false);
        }
		foreach (Node child in root.GetChildren())
			applyMaterial(child, m);
	}
	//public VisualManager PlayFuture(float offset)
	//{
	//	//GD.Print(ActionAnimator.CurrentAnimation);
	//	return PlayFuture(offset, ActionAnimator.CurrentAnimation, Vector3.Zero);
	//}

    //private VisualManager dupe;
    //Random rnd = new Random();
    public VisualManager PlayFuture(float offset, String animName, Vector3 localOffset, float transparency, int renderPriority)
	{
        VisualManager dupe = Duplicate() as VisualManager;
        dupe.VisualRoot.ProcessMode = ProcessModeEnum.Disabled;
		dupe.BulletTimeAffected = false;
		dupe.CreateTrail = true;
		dupe.ManagerTime = ManagerTime;
		dupe.VisualRoot.Visible = false;

        ShaderMaterial mat = GhostMaterial.Duplicate() as ShaderMaterial;
		mat.SetShaderParameter("color", new Vector4(transparency, (float)Math.Sin(ManagerTime* ColorChangeMul)/2+.5f,.5f,1));
		mat.RenderPriority = renderPriority;
		dupe.GhostMaterial = mat;
        //GD.Print((Vector4)(dupe.GhostMaterial as ShaderMaterial).GetShaderParameter("color") + "orig");
        applyMaterial(dupe.VisualRoot, mat);
		//dupe.Name
		//GD.Print(dupe.ActionAnimator.CurrentAnimation);

		dupe.ActionAnimator.SpeedScale = 1f;
        dupe.Play(animName, offset);


        BulletTime.outsideActivations++;
        dupe.ActionAnimator.AnimationFinished += dupe.DeleteFutureSignal;
        Tween t = dupe.CreateTween();
		t.TweenProperty(dupe, "position", localOffset, dupe.ActionAnimator.CurrentAnimationLength/2).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        //t.TweenProperty(dupe, "position", Vector3.Zero, dupe.ActionAnimator.CurrentAnimationLength/2).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Sine);
        //dupe.Position += localOffset;


        AddSibling(dupe);

        return dupe;
	}
    public void EndFuture()
    {
        //dupe?.EndAnimation();
    }

    public void DeleteFutureSignal(StringName n)
	{
        BulletTime.outsideActivations--;
        QueueFree();
	}
    public void HitStop(double amt)
    {
		Hitstop += amt;
        //HitstopCache += amt;
    }
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (BulletTimeAffected)
		{
			ActionAnimator.SpeedScale = BulletTime.SpeedScale;
			delta *= BulletTime.SpeedScale;
		}

		ManagerTime += delta;
		if (Hitstop > 0) {
			ActionAnimator.Pause();
			Hitstop -= delta;
			if (Hitstop <= 0)
				ActionAnimator.Play();
		}
		if (CreateTrail)
		{
			if (createTrailTimer > 0)
			{
				createTrailTimer -= delta;
				
			}
			if (createTrailTimer <= 0)
			{
				createTrailTimer = CreateTrailIntervals;

                Node3D dupe = VisualRoot.Duplicate() as Node3D;
                dupe.ProcessMode = ProcessModeEnum.Disabled;
				dupe.TopLevel = true;
				dupe.Visible = true;
				//GD.Print((Vector4)(GhostMaterial as ShaderMaterial).GetShaderParameter("color") + "new");
				Vector4 col = (Vector4)GhostMaterial.GetShaderParameter("color");
				//GD.Print(col);
                ShaderMaterial mat = GhostMaterial.Duplicate() as ShaderMaterial;
                //GD.Print(col);
                //GD.Print((Vector4)(GhostMaterial as ShaderMaterial).GetShaderParameter("color")+"new");
                mat.SetShaderParameter("color", new Vector4(col.X, (float)Math.Sin(ManagerTime * ColorChangeMul) / 2 + .5f, .5f, 1));
				mat.RenderPriority = GhostMaterial.RenderPriority + 1;
				//GD.Print(mat.RenderPriority+" "+GhostMaterial.RenderPriority);
                GhostMaterial = mat;
				//GD.Print(mat.GetShaderParameter("color")+" "+ (Vector4)GhostMaterial.GetShaderParameter("color"));
                applyMaterial(VisualRoot, mat);

                //GD.Print(dupe.GlobalPosition+" "+GlobalPosition);
                AddChild(dupe);
				dupe.GlobalTransform = VisualRoot.GlobalTransform;
				//GD.Print(dupe.GlobalPosition+" "+GlobalPosition);
                
            }

        }
		//else
		//{
		//	//ActionAnimator.Advance(HitstopCache);
		//	//HitstopCache = 0;
		//}
    }
  //  public override void _Ready()
  //  {
  //      base._Ready();
		//GD.Print("AGAIN!");
  //  }

    public int DataLength{get{return 2;}}
	public List<Object> GetData()
	{		
		List<Object> data = new List<Object>
		{
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimation : "",
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimationPosition : 0.0,
		};
		return data;
	}

	public void SetData(List<Object> data)
	{
		if (ActionAnimator != null){
			ActionAnimator.CurrentAnimation = (String)data[0];
			ActionAnimator.Seek(              (double)data[1], true);
		}
	}
}
interface IAnimated
{
	[Export]
	public VisualManager Animation{get; set;}
	[Export]
	public String AnimName { get; set; }
	[Export]
	public Vector3 ForesightOffset { get; set; }
}

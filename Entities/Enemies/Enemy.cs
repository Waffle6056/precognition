using Godot;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class Enemy : Entity
{
    [Export]
    public double weightThreshold = 60;
    //[Export]
    //public double DisplayTimer = .5;
    //public double currentDisplayTimer = 0;

    //int cnter = 0;
    public override void _Process(double delta) {
        //GD.Print(TargetPos);
        if (RewindController.Instance.IsRewinding){
        	return;
        }
        if (!Active && !IsStunned)
        {
            Action next = PickOption();
            if (next != null)
            {
                //ShowOptions();

                //currentDisplayTimer = DisplayTimer;
                if (next is IAnimated)
                {
                    IAnimated i = (next as IAnimated);
                    i.Animation.PlayFuture(0, i.AnimName, i.ForesightOffset, transparency: (float)(next as IWeighted).GetWeight(this) / 100 * .5f, renderPriority: 1);
                }
                if (!IsLagging && !IsStunned)
                    (CurrentAction = next).CallAction();
            }
            
        }
        //if (!IsChannelling && !IsActing && currentDisplayTimer <= 0)
        //{
        //    //GD.Print(cnter++);
        //    currentDisplayTimer = DisplayTimer;
        //    ShowOptions();
        //}
        //else
        //    currentDisplayTimer -= delta * BulletTime.SpeedScale;
        //GD.Print(CurrentAction != null);

        //GD.Print(Options.Count);
        TargetPos.GlobalBasis = Basis.LookingAt(Player.Instance.GlobalPosition - TargetPos.GlobalPosition);

        base._Process(delta);
        VisualHP.Size = new Vector2(Stability * 500, 40);
    }

    public void ShowOptions()
    {
        List<IWeighted> Options = new List<IWeighted>();
        foreach (Option a in CurrentAction.ActionProperties.FollowUpOptions)
            foreach (Action b in a.GetActions())
                if (b is IWeighted && b is IAnimated)
                    Options.Add(b as IWeighted);
        
        int cnter = 1;
        Options.Sort((a, b) => (a.GetWeight(this)>b.GetWeight(this)?1:-1));
        List<VisualManager> vis = new List<VisualManager>();
        foreach (IWeighted option in Options)
        { 
            IAnimated i = (option as IAnimated);
            vis.Add(i.Animation.PlayFuture(0, i.AnimName, i.ForesightOffset, transparency:(float)(option as IWeighted).GetWeight(this) / 100 * .5f, renderPriority: cnter));
            cnter++;
            //GD.Print(option.Name);
            
        }
        //GD.Print(cnter);
        //foreach (VisualManager b in vis)
        //{
        //    GD.Print(b.Name + " " + b.ActionAnimator.CurrentAnimation);
        //}
    }


    public Action PickOption()
    {   
        
        Action next = null;

        if (CurrentAction.CurrentActionBufferTime > 0)
        {
            List<Action> Options = new List<Action>();
            foreach (Option a in CurrentAction.ActionProperties.FollowUpOptions)
                Options.AddRange(a.GetActions());
            if (Options.Count == 0)
            {
                //GD.Print(CurrentAction.GetType()+" = null ");
                return null;
            }
            double largestWeight = weightThreshold;
            foreach (Action option in Options)
            {
                if (option is IWeighted)
                {
                    double actionWeight = (option as IWeighted).GetWeight(this);
                    //if (option is IAnimated)
                    //    (option as IAnimated).Animation.PlayFuture(option.ActionProperties.ChannelTime);
                    if (!option.OnCD() && actionWeight > largestWeight)
                    {
                        largestWeight = actionWeight;
                        next = option;
                    }
                    //GD.Print(option.GetType()+" "+actionWeight);
                }
            }
        }
        else
        {
            next = CurrentAction.ActionProperties.DefaultFollowUp;
            next.Cooldown?.End(); // ends cooldown for default actions inorder to ensure its actually callable
        }
        if (next != null)
            GD.Print("next=" + next.Name);
        //else
        //    GD.Print("next=null");
        return next;
    }


    public new int DataLength{get{return base.DataLength+1;}}
    public override List<Object> GetData()
    {
        List<Object> data = base.GetData();
		data.AddRange(new List<Object>
        {
			CurrentAction,
        });
		return data;
    }
    public override void SetData(List<Object> data)
    {
        base.SetData(data);
        CurrentAction = data[base.DataLength+0] as Action;
    }
}

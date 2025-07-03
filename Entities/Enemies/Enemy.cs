using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Entity
{
    

    public override void _Process(double delta) {
        //GD.Print(TargetPos);
        if (RewindController.Instance.IsRewinding){
        	return;
        }
        if (!Active && !IsStunned)
            PickOption();
        //GD.Print(CurrentAction != null);

        //GD.Print(Options.Count);
        TargetPos.GlobalBasis = Basis.LookingAt(Player.Instance.GlobalPosition - TargetPos.GlobalPosition);

        base._Process(delta);
        VisualHP.Size = new Vector2(Stability * 500, 40);
    }
    public void PickOption()
    {   
        double largestWeight = -1;
        Action next = null;
        //GD.Print(CurrentAction.GetType()+" : ");
        if (CurrentAction is IWeighted == false)
        {
            //GD.Print("not valid? "+(CurrentAction as IWeighted));
            return;
        }
        List<Action> Options = new List<Action>();
        foreach (Option a in (CurrentAction as IWeighted).GetWeightManager().WeightProperties.FollowUpOptions)
            Options.AddRange(a.GetActions());
        if (Options.Count == 0){
            //GD.Print(CurrentAction.GetType()+" = null ");
            return;
        }
        foreach (Action option in Options)
        {
            if (option is IWeighted){
                    double actionWeight = (option as IWeighted).GetWeight();
                if (!option.OnCD() && actionWeight > largestWeight)
                {
                    largestWeight = actionWeight;
                    next = option;
                }
                //GD.Print(option.GetType()+" "+actionWeight);
            }
        }

        
        if (next != null)
            (CurrentAction = next).CallAction();
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

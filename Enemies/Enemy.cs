using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Entity
{ 
    [Export]
    public WeightedAction CurrentAction;
   
    public override void _Process(double delta) {
        base._Process(delta);

        //GD.Print(TargetPos);
        if (RewindController.Instance.IsRewinding){
        	return;
        }
        if (!Active)
            PickOption();
        //GD.Print(CurrentAction != null);
        
        //GD.Print(Options.Count);
    }
    public void PickOption()
    {   
        double largestWeight = 0;
        WeightedAction next = null;
        GD.Print(CurrentAction.GetType()+" : ");
        if (CurrentAction.FollowUpOptions == null){
            GD.Print(CurrentAction.GetType()+" = null ");
            return;
        }
        foreach (WeightedAction action in CurrentAction.FollowUpOptions)
        {
            double actionWeight = action.GetWeight();
            if (!action.OnCD() && actionWeight > largestWeight)
            {
                largestWeight = actionWeight;
                next = action;
            }
            GD.Print(action.GetType()+" "+actionWeight);
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
        CurrentAction = (WeightedAction) data[base.DataLength+0];
    }
}

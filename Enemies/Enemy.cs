using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Entity
{ 
    public List<WeightedAction> Options = new List<WeightedAction>();
    public WeightedAction CurrentAction;
   
    public override void _Process(double delta) {
        base._Process(delta);

        //GD.Print(TargetPos);
        if (RewindController.Instance.IsRewinding){
            
        	return;
        }
        if (!Channelling && !Acting)
            PickOption();
        //GD.Print(CurrentAction != null);
        
        //GD.Print(Options.Count);
    }
    public void PickOption()
    {   
        double largestWeight = 0;
        WeightedAction next = null;
        foreach (WeightedAction action in Options)
        {
            double actionWeight = action.GetWeight();
            if (!action.OnCD() && actionWeight > largestWeight)
            {
                largestWeight = actionWeight;
                next = action;
            }
            GD.Print(action.GetType()+" "+actionWeight);
        }
        CurrentAction = next;
        if (CurrentAction != null)
            CurrentAction.CallAction();
    }


    public new int DataLength{get{return base.DataLength+2;}}
    public override List<Object> GetData()
    {
        List<Object> data = base.GetData();
		data.AddRange(new List<Object>
        {
			CurrentAction,
            Options,
        });
		return data;
    }
    public override void SetData(List<Object> data)
    {
        base.SetData(data);
        CurrentAction = (WeightedAction) data[base.DataLength+0];
        Options = (List<WeightedAction>) data[base.DataLength+1];
    }
}

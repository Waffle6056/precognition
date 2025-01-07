using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Entity
{ 
    public List<WeightedWeapon> Options = new List<WeightedWeapon>();
    public WeightedWeapon CurrentAction;
   
    public override void _Process(double delta) {
        base._Process(delta);
        if (!Channelling && !Attacking)
            PickOption();
        GD.Print(CurrentAction != null);
        
        GD.Print(Options.Count);
    }
    public void PickOption()
    {   
        double largestWeight = 0;
        WeightedWeapon next = null;
        foreach (WeightedWeapon action in Options)
        {
            double actionWeight = action.GetWeight();
            if (!action.OnCD() && actionWeight > largestWeight)
            {
                largestWeight = actionWeight;
                next = action;
            }
            GD.Print(actionWeight);
        }
        CurrentAction = next;
        if (CurrentAction != null)
            CurrentAction.CallAttack();
    }
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
        CurrentAction = (WeightedWeapon) data[Entity.DataLength+0];
        Options = (List<WeightedWeapon>) data[Entity.DataLength+1];
    }
}

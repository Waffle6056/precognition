using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ActionGroup : Option
{
    public override void LoadActions(){
        List<Action> list = new List<Action>();
        foreach (Node child in RewindController.GetAllChildren(this)){
            if (child is Option)
                list.AddRange((child as Option).GetActions());
        }
        options = list.ToArray();
    }
}
public abstract partial class Option : Node3D
{
    public Action[] options;
    public virtual Action[] GetActions(){
        if (options == null)
            LoadActions();
        return options;
    }
    public abstract void LoadActions();
}

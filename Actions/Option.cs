using Godot;
using System;

[GlobalClass]
public partial class Option : Node3D
{
    public Action[] options;
    public virtual Action[] GetActions()
    {
        if (options == null)
            LoadActions();
        return options;
    }
    public virtual void LoadActions()
    {

    }
}

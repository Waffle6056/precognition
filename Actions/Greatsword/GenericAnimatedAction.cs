using Godot;
using System;
using System.Collections.Generic;

public partial class GenericAnimatedAction : Action, IAnimated
{


    public VisualManager Animation { get { return IAnimated.findAnimation(this); } }
    [Export]
    public String AnimName { get; set; } = "Swing";
    [Export]
    public Vector3 ForesightOffset { get; set; }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    protected override bool StartChannel()
    {
        if (!base.StartChannel())
            return false;
        Animation.Play(AnimName);
        return true;
    }
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
        return true;
    }
}


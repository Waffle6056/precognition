using Godot;
using System;
using System.Collections.Generic;

public partial class GenericAnimatedAction : Action, IAnimated
{


    [Export]
    public VisualManager Animation { get; set; }
    [Export]
    public bool playGhost = false;
    [Export]
    public String AttackName = "Swing";
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    protected override bool StartChannel()
    {
        if (!base.StartChannel())
            return false;
        Animation.Play(AttackName);
        if (playGhost)
            Animation.PlayFuture(ActionProperties.ChannelTime, AttackName);
        return true;
    }
}


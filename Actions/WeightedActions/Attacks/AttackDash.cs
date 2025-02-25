using Godot;
using System;

public partial class AttackDash : Dash, IAttack
{

    [Export]
    public AttackManager Attack{get;set;}
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
        Attack.Active = true;
        return true;
    }
    protected override bool EndAction()
    {
        if (!base.EndAction())
            return false;
        Attack.Active = false;
        return true;
    }
}

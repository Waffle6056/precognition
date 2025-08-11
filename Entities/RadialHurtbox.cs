using Godot;
using System;

[GlobalClass]
[Tool]
public partial class RadialHurtbox : Hurtbox
{
    public override void AttackHandler(Limb area)
    {
        Vector3 dir = (area.GlobalPosition - GlobalPosition).Normalized();
        GD.Print("Attack called " + (dir * ForceMagnitude));

        Parent.DealDamage(area.Parent, DamageCalc(area.Parent, dir));
        Parent.DealKnockback(area.Parent, dir * ForceMagnitude);
    }
}

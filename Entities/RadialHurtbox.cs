using Godot;
using System;

public partial class RadialHurtbox : Hurtbox
{
    public override void Collide(Limb area)
    {
        Vector3 dir = (area.GlobalPosition - GlobalPosition).Normalized();
        GD.Print("Attack called " + (dir * ForceMagnitude));
        area.Parent.TakeKnockback(dir * ForceMagnitude);
        v.HitStop(area.Parent.TakeHit(DamageCalc(area.Parent)) * HitstunMul);
    }
}

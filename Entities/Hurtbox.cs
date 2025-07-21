using Godot;
using System;
using System.Numerics;

[GlobalClass]
public partial class Hurtbox : Area3D
{
    [Export]
    float ForceMagnitude = 0;
    [Export]
    float HitstunMul = .005f;
    [Export]
    float Damage = 0;
    [Export]
    float CounterDamageMultipler = 1;
    [Export]
    public VisualManager v;
    public void Collide(Area3D area)
    {
        GD.Print("Collide called ");
        if (area is Limb)
        {
            GD.Print("Limb identified called ");
            Collide(area as Limb);
        }
    }
    public void Collide(Limb area)
    {
        GD.Print("Attack called "+(  -GlobalBasis[2] * ForceMagnitude));
        area.Parent.TakeKnockback(-GlobalBasis[2] * ForceMagnitude);
        v.HitStop(area.Parent.TakeHit(DamageCalc(area.Parent)) * HitstunMul);
    }
    public float DamageCalc(Entity entity)
    {
        float angle = (-GlobalBasis[2]).AngleTo(entity.Velocity);
        float angleFactor = (float)(angle / Math.PI);
        float counterDamage = angleFactor * entity.Velocity.Length() * CounterDamageMultipler;
        GD.Print(entity.Name + " Hit for " + Damage + " BaseDamage + " + counterDamage + " CounterDamage");
        return Damage + counterDamage;
    }
}

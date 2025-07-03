using Godot;
using System;

[GlobalClass]
public partial class Hurtbox : Area3D
{
    [Export]
    float ForceMagnitude = 1;
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
        GD.Print("Attack called "+(  GlobalBasis[2] * ForceMagnitude));
        area.Parent.TakeKnockback(GlobalBasis[2] * ForceMagnitude);
    }
}

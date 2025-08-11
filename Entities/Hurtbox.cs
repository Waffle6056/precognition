using Godot;
using System;

[GlobalClass]
[Tool]
public partial class Hurtbox : Area3D
{
	[Export]
	public float ForceMagnitude = 0;
	[Export]
	public float Damage = 0;
	[Export]
	public float CounterDamageMultipler = 1;
	public Entity Parent;
	public override void _Ready()
	{
		base._Ready();
		Parent = Misc.findParent(this);
	}
	//public override void _Process(double delta) { 
	//	GD.Print(GetSignalConnectionList(Hurtbox.SignalName.AreaEntered));
	//}
	public void Collide(Area3D area)
	{
		GD.Print("Collide called ");
		if (area is Limb)
		{
			GD.Print("Limb identified called ");
			AttackHandler(area as Limb);
		}
	}
	public virtual void AttackHandler(Limb area)
	{
		
		GD.Print("Attack called "+(  -GlobalBasis[2] * ForceMagnitude));
		Parent.DealDamage(area.Parent, DamageCalc(area.Parent, -GlobalBasis[2]));
        Parent.DealKnockback(area.Parent, -GlobalBasis[2] * ForceMagnitude);
    }
	
	public float DamageCalc(Entity entity, Vector3 Dir)
	{
		float angle = Dir.AngleTo(entity.Velocity);
		float angleFactor = (float)Math.Max(-Math.Cos(angle), 0);
		float counterDamage = angleFactor * entity.Velocity.Length() * CounterDamageMultipler;
		GD.Print(entity.Name + " Hit for " + Damage + " BaseDamage + " + counterDamage + " CounterDamage");
		return Damage + counterDamage;
	}
}

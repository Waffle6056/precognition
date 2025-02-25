using Godot;
using System;

[GlobalClass]
public partial class AttackManager : Node
{
	
	[Export]
	public float Damage{get;set;}
	[Export]
	public Area3D Hitbox;
	[Export]
	private bool active;
	public bool Active{
		get{return active;}
	 	set{Hitbox.Monitoring = active = value;}
	 }
	public virtual void DealDamage(Node3D body)
	{
		Entity entity = body as Entity;
		if (entity == null)
			return;
		//GD.Print(entity.Name+" took "+Damage+" damage");
		entity.TakeHit(Damage);
	}
}
interface IAttack
{
	[Export]
	public AttackManager Attack{get;set;}
}

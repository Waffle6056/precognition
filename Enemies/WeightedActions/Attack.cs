using Godot;
using System;
interface Attack
{
    [Export]
    public float Damage{get;set;}
    public virtual void DealDamage(Node3D body){
        Entity entity = body as Entity;
        if (entity == null)
            return;
        entity.TakeHit(Damage);
    }
}
public abstract partial class WeightedAttackAction : WeightedAction, Attack
{
    
    [Export]
    public float Damage{get;set;}
    public virtual void DealDamage(Node3D body){
        Entity entity = body as Entity;
        if (entity == null || entity == Root)
            return;
        entity.TakeHit(Damage);
    }
}
public abstract partial class AttackAction : Action, Attack
{
    
    [Export]
    public float Damage{get;set;}
    public virtual void DealDamage(Node3D body){
        Entity entity = body as Entity;
        if (entity == null || entity == Root)
            return;
        //GD.Print(entity.Name+" took "+Damage+" damage");
        entity.TakeHit(Damage);
    }
}

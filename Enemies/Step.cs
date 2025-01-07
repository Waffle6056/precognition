using Godot;
using System;

public partial class Step : WeightedWeapon
{
    [Export]
    public Vector3 Direction;
    protected override void StartAttack()
    {
        base.StartAttack();
        Root.TargetPos += Direction;
        EndAttack();
    }
    public override double GetWeight()
    {
        if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis,Root.TargetPos),Direction))
            return -1;
        
        return WeightMultiplier * (Root.TargetPos.DistanceTo(Player.Instance.TargetPos) - (Root.TargetPos+Direction).DistanceTo(Player.Instance.TargetPos));
        
    }

}

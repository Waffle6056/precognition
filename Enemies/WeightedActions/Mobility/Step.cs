using Godot;
using System;

public partial class Step : WeightedAction
{
    [Export]
    public Vector3 Direction;
    protected override void StartAction()
    {
        base.StartAction();
        Root.TargetPos += Direction;
        EndAction();
    }
    public override double GetWeight()
    {
        if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis,Root.TargetPos),Direction))
            return -1;
        
        return WeightMultiplier * ( PlayerDistance(Root.TargetPos) - PlayerDistance(Root.TargetPos+Direction));
        
    }

}

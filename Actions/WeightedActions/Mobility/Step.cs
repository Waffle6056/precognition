using Godot;
using System;

public partial class Step : Action, IWeighted
{
     [Export]
    public WeightProperties WeightProperties{get; set;}
    [Export]
    public Vector3 Direction;
    protected override bool StartAction()
    {
        if (!base.StartAction())
		 	return false;
        Properties.Root.TargetPos += Direction;
        EndAction();
        return true;
    }
    public double GetWeight()
    {
        if (Properties.Root.GridSpace.TestMove(new Transform3D(Properties.Root.GridSpace.GlobalBasis,Properties.Root.TargetPos),Direction))
            return -1;
        
        return WeightProperties.WeightMultiplier * ( IWeighted.PlayerDistance(Properties.Root.TargetPos) - IWeighted.PlayerDistance(Properties.Root.TargetPos+Direction));
        
    }

}

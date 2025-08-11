#if TOOLS
using Godot;
using System;

[Tool]
public partial class BindHurtbox : EditorScript
{
	// Called when the script is executed (using File -> Run in Script Editor).
	public override void _Run()
	{
		GD.Print("signal passs");
		travSignal(GetScene());
		GD.Print("mask passs");
		travMask(GetScene(), 2);
		//GD.Print("a");
	}
	void travSignal(Node n)
	{
		if (n == null)
			return;
		//GD.Print("trav on " + n.Name+" "+n.GetType()+" "+n.GetScript().GetType());
		if (n is Hurtbox)
		{
			Hurtbox b = (Hurtbox)n;
			GD.Print("hitbox on " + b.Name);
			if (!b.IsConnected(Hurtbox.SignalName.AreaEntered, new Callable(b, Hurtbox.MethodName.Collide)))
			{
				GD.Print("Called on "+b.Name);
				b.Connect(Hurtbox.SignalName.AreaEntered, new Callable(b, Hurtbox.MethodName.Collide), (uint)Hurtbox.ConnectFlags.Persist);
			}
		}
		foreach (Node c in n.GetChildren())
		{
			travSignal(c);
		}
	}
	void travMask(Node n, int mask)
	{
		if (n == null)
			return;
		//GD.Print("trav on " + n.Name+" "+n.GetType()+" "+n.GetScript().GetType());
		if (n is Hurtbox)
		{
			Hurtbox b = (Hurtbox)n;
			GD.Print("hitbox on " + b.Name);
			for (int i = 1; i <= 32; i++)
				b.SetCollisionLayerValue(i, false);
			for (int i = 1; i <= 32; i++)
				b.SetCollisionMaskValue(i, false);
			b.SetCollisionMaskValue(mask, true);
			//GD.Print(b.GetCollision);
		}
		foreach (Node c in n.GetChildren())
		{
			travMask(c, mask);
		}
	}
}
#endif

#if TOOLS
using System.Collections.Generic;
using Godot;

[Tool]
public partial class SetAllRewindableRewindableGroup : EditorScript
{
	public Godot.Collections.Array<Node> GetAllChildren(Node n){
		Godot.Collections.Array<Node> children = n.GetChildren();
		foreach (Node next in n.GetChildren())
			children.AddRange(GetAllChildren(next));
		return children;
	}
	// Called when the script is executed (using File -> Run in Script Editor).
	public override void _Run()
	{
		foreach (Node n in GetAllChildren(GetScene())){
			GD.Print(n.GetType()+" "+(n is VisualManager)+" "+n.GetScript());
			if (n is RewindableObject && !n.IsInGroup("Rewindable"))
				n.AddToGroup("Rewindable", true);
		}
		GD.Print();
	}
}
#endif

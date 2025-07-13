using Godot;
using System;

public partial class Charge : GenericAnimatedAction
{
	[Export]
	public String[] ChargeInputs { get; set; }
	protected override bool Act(double delta)
	{
		if (!base.Act(delta))
			return false;

		bool act = false;
		foreach (String input in ChargeInputs) {
			if (Input.IsActionPressed(input))
				act = true;
        }
		if (!act)
			return EndAction();

		return true;
	}
}

using Godot;
using System;

public partial class Charge : GenericAnimatedAction
{
	[Export]
	public String[] ChargeInputs { get; set; }
	protected override bool Channel(double delta)
	{
		if (!base.Channel(delta))
			return false;

		bool act = false;
		foreach (String input in ChargeInputs) {
			if (Input.IsActionPressed(input))
				act = true;
        }
		if (!act)
			return EndChannel();

		return true;
	}
}

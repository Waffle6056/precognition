using Godot;
using System;

public partial class TimelineButton : TextureButton
{
	public SavedNode Timeline;
	[Export]
    public Sprite2D Preview;
	public void OnPressed(){
		GD.Print("Pressed");
		RewindController.Instance.PlayFrame(Timeline);
		RewindController.Instance.PreviousFrame = Timeline;
		RewindController.Instance.Time = Timeline.CallTime;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Preview.Texture = Timeline?.Preview;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}

using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class MouseDirection : Pivot
{
	public override void UpdateTarget(){
		Viewport vp = GetViewport();
		Vector2 mousePos = vp.GetMousePosition();
		mousePos -= vp.GetVisibleRect().Size / 2;
		Target = new Vector3(mousePos.X,0,mousePos.Y);
	}
}

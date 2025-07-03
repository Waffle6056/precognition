using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class CameraPivot : Pivot
{

	public override void UpdateTarget(){
		Viewport vp = GetViewport();
		Vector2 mousePos = vp.GetMousePosition();
		mousePos -= vp.GetVisibleRect().Size / 2; 
        Vector3 nTarget = new Vector3(mousePos.X, 0, mousePos.Y);
        if (!nTarget.IsEqualApprox(Vector3.Zero))
        {
            Target = nTarget;
        }
    }
}

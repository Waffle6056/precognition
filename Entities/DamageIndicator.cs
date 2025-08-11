using Godot;
using System;

public partial class DamageIndicator : GpuParticles3D
{
    float sum = 0;
    public void EmitDamage(float Amt)
    {
        sum += Amt;
        (DrawPass1 as TextMesh).Text = sum + "";
        Restart();
    }
    public void resetCount()
    {
        sum = 0;
    }
}

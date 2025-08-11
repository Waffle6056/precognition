using Godot;
using System;

public static class Misc
{
    public static Entity findParent(Node n)// sets parent to first entity above it
    {
        Node t = n.GetParent();
        while (t != null && !(t is Entity))
            t = t.GetParent();
        if (t != null)
            return t as Entity;
        else
            return null;
    }
}

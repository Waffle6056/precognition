using Godot;
using System;
using System.Collections.Generic;

public partial class RewindGUI : Control
{
    [Export]
    public PackedScene TimelineButtonScene;
    [Export]
    public PackedScene LineScene;
    [Export]
    public float XScale = 100f;
    [Export]
    public float Height = 648;
    [Export]
    public Vector2 ButtonOffset = new Vector2(-64,-64);
    
    
    public List<List<TimelineButton>> ButtonDepth = new List<List<TimelineButton>>();
    public List<Line2D> Lines = new List<Line2D>();
    int leafs = 0;
    public void Traverse(SavedNode root, int depth)
    {
        TimelineButton b = TimelineButtonScene.Instantiate<TimelineButton>();
        b.Timeline = root;
        b.Position = new Vector2((float)(root.CallTime - RewindController.Instance.Root.CallTime) * XScale, 0);
        while (ButtonDepth.Count <= depth)
            ButtonDepth.Add(new List<TimelineButton>());
        ButtonDepth[depth].Add(b);
        AddChild(b);


        foreach (SavedNode child in root.Children)
            Traverse(child, depth+1);

        if (root.Children.Count == 0)
            leafs++;
    }

    public void InstantiateCurrentSavedPoints()
    {
        ButtonDepth = new List<List<TimelineButton>>();
        leafs = 0;
        Traverse(RewindController.Instance.Root, 0);
    }

    public void SetPointYs(){
        int usedLeafs = 0;
        for (int i = ButtonDepth.Count-1; i >= 0; i--){
            int childrenInd = 0;
            for (int j = 0; j < ButtonDepth[i].Count; j++){
                TimelineButton b = ButtonDepth[i][j];
                int childCount = b.Timeline.Children.Count;

                if (childCount == 0){
                    usedLeafs++;
                    b.Position = b.Position with {Y = Height / (leafs+1) * usedLeafs};
                    continue;
                }


                float firstChildY = ButtonDepth[i+1][childrenInd].Position.Y;
                float lastChildY = ButtonDepth[i+1][childrenInd+childCount-1].Position.Y;
                float space = lastChildY - firstChildY;

                b.Position = b.Position with {Y = space / 2 + firstChildY};

                childrenInd += childCount;
            }
        }
    }

    public void DrawLines()
    {
        Lines = new List<Line2D>();
        for (int i = ButtonDepth.Count-2; i >= 0; i--){
            int childrenInd = 0;
            for (int j = 0; j < ButtonDepth[i].Count; j++){
                TimelineButton b = ButtonDepth[i][j];
                int childCount = b.Timeline.Children.Count;

                if (childCount == 0)
                    continue;
                
                for (int c = childrenInd; c < childrenInd+childCount; c++){
                    Line2D l = LineScene.Instantiate<Line2D>();
                    l.AddPoint(b.Position);
                    l.AddPoint((b.Position + ButtonDepth[i+1][c].Position) / 2);
                    l.AddPoint(ButtonDepth[i+1][c].Position);
                    AddChild(l);
                    Lines.Add(l);
                }

                childrenInd += childCount;
            }
        }
    }

    public void ApplyButtonOffset(){
        foreach (List<TimelineButton> level in ButtonDepth)
            foreach (TimelineButton b in level)
                b.Position += ButtonOffset;
    }

    public void OpenGUI()
    {
        RewindController.Instance.pauseOutside++;
        Position = Vector2.Zero;
        
        InstantiateCurrentSavedPoints();
        SetPointYs();
        DrawLines();
        ApplyButtonOffset();
    }

    public void CloseGUI()
    {
        RewindController.Instance.pauseOutside--;
        foreach (List<TimelineButton> level in ButtonDepth)
            foreach (TimelineButton b in level)
                RemoveChild(b);
        
        foreach (Line2D l in Lines)
            RemoveChild(l);
        Visible = false;
    }
    
    public Vector2 mouseOffset = Vector2.Zero;
    public override void _Process(double delta) 
    {
        if (!RewindController.Instance.IsPredicting && Visible)
            CloseGUI();
        if (RewindController.Instance.IsPredicting && Input.IsActionJustPressed("TimelineGUI"))
        {
            Visible = !Visible;
            if (Visible)
                OpenGUI();
            else
                CloseGUI();
            
        }


        if (Visible && Input.IsActionJustPressed("RightClick"))
            mouseOffset = GlobalPosition - GetGlobalMousePosition();
        
        if (Visible && Input.IsActionPressed("RightClick"))
            Position = GetGlobalMousePosition() + mouseOffset;

        
    }
    public override void _Ready()
    {
        Visible = false;
    }
}

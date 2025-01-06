using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;




public partial class RewindController : Node
{
    
    public override void _PhysicsProcess(double delta) 
    {
        base._PhysicsProcess(delta);

        if (!IsPaused && IsPredicting)
            StoreFrame(false);
        
    }

    public override void _Process(double delta) 
    {
        base._Process(delta);
        
        
        if (Input.IsActionJustPressed("Predict"))
        {
            IsPredicting = !IsPredicting;
            if (IsPredicting)
                StartPredict();
            else 
                EndPredict();
        }

        if (Input.IsActionJustPressed("SaveFrame") && IsPredicting)
            SaveFrame();


        if (Input.IsActionJustPressed("Rewind") && IsPredicting)
            StartRewind();

        if (Input.IsActionJustReleased("Rewind"))
            EndRewind();
        
        
        if (Input.IsActionJustPressed("Pause") && IsPredicting)
            pauseManual = !pauseManual;
        IsPaused = pauseManual || IsRewinding || pauseOutside > 0;
        
        if (!IsPaused)
            Time += delta;


        if (IsRewinding)
            Rewind(delta);
        if (PreviousFrame == null)
            EndRewind();
        //GD.Print(Rewinding);
    }
    
    public double Focus = 100.0;
    
    public double FocusMax = 100.0;
    public double FocusThreshold = 50.0;
    public double RewindSpeed = 3.0;
    public double RewindSpeedBase = 3.0;
    public double RewindSpeedAcceleration = 9.0;
    public double RewindSpeedAccelerationBase = 9.0;
    public double RewindSpeedAccelerationAcceleration = 9.0;
    public bool IsRewinding = false;
    public bool IsPredicting = false;
    public bool IsPaused = false;
    private bool pauseManual = false;
    public byte pauseOutside = 0;
    public RewindNode PreviousFrame = null;
    public SavedNode LastSavePoint = null;
    public SavedNode Root = null;
    public double Time = 0.0;
    //LinkedList<List<NodeData>> RedoData = new LinkedList<List<NodeData>>();


    public void StoreFrame(bool isSavePoint)
    {
        Array<Node> rewindableGroup = GetTree().GetNodesInGroup("Rewindable");
        List<NodeData> frameData = new List<NodeData>();

        foreach (Node n in rewindableGroup)
        {
            RewindableObject r = n as RewindableObject;
            if (n is RewindableObject)
                frameData.Add(new NodeData(r));
        }

        if (isSavePoint)
            PreviousFrame = new SavedNode(frameData, Time, PreviousFrame, LastSavePoint, ImageTexture.CreateFromImage(GetViewport().GetTexture().GetImage()));
        else
            PreviousFrame = new RewindNode(frameData, Time, PreviousFrame);

    }

    public void SaveFrame()
    {
        StoreFrame(true);
        SavedNode SavedFrame = PreviousFrame as SavedNode;
        
        LastSavePoint?.Children.Add(SavedFrame);
        LastSavePoint = SavedFrame;
        GD.Print("Save");
    }

    public void PlayLast()
    {
        PlayFrame(PreviousFrame);
    }

    public void PlayFrame(RewindNode frame)
    {
        if (frame == null) 
            return; 

        List<NodeData> frameData = frame.FrameData;
        foreach (NodeData n in frameData)
            n.Node.SetData(n.Data);
        
        //RedoData.AddLast(FrameData);
        if (frame is SavedNode)
            LastSavePoint = frame as SavedNode;

    }


    public bool Rewind(double delta)
    {
        if (PreviousFrame == null)
            return false;
        

        RewindSpeedAcceleration += RewindSpeedAccelerationAcceleration * delta;
        RewindSpeed += RewindSpeedAcceleration * delta;

        Time -= delta * RewindSpeed;
        Time = Math.Max(Time, Root.CallTime);

        while (PreviousFrame.Root != null && PreviousFrame.CallTime > Time && !(PreviousFrame is SavedNode))
            PreviousFrame = PreviousFrame.Root;
        
        PlayLast();

        
        if (!IsPredicting || PreviousFrame.Root != null){
            if (PreviousFrame == LastSavePoint)
                LastSavePoint = LastSavePoint.SavedRoot;
            PreviousFrame = PreviousFrame.Root;
            return true;
        }
        else
            return false;
        
    }

    public void StartRewind()
    {
        RewindSpeed = RewindSpeedBase;
        RewindSpeedAcceleration = RewindSpeedAccelerationBase;
        IsRewinding = true;
    }

    public void EndRewind()
    {
        IsRewinding = false;
    }

    public void StartPredict()
    {
        PreviousFrame = null;
        LastSavePoint = null;
        SaveFrame();
        SavedNode SavedFrame = PreviousFrame as SavedNode;
        Root = SavedFrame;
    }

    public void EndPredict()
    {
        pauseManual = false;
        StartRewind();
    }


    public static RewindController Instance {get; private set;}
    public override void _Ready() 
    {
        Instance = this;
        ProcessMode = Node.ProcessModeEnum.Always;
    }
}

public class RewindNode
{
    public List<NodeData> FrameData;
    public double CallTime;
    public RewindNode Root;
    public RewindNode(List<NodeData> frameData, double callTime, RewindNode root)
    {
        this.FrameData = frameData;
        this.CallTime = callTime;
        this.Root = root;
    }
    
}

public class SavedNode : RewindNode
{
    public List<RewindNode> Children = new List<RewindNode>();
    public SavedNode SavedRoot;
    public ImageTexture Preview;
    public SavedNode(List<NodeData> frameData, double callTime, RewindNode root, SavedNode savedRoot, ImageTexture preview) : base(frameData, callTime, root)
    {
        this.Preview = preview;
        this.SavedRoot = savedRoot;
    }
}


public class NodeData
{
    public RewindableObject Node;
    public List<Object> Data;
    public NodeData(RewindableObject Node)
    {
        this.Node = Node;
        this.Data = Node.GetData();
    }
}

public interface RewindableObject
{
    List<Object> GetData();
    void SetData(List<Object> data);
}
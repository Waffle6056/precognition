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

        if (!IsPaused)
            Past.AddLast(StoreState());
        while (Past.Count > 0 && Time-Past.First.Value.CallTime > RewindTimeLimit)
            Past.RemoveFirst();
        //GD.Print(Fated.Count);
        
    }

    public override void _Process(double delta) 
    {
        base._Process(delta);
        
        PredictionCooldownCurrent -= delta;
        //GD.Print(PredictionCooldownCurrent + " "+ (PredictionCooldownCurrent <= 0));
        if (Input.IsActionJustPressed("Predict") && PredictionCooldownCurrent <= 0)
        {
            IsPredicting = !IsPredicting;
            if (IsPredicting)
                StartPredict();
            else 
                EndPredict();
        }
        
        RewindCooldownCurrent -= delta;
        if (Input.IsActionJustPressed("Rewind") && RewindCooldownCurrent <= 0)
            StartRewind();

        if (Input.IsActionJustReleased("Rewind"))
            EndRewind();
        
        
        // if (Input.IsActionJustPressed("Pause") && IsPredicting)
        //     pauseManual = !pauseManual;
        IsFateing = Fated.Count > 0;
        IsPaused = pauseManual || pauseOutside > 0 || IsRewinding || IsFateing;
        
        if (!IsPaused)
            Time += delta;

        if (IsPredicting && !IsRewinding && Time-PredictionCallTime > PredictionTimeLimit)
            EndPredict();

        if (IsRewinding)
            Rewind(delta);
        if (IsPredicting)
            Predict(delta);
        if (IsFateing)
            Fate(delta);
        if (Past.Count <= 0)
            EndRewind();
        //GD.Print(Rewinding);
    }
    public LinkedListNode<StateNode> PredictionStartNode = null;
    public LinkedList<StateNode> Past = new LinkedList<StateNode>();
    public LinkedList<StateNode> Fated = new LinkedList<StateNode>();
    public double PredictionTimeLimit = 3.0; // seconds
    
    public double PredictionCallTime = 0.0;

    public double PredictionCooldownBase = 0.0;
    public double PredictionCooldownCurrent = 0.0;
    public double RewindTimeLimit = 10.0;
    public double RewindCooldownBase = 0.0;
    public double RewindCooldownCurrent = 0.0;
    public double RewindSpeed = 9.0;
    public double RewindSpeedBase = 9.0;
    public double RewindSpeedAcceleration = 9.0;
    public double RewindSpeedAccelerationBase = 9.0;
    public double RewindSpeedAccelerationAcceleration = 9.0;
    public bool IsRewinding = false;
    public bool IsFateing = false;
    public bool IsPredicting = false;
    public bool IsPaused = false;
    public bool SaveFated = false;
    private bool pauseManual = false;
    public byte pauseOutside = 0;
    
    public double Time = 0.0;
    //LinkedList<List<NodeData>> RedoData = new LinkedList<List<NodeData>>();


    public StateNode StoreState()
    {
        Array<Node> rewindableGroup = GetTree().GetNodesInGroup("Rewindable");
        System.Collections.Generic.Dictionary<RewindableObject, NodeData> frameData = new System.Collections.Generic.Dictionary<RewindableObject, NodeData>();

        foreach (Node n in rewindableGroup)
        {
            RewindableObject r = n as RewindableObject;
            if (n is RewindableObject)
                frameData.Add(r,new NodeData(r));
            else
                GD.Print("Save error, "+n.Name+" doesn't extend RewindableObject");
            // if (r is Enemy)
            //     GD.Print(r.GetData().Count);
        }

        return new StateNode(frameData, Time);

    }

    public void PlayLast()
    {
        PlayState(Past.Last.Value);
    }

    public void PlayState(StateNode frame)
    {
        if (frame == null) 
            return; 

        System.Collections.Generic.Dictionary<RewindableObject, NodeData> frameData = frame.StateData;
        foreach (System.Collections.Generic.KeyValuePair<RewindableObject, NodeData> n in frameData)
            n.Key.SetData(n.Value.Data);
        
        //RedoData.AddLast(FrameData);

    }


    public void Rewind(double delta)
    {
        if (Past.Count <= 0)
            EndRewind();

        RewindSpeedAcceleration += RewindSpeedAccelerationAcceleration * delta;
        RewindSpeed += RewindSpeedAcceleration * delta;

        Time -= delta * RewindSpeed;

        LinkedListNode<StateNode> Last = Past.Last;
        
        while (Last.Previous != null && Last.Value.CallTime > Time && Last != PredictionStartNode){
            
            LinkedListNode<StateNode> Prev = Last.Previous;
            Past.Remove(Last);
            if (SaveFated)
                Fated.AddFirst(Last);
            Last = Prev;
        }

        PlayState(Last.Value);

        if (Last.Value.CallTime > Time || Last == PredictionStartNode){
            EndRewind();
            Past.Remove(Last);
        }
    }

    public void StartRewind()
    {
        Fated = new LinkedList<StateNode>();
        RewindSpeed = RewindSpeedBase;
        RewindSpeedAcceleration = RewindSpeedAccelerationBase;
        IsRewinding = true;
    }

    public void EndRewind()
    {
        RewindCooldownCurrent = PredictionCooldownBase;
        IsRewinding = false;
        SaveFated = false;
    }

    public void StartPredict()
    {
        Fated = new LinkedList<StateNode>();
        PredictionCallTime = Time;
        PredictionStartNode = Past.AddLast(StoreState());
        RewindTimeLimit += PredictionTimeLimit;
        IsPredicting = true;
    }
    public void Predict(double delta)
    {
        
    }
    public void EndPredict()
    {
        IsPredicting = false;
        pauseManual = false;
        RewindTimeLimit -= PredictionTimeLimit;
        PredictionCooldownCurrent = PredictionCooldownBase;
        SaveFated = true;
        StartRewind();
        
    }
    public void Fate(double delta)
    {
        Time += delta;

        LinkedListNode<StateNode> Current = Fated.First;
        
        while (Current.Next != null && Current.Value.CallTime < Time){
            LinkedListNode<StateNode> Next = Current.Next;
            Fated.Remove(Current);
            Current = Next;
        }
        
        PlayState(Current.Value);

        if (Current.Value.CallTime < Time){
            Fated.Remove(Current);
        }
    }

    public static RewindController Instance {get; private set;}
    public static Godot.Collections.Array<Node> GetAllChildren(Node n){
		Godot.Collections.Array<Node> children = n.GetChildren(true);
		foreach (Node next in n.GetChildren(true))
			children.AddRange(GetAllChildren(next));
		return children;
	}
    public override void _Ready() 
    {
        Instance = this;
        ProcessMode = Node.ProcessModeEnum.Always;
        foreach (Node n in GetAllChildren(GetTree().Root)){
			//GD.Print(n.GetType()+" "+(n is AnimationManager)+" "+n.GetScript());
			if (n is RewindableObject && !n.IsInGroup("Rewindable"))
				n.AddToGroup("Rewindable", true);
		}
		GD.Print();
    }
}

public class StateNode
{
    public System.Collections.Generic.Dictionary<RewindableObject, NodeData> StateData;
    public double CallTime;
    public StateNode(System.Collections.Generic.Dictionary<RewindableObject, NodeData> frameData, double callTime)
    {
        this.StateData = frameData;
        this.CallTime = callTime;
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
    protected int DataLength{get;}
    public List<Object> GetData();
    public void SetData(List<Object> data);
}
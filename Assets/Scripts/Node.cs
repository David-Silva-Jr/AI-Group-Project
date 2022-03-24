using System.Collections;
using System.Collections.Generic;

public class Node
{
    private static int nodeCounter = 0; // How many nodes have been created?
    private int nodeID;                 // Node ID, to be used for checking if two nodes are the same
    private List<Node> adjacentNodes;   // List of adjacent nodes
    private int resources;              // Value of the resource on this node. Negative values are dropoff, positive are pickup
    private bool occupied;              // Is this node occupied?

    public Node(int _resources, ref Node _parent){        
        adjacentNodes = new List<Node>();
        adjacentNodes.Add(_parent);
        _parent.AdjacentNodes.Add(this);

        nodeID = nodeCounter;
        nodeCounter++; // Because this is static, every node will have an ID 1 higher than the last

        resources = _resources;
        occupied = false;
    }

    public Node(int _resources){        
        adjacentNodes = new List<Node>();

        nodeID = nodeCounter;
        nodeCounter++; // Because this is static, every node will have an ID 1 higher than the last

        resources = _resources;
        occupied = false;
    }

    public List<Node> AdjacentNodes{
        get {return adjacentNodes;}
    }

    public int Resources{
        get {return resources;}
        set {resources = value;}
    }

    public bool Occupied{
        get {return occupied;}
        set {occupied = value;}
    }

    public bool IsPickup{
        get {return resources > 0;}
    }

    public bool IsDropoff{
        get {return resources < 0;}
    }

    public bool IsPath{
        get {return resources == 0;}
    }

    public int ID{
        get {return nodeID;}
    }
}

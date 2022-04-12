using System.Collections;
using System.Collections.Generic;

public class Tile
{
    private static int tileCounter = 0; // How many tiles have been created?
    private int tileID;                 // Tile ID, to be used for checking if two tiles are the same
    private List<Tile> adjacentTiles;   // List of adjacent tiles
    private int resources;              // Value of the resource on this tile. Negative values are dropoff, positive are pickup
    private bool occupied;              // Is this tile occupied?

    public Tile(int _resources, ref Tile _parent){        
        adjacentTiles = new List<Tile>();
        adjacentTiles.Add(_parent);
        _parent.AdjacentTiles.Add(this);

        tileID = tileCounter;
        tileCounter++; // Because this is static, every tile will have an ID 1 higher than the last

        resources = _resources;
        occupied = false;
    }

    public Tile(int _resources){        
        adjacentTiles = new List<Tile>();

        tileID = tileCounter;
        tileCounter++; // Because this is static, every tile will have an ID 1 higher than the last

        resources = _resources;
        occupied = false;
    }

    public List<Tile> AdjacentTiles{
        get {return adjacentTiles;}
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
        get {return tileID;}
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

public class Tile
{
    private int resources;              // Value of the resource on this tile. Negative values are dropoff, positive are pickup
    private bool occupied;              // Is this tile occupied?

    public event EventHandler ResourceChanged;

    public Tile(int _resources){
        resources = _resources;
        occupied = false;
    }

    public int Resources{
        get {return resources;}
        set {
            resources = value;
            ResourceChanged?.Invoke(this, EventArgs.Empty);
        }
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
}

using System.Collections;
using System.Collections.Generic;

// May want to move this to RLFrameworks
// Need some way to check equality between states (two states with the same agent and the same tile should be the same)
public class State
{
    private DAgent agent;   // The agent this state is relevant to
    private Tile tile;      // Tile this state concerns (not always the same as agent's location)
    private float utility;  // Utility of the state

    public State(DAgent _agent, Tile _tile, float _utility){
        agent = _agent;
        tile = _tile;
        utility = _utility;
    }

    public State(DAgent _agent, Tile _tile){
        agent = _agent;
        tile = _tile;
        utility = 0;
    }

    public State(DAgent _agent){
        agent = _agent;
        tile = agent.Location;
        utility = 0;
    }

    public State(DAgent _agent, float _utility){
        agent = _agent;
        tile = agent.Location;
        utility = 0;
    }

    public float Value{
        get {return utility;}
        set {utility = value;}
    }

    // These are read only because you shouldn't be assigning anything but value directly to state
    public bool AgentHasCargo{
        get {return agent.HasCargo;}
    }

    public Tile CurrentTile{
        get {return tile;}
    }

    public bool IsPickup{
        get {return tile.IsPickup;}
    }

    public bool IsDropoff{
        get {return tile.IsDropoff;}
    }
}

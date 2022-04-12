using System.Collections;
using System.Collections.Generic;

// This file should store everything that would be common between all the reinforcement learning paradigms we're using


public class QTable{
    Dictionary<string, float> table;


    public float this[string s]{
        get{return table[s];}
    }
}

public static class RLFramework
{
    
    // Returns the reward of a given action given a certain state
    public static int GetReward(State _state, string _action){
        if(_state.IsPickup && _action == "pickup" && !_state.AgentHasCargo){
            return 5;
        }

        if(_state.IsDropoff && _action == "dropoff" && _state.AgentHasCargo){
            return 20;
        }

        if(_action == "move"){
            return -1;
        }

        return 0;
    }
}

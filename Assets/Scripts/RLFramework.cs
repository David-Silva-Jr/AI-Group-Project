using System.Collections;
using System.Collections.Generic;

// This class should store everything that would be common between all the reinforcement learning paradigms we're using

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

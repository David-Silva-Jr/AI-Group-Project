using System.Collections;
using System.Collections.Generic;

// This file stores some utility functions that would be useful to any RL Paradigm
// Operators: N, E, S, W, P, D
public static class RLFramework
{
    public static string StateToString(int i, int j, int dm, bool x, bool s, bool t, bool u, bool v){
        string out_str = "";

        out_str += i + " ";
        out_str += j + " ";
        out_str += dm + " ";
        out_str += (x ? "1" : "0") + " ";
        out_str += (s ? "1" : "0") + " ";
        out_str += (t ? "1" : "0") + " ";
        out_str += (u ? "1" : "0") + " ";
        out_str += (v ? "1" : "0");

        return out_str;
    }


    // Returns the state as a type so data can be more easily read
    public static State StringToState(string state_as_string){
        return new State(state_as_string);
    }

    // Returns the reward of a given action
    // bases reward on the difference between old and new states
    public static int GetReward(string oldState, string newState){
        State state_old = StringToState(oldState);
        State state_new = StringToState(newState);

        // If agent picks up cargo
        if(state_new.HasCargo && !state_old.HasCargo){
            return 5;
        }

        // If agent drops off cargo
        if(!state_new.HasCargo && state_old.HasCargo){
            return 20;
        }

        // If agent moves at all
        if((state_new.I - state_old.I) + (state_new.J - state_old.J) != 0){
            return -1;
        }

        return 0;
    }

    // Returns the reward of a given action given a certain state
    // public static int GetReward(Map _map, string _state_as_string,  char _action){
    //     List<string> vals = new List<string>(_state_as_string.Split(' '));
    //     int i = int.Parse(vals[0]);
    //     int j = int.Parse(vals[1]);
    //     int x = int.Parse(vals[3]);

    //     Tile location = _map[i, j];

    //     if(location.IsPickup && _action == 'p' && x == 0){
    //         return 10;
    //     }

    //     if(location.IsDropoff && _action == 'd' && x == 1){
    //         return 50;
    //     }

    //     if(_action == 'n' || _action == 'e' || _action == 's' || _action == 'w'){
    //         return -1;
    //     }

    //     return 0;
    // }
}

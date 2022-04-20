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

    public static List<int> StringStateToList(string state_as_string){
        return new State(state_as_string).As_List();
    }

    // Returns the state as a type so data can be more easily read
    public static State StringToState(string state_as_string){
        return new State(state_as_string);
    }

    public static List<char> GetPossibleOperators(Map world, string state_as_string){
        State data = new State(state_as_string);
        List<char> ops = new List<char>();

        if(data.I > 0){
            ops.Add('n');
        }

        if(data.J < world.Width - 1){
            ops.Add('e');
        }

        if(data.I < world.Height - 1){
            ops.Add('s');
        }

        if(data.J > 0){
            ops.Add('w');
        }

        if(world[data.I, data.J].IsPickup){
            ops.Add('p');
        }

        if(world[data.I, data.J].IsDropoff){
            ops.Add('d');
        }

        return ops;
    }

    public static List<char> GetPossibleMovementOperators(Map world, int r, int c){
        List<char> ops = new List<char>();

        if(r > 0){
            ops.Add('n');
        }

        if(c < world.Width - 1){
            ops.Add('e');
        }

        if(r < world.Height - 1){
            ops.Add('s');
        }

        if(c > 0){
            ops.Add('w');
        }

        return ops;
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
}

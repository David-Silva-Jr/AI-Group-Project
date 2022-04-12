using System.Collections;
using System.Collections.Generic;

// May want to move this to RLFrameworks
// Need some way to check equality between states (two states with the same agent and the same tile should be the same)
public class Revised_State
{
    //     State string format: "i j Dm x s t u v"
    // note that values are separated by spaces
    int pos_x;              // 'i'
    int pos_y;              // 'j'
    int manhattan_distance; // 'Dm' Manhattan distance between agents
    bool hasCargo;          // 'x' Is the agent holding anything?
    bool p1;                // 's' Is pickup location 1 active?
    bool p2;                // 't'
    bool d1;                // 'u' Is dropoff location 1 active?
    bool d2;                // 'v'

    public State(){
        pos_x = 0;
        pos_y = 0;
        manhattan_distance = 0;
        hasCargo = false;
        p1 = false;
        p2 = false;
        d1 = false;
        d2 = false;
    }

    public State(string state_as_string){
        List<string> vals = state_as_string.Split(' ');

        pos_x = int.Parse(vals[0]);
        pos_y = int.Parse(vals[1]);
        manhattan_distance = int.Parse(vals[2]);
        hasCargo = bool.Parse(vals[3]);
        p1 = bool.Parse(vals[4]);
        p2 = bool.Parse(vals[5]);
        d1 = bool.Parse(vals[6]);
        d2 = bool.Parse(vals[7]);
    }

    public string AsString(){
        return pos_x + " " 
            + pos_y + " " 
            + manhattan_distance + " " 
            + (int)hasCargo + " " 
            + (int)p1 + " " 
            + (int)p2 + " " 
            + (int)d1 + " " 
            + (int)d2;
    }
}

using System.Collections;
using System.Collections.Generic;

// May want to move this to RLFrameworks
// Really ony used to make reading state data easier
public class State
{
    //     State string format: "i j Dm x s t u v"
    // note that values are separated by spaces
    private int pos_i;              // 'i'
    private int pos_j;              // 'j'
    private int manhattan_distance; // 'Dm' Manhattan distance between agents
    private bool hasCargo;          // 'x' Is the agent holding anything?
    private bool p1;                // 's' Is pickup location 1 active?
    private bool p2;                // 't'
    private bool d1;                // 'u' Is dropoff location 1 active?
    private bool d2;                // 'v'

    public State(){
        pos_i = 0;
        pos_j = 0;
        manhattan_distance = 0;
        hasCargo = false;
        p1 = false;
        p2 = false;
        d1 = false;
        d2 = false;
    }

    // Essentially just read string and convert to a state object
    public State(string state_as_string){
        List<string> vals = new List<string>(state_as_string.Split(' '));

        pos_i = int.Parse(vals[0]);
        pos_j = int.Parse(vals[1]);
        manhattan_distance = int.Parse(vals[2]);
        hasCargo = vals[3] == "1" ? true : false;
        p1 = vals[4] == "1" ? true : false;
        p2 = vals[5] == "1" ? true : false;
        d1 = vals[6] == "1" ? true : false;
        d2 = vals[7] == "1" ? true : false;
    }

    // Accessors
    public int I{
        get{return pos_i;}
    }

    public int J{
        get{return pos_j;}
    }

    public int Manhattan_Distance{
        get{return manhattan_distance;}
    }

    public bool HasCargo{
        get{return hasCargo;}
    }

    public bool S{
        get{return p1;}
    }

    public bool T{
        get{return p2;}
    }

    public bool U{
        get{return d1;}
    }

    public bool V{
        get{return d2;}
    }

    // Converts the state object back into a string
    public string As_String(){
        string out_str = "";

        out_str += pos_i + " ";
        out_str += pos_j + " ";
        out_str += manhattan_distance + " ";
        out_str += (hasCargo ? "1" : "0") + " ";
        out_str += (p1 ? "1" : "0") + " ";
        out_str += (p2 ? "1" : "0") + " ";
        out_str += (d1 ? "1" : "0") + " ";
        out_str += (d2 ? "1" : "0");

        return out_str;
    }
}

using System.Collections;
using System.Collections.Generic;

// This is planned to be a more general construction of a Q-Table
// It is still a work in progress
//    Idea: You feed in the state space somehow, and the Q-Table generates rows
// for all possible permutations of those states

public struct QStateVariable{
    char varName;
    int lowerBound;
    int upperBound;

    public QStateVariable(char opName, int lBound, int uBound){
        varName = opName;
        lowerBound = lBound;
        upperBound = uBound;
    }
}

public class StateSpace{
    List<QStateVariable> dimensions;

    public StateSpace(){
        dimensions = new List<QStateVariable>();
    }

    public void AddDimension(){
        
    }
}

// Working on making this more general
public class QTableGeneral{
    // State format: i j Dm x s t u v
    // table[state] will return the row of the q table corresponding to that state
    // table[string, char] will return the float q-value of action char from state string    
    Dictionary<string, Dictionary<char, float>> table;


    // This constructor will generate a Q-Table for a given map, initializing all Q-Values to 0
    public QTableGeneral(Map _map){
        table = new Dictionary<string, Dictionary<char, float>>();

        // Add all possible states to the table upon initialization
        int maxDist = _map.Width + _map.Height - 2;
        for(int i = 0; i < _map.Height; i++){
            for(int j = 0; j < _map.Width; j++){

                // Each location has quite a few possible variants
                List<string> variants = new List<string>();
                string state_pos = "" + i + " " + j;

                // For each location in the map, add all possible variants of it to the list of variants
                for(int d = 1; d <= maxDist; d++){
                    for(int x = 0; x < 2; x++){
                        for(int s = 0; s < 2; s++){
                            for(int t = 0; t < 2; t++){
                                for(int u = 0; u < 2; u++){
                                    for(int v = 0; v < 2; v++){
                                        variants.Add(state_pos + " " + d + " " + x + " " + s + " " + t + " " + u + " " + v);
                                    }
                                }
                            }
                        }
                    }
                }

                // Add variants to Q table
                foreach (string s in variants){
                    // Create a dictionary to store Q-Values for each action indexed by operator
                    Dictionary<char, float> rowVals = new Dictionary<char, float>();
                    rowVals.Add('n', 0);
                    rowVals.Add('e', 0);
                    rowVals.Add('s', 0);
                    rowVals.Add('w', 0);
                    rowVals.Add('p', 0);
                    rowVals.Add('d', 0);

                    table.Add(s, rowVals);
                }
            }
        }
    }

    // Returns string that crudely represents a section of the table
    // Not inclusive on last
    public string DrawRows(int first, int last){
        if(first < 0 || first >= last || last > table.Count){
            return "Invalid\n";
        }

        string out_str = "            n e s w p d\n";

        List<string> keyList = States;
        for(int i = first; i < last; i++){
            out_str += keyList[i] + " > ";
            
            foreach( float v in table[keyList[i]].Values){
                out_str += v + " ";
            }
            out_str += "\n";
        }

        return out_str;
    }

    // Return dictionary corresponding to row of Q-Table for a given state
    public Dictionary<char, float> this[string state_as_string]{
        get{return table[state_as_string];}
    }

    // Return number of rows in Q-Table
    public int Size{
        get{return table.Count;}
    }

    // Gets list of possible states
    public List<string> States{
        get{return new List<string>(table.Keys);}
    }

    // Return Q-Value of a certain action from a certain state
    public float this[string state_as_string, char action]{
        get{return table[state_as_string][action];}
        set{table[state_as_string][action] = value;}
    }
}
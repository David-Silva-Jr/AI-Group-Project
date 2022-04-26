using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// I should probably go back and make this more general, but it'll do for now
public class QTable{
    // State format: i j Dm x s t u v
    // table[state] will return the row of the q table corresponding to that state
    // table[string, char] will return the float q-value of action char from state string    
    Dictionary<string, Dictionary<char, float>> table;

    // This constructor will generate a Q-Table for a given map, initializing all Q-Values to 0
    public QTable(Map _map){
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

    public void SaveToCSV(string name){
        using (StreamWriter outputFile = new StreamWriter("Output\\" + name)){
            outputFile.WriteLine("i, j, dm, x, s, t, u, v, n, e, s, w, p, d");
            foreach(KeyValuePair<string, Dictionary<char, float>> row in table){
                List<string> stateStuff = new List<string>(row.Key.Split(' '));

                string line = stateStuff[0];
                foreach(string s in stateStuff.Skip(1)){
                    line += ", " + s;
                }

                foreach(KeyValuePair<char, float> op in row.Value){
                    line += ", " + op.Value;
                }
                outputFile.WriteLine(line);
            }
        }
    }

    public char MostValuableAction(string state_as_string){
        return table[state_as_string].Where(e => e.Value == table[state_as_string].Values.Max()).First().Key;
    }

    // Return dictionary corresponding to row of Q-Table for a given state
    public Dictionary<char, float> this[string state_as_string]{
        get{return table[state_as_string];}
    }

    // Return number of rows in Q-Table
    public int Size{
        get{return table.Count;}
    }

    public Dictionary<string, Dictionary<char, float>> Dict{
        get{return table;}
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
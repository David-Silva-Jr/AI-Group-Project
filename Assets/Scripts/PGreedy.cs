using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PGreedy : I_Policy
{
    private Random rand; // System.Random

    public PGreedy(int seed){
        rand = new Random(seed);
    }

    // Note that ops should only contain actions available from state_as_string
    public char GetAction(QTable table, string state_as_string, List<char> ops){
        if(ops.Contains('p')){
            return 'p';
        }

        if(ops.Contains('d')){
            return 'd';
        }

        // Get values of available operators
        List<float> relevantOpValues = new List<float>(table[state_as_string].Where(e => ops.Contains(e.Key)).Select(e => e.Value));

        // Gets all operators tied for most valuable
        // Checking for difference rather than equality because of floating-point inaccuracy
        List<char> maxOps = new List<char>(ops.Where(e => Math.Abs(table[state_as_string, e] - relevantOpValues.Max()) < 0.0001f));

        // Randomly select from all actions tied for best
        int selection = rand.Next(maxOps.Count);
        return maxOps[selection];
    }
}

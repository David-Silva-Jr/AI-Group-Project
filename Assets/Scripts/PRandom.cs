using System;
using System.Collections;
using System.Collections.Generic;

public class PRandom : I_Policy
{
    private Random rand; // Note that this is System.Random

    public PRandom(int seed){
        rand = new Random(seed);
    }

    public char GetAction(QTable table, string state_as_string, List<char> ops){
        if(ops.Contains('p')){
            return 'p';
        }

        if(ops.Contains('d')){
            return 'd';
        }

        int selection = rand.Next(ops.Count);
        return ops[selection];
    }
}

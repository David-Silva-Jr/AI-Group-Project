using System.Linq;
using System.Collections;
using System.Collections.Generic;

// Note that this class inherits from the I_Learning_Formula interface
public class Learning_Q : I_Learning_Formula
{
    private float learning_rate;
    private float discount_rate;

    public Learning_Q(float alpha, float gamma){
        learning_rate = alpha;
        discount_rate = gamma;
    }

    // Implementing the function required by the interface
    public void UpdateQTable(ref QTable table, List<char> possibleOps, string old_state, char op, float reward, string new_state){
        float oldVal = table[old_state, op];
        List<float> newStateQVals = new List<float>(table[new_state].Where(e => possibleOps.Contains(e.Key)).Select(e => e.Value));

        table[old_state, op] = (1-learning_rate) * oldVal + learning_rate * (reward + discount_rate*newStateQVals.Max());
    }
}

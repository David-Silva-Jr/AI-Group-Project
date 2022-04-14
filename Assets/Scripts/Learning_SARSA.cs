using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Learning_SARSA : I_Learning_Formula
{
    private float learning_rate;
    private float discount_rate;

    public Learning_SARSA(float alpha, float gamma){
        learning_rate = alpha;
        discount_rate = gamma;
    }

    public void UpdateQTable(ref QTable table, string old_state, char op, float reward, string new_state){
        float oldVal = table[old_state, op];
        List<float> newStateQVals = new List<float>(table[new_state].Values);

        table[old_state, op] = oldVal + learning_rate * (reward + discount_rate*newStateQVals.Max() - oldVal);
    }
}

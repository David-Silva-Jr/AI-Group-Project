using System.Collections;
using System.Collections.Generic;

public interface I_Learning_Formula
{
    // All learning formulas must have a function by which they update the Q-Table
    void UpdateQTable(ref QTable table, string old_state, char op, float reward, string new_state);
}

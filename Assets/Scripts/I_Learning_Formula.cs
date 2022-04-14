using System.Collections;
using System.Collections.Generic;

// You can implement whatever learning formula you want, but it must inherit from this interface
public interface I_Learning_Formula
{
    // All learning formulas must have a function by which they update the Q-Table
    void UpdateQTable(ref QTable table, string old_state, char op, float reward, string new_state);
}

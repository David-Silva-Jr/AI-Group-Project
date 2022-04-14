using System.Collections;
using System.Collections.Generic;

public interface I_Policy{

    // GetAction takes a state and returns an action operator based on a Q-Table
    // Only checks for actions listed in possible_actions
    char GetAction(QTable table, string state_as_string, List<char> possible_actions);

    // Updates the Q-Table based on specific policy
    // Assumes op is valid operation given state
    // void UpdateQTable(ref QTable table, string old_state_as_string, char op, float reward, string new_state_as_string);
}
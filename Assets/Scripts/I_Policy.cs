using System.Collections;
using System.Collections.Generic;

//    This is an interface so you can make policies with whatever parameters you want,
// as long as they implement GetAction as specified here
//    Also won't interfere if you need to make a policy that inherits from MonoBehavior
// for some reason
public interface I_Policy{

    // GetAction takes a state and returns an action operator based on a Q-Table
    // Only checks for actions listed in possible_actions
    char GetAction(QTable table, string state_as_string, List<char> possible_actions);
}
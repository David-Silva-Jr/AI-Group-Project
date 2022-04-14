using System;
using System.Collections;
using System.Collections.Generic;

public class Environment
{
    private Map world;
    private QTable qTable;
    private DAgent male;
    private DAgent female;
    private int turn; // male turn is 0, female turn is 1

    private I_Learning_Formula formula; // How is the Q-Table updating?
    private I_Policy policy; // What RL Policy is active?

    int manhattan_distance; // Distance between agents

    // pickups
    private Tile s;
    private Tile t;

    private int s_initial;
    private int t_initial;

    // dropoffs
    private Tile u;
    private Tile v;

    private int u_initial;
    private int v_initial;

    public Environment(int world_width, int world_height, I_Policy rl_policy, I_Learning_Formula rl_formula, int s_capacity, int si, int sj, int t_capacity, int ti, int tj, int u_capacity, int ui, int uj, int v_capacity, int vi, int vj){
        world = new Map(world_width, world_height);

        policy = rl_policy;
        formula = rl_formula;

        // Assign pickups
        s = world[si, sj];
        s.Resources = s_capacity;
        s_initial = s_capacity;

        t = world[ti, tj];
        t.Resources = t_capacity;
        t_initial = t_capacity;
    
        // Assign dropoffs
        u = world[ui, uj];
        u.Resources = -u_capacity;
        u_initial = -u_capacity;

        v = world[vi, vj];
        v.Resources = -v_capacity;
        v_initial = -v_capacity;

        qTable = new QTable(world);
        male = new DAgent(world, "Bob", 0, 0);
        female = new DAgent(world, "Alice", world_height-1, world_width-1);
        UpdateManhattanDistance();
        turn = 0;
    }

    public QTable QTable{
        get{return qTable;}
    }

    public I_Learning_Formula Formula{
        get{return formula;}
        set{formula = value;}
    }

    public I_Policy Policy{
        get{return Policy;}
        set{policy = value;}
    }

    public DAgent Male{
        get{return male;}
    }

    public DAgent Female{
        get{return female;}
    }

    public Map World{
        get{return world;}
    }

    // These accessors just return if the pickups and dropoffs are still active
    public bool S{
        get{return s.IsPickup;}
    }

    public bool T{
        get{return t.IsPickup;}
    }

    public bool U{
        get{return u.IsDropoff;}
    }

    public bool V{
        get{return v.IsDropoff;}
    }

    // Returns true if final condition is reached
    public bool FinalConditionReached{
        get{return !(S || T || U || V);}
    }

    public void DoTurn(){
        if(turn == 0){
            MakeAgentDoSomething(ref male);
        }
        else{
            MakeAgentDoSomething(ref female);
        }

        turn++;
        turn = turn%2;

        if(FinalConditionReached){
            Reset();
        }
    }

    private void MakeAgentDoSomething(ref DAgent agent){
        // Get possible actions
        List<char> possible_actions = agent.GetAvailableActions();
        // Debug.Log("Possible moves: ");
        // foreach(char c in possible_actions){
        //     Debug.Log(c);
        // }

        // Get current state as string
        string state_as_string = RLFramework.StateToString(agent.Row, agent.Col, manhattan_distance, agent.HasCargo, S, T, U, V);
        // Debug.Log("State as string: " + state_as_string);

        // Chose an action based on policy
        char chosen = policy.GetAction(qTable, state_as_string, possible_actions);
        // Debug.Log("Chosen action: " + chosen);

        // Perform action
        agent.DoAction(chosen);
        // Debug.Log("Action performed");

        // In case the manhattan distance has changed, update it
        // I wanted to make this automatic with events, but it's really important that this happens before the next line runs,
        // so it's here instead
        UpdateManhattanDistance();

        // get new state as string
        string new_state_as_string = RLFramework.StateToString(agent.Row, agent.Col, manhattan_distance, agent.HasCargo, S, T, U, V);
        // Debug.Log("New state: " + new_state_as_string);

        // Get reward
        float reward = RLFramework.GetReward(state_as_string, new_state_as_string);
        // Debug.Log("Reward for move: " + reward);

        // Update Q-Table based on policy
        formula.UpdateQTable(ref qTable, state_as_string, chosen, reward, new_state_as_string);
        // Debug.Log("Table updated.");
    }

    public void UpdateManhattanDistance(){
        manhattan_distance = Math.Abs(male.Row - female.Row) + Math.Abs(male.Col - female.Col);
    }

    private void Reset(){
        male.MoveTo(0, 0);
        female.MoveTo(world.Height-1, world.Width-1);
        UpdateManhattanDistance();

        s.Resources = s_initial;
        t.Resources = t_initial;
        u.Resources = u_initial;
        v.Resources = v_initial;

        turn = 0;
    }
}

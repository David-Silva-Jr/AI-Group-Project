using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Environment
{
    private Map world;                  // Map object holding tile data
    private QTable qTable;              // Q-Table
    private DAgent male;                // Male agent
    private DAgent female;              // Female agent
    private int turn;                   // male turn is 0, female turn is 1

    private I_Learning_Formula formula; // How is the Q-Table updating?
    private I_Policy policy;            // What RL Policy is active?

    int manhattan_distance;             // Distance between agents

    // pickups
    private Tile p1;
    private Tile p2;

    private int p1_initial;
    private int p2_initial;

    // dropoffs
    private Tile d1;
    private Tile d2;
    private Tile d3;
    private Tile d4;

    private int d1_initial;
    private int d2_initial;
    private int d3_initial;
    private int d4_initial;

    bool firstDropoffFilled;

    // This event fires when Reset is called
    public event EventHandler ResetCalled;

    // Statistics
    public List<int> turnsResetCalled;
    public List<int> turnsMaleBumped;
    public List<int> turnsFemaleBumped;
    public List<int> distancePerTurn;

    public Environment(int world_width, int world_height, I_Policy rl_policy, I_Learning_Formula rl_formula){
        world = new Map(world_width, world_height);

        policy = rl_policy;
        formula = rl_formula;

        qTable = new QTable(world);
        male = new DAgent(world, "Bob", 0, 0);
        female = new DAgent(world, "Alice", world_height-1, world_width-1);
        UpdateManhattanDistance();

        turn = 0;
        turnsResetCalled = new List<int>();
        turnsMaleBumped = new List<int>(); // Need to change this to work as most valuable move only is considered
        turnsFemaleBumped = new List<int>();
        distancePerTurn = new List<int>();

        distancePerTurn.Add(manhattan_distance);

        firstDropoffFilled = false;
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

    public Tile P1{
        get{return p1;}
        set{
            if (p1 != null){
                value.Resources = p1.Resources;
                p1.Resources = 0;
            }
            p1 = value;
        }
    }

    public Tile P2{
        get{return p2;}
        set{
            if (p2 != null){
                value.Resources = p2.Resources;
                p2.Resources = 0;
            }
            p2 = value;
        }
    }

    public Tile D1{
        get{return d1;}
        set{
            if (d1 != null){
                value.Resources = d1.Resources;
                d1.Resources = 0;
            }
            d1 = value;
        }
    }

    public Tile D2{
        get{return d2;}
        set{
            if (d2 != null){
                value.Resources = d2.Resources;
                d2.Resources = 0;
            }
            d2 = value;
        }
    }
    public Tile D3{
        get{return d3;}
        set{
            if (d3 != null){
                value.Resources = d3.Resources;
                d3.Resources = 0;
            }
            d3 = value;
        }
    }
    public Tile D4{
        get{return d4;}
        set{
            if (d4 != null){
                value.Resources = d4.Resources;
                d4.Resources = 0;
            }
            d4 = value;
        }
    }

    // Returns true if final condition is reached
    public bool FinalConditionReached{
        get{return p1.Resources == 0 && p2.Resources == 0 && !male.HasCargo && !female.HasCargo;}
    }

    public void InitializePAndDValues(int _p1, int _p2, int _d1, int _d2, int _d3, int _d4){
        p1_initial = _p1;
        p2_initial = _p2;

        d1_initial = _d1;
        d2_initial = _d2;
        d3_initial = _d3;
        d4_initial = _d4;
    }

    public void InitializePAndDTiles(Vector2Int tile_p1, Vector2Int tile_p2, Vector2Int tile_d1, Vector2Int tile_d2, Vector2Int tile_d3, Vector2Int tile_d4){
        p1 = world[tile_p1];
        p2 = world[tile_p2];

        p1.Resources = p1_initial;
        p2.Resources = p2_initial;

        d1 = world[tile_d1];
        d2 = world[tile_d2];
        d3 = world[tile_d3];
        d4 = world[tile_d4];

        d1.Resources = d1_initial;
        d2.Resources = d2_initial;
        d3.Resources = d3_initial;
        d4.Resources = d4_initial;
    }

    // Has an agent perform an action and resets if goal condition is reached
    public void DoTurn(){
        if(turn%2 == 0){
            MakeAgentDoSomething(ref male);
        }
        else{
            MakeAgentDoSomething(ref female);
        }

        turn++;
        distancePerTurn.Add(manhattan_distance);

        if((!d1.IsDropoff || !d2.IsDropoff || !d3.IsDropoff || !d4.IsDropoff) && !firstDropoffFilled){
            qTable.SaveToCSV("Q-Table-FirstDropoffFilledTurn" + turn + ".csv");
            firstDropoffFilled = true;
        }

        if(FinalConditionReached){
            Reset();
        }
    }

    // Uses loaded policy to pass appropriate action to an agent
    // Agent then performs that action
    // Q-Table is updated afterwards
    private void MakeAgentDoSomething(ref DAgent agent){
        // Get available actions
        List<char> available_actions = agent.GetAvailableActions();

        // Get possible actions
        List<char> possible_actions = agent.GetPossibleActions();

        // Get correct s, t, u, v for context
        // Kinda gross but it was a quick fix
        bool S;
        bool T;
        bool U;
        bool V;

        if(agent.HasCargo){
            S = d1.IsDropoff;
            T = d2.IsDropoff;
            U = d3.IsDropoff;
            V = d4.IsDropoff;
        }
        else{
            S = p1.IsPickup;
            T = p2.IsPickup;
            U = false;
            V = false;
        }

        // Get current state as string
        string state_as_string = RLFramework.StateToString(agent.Row, agent.Col, manhattan_distance, agent.HasCargo, S, T, U, V);
        // Debug.Log("State as string: " + state_as_string);

        // Chose an action based on policy
        char chosen = policy.GetAction(qTable, state_as_string, available_actions);

        // See what an unobstructed agent would choose
        char ideal = qTable.MostValuableAction(state_as_string);

        // Check for bumps
        if(chosen != ideal && manhattan_distance == 1){
            // Debug.Log("Chosen: " + chosen + ", ideal:" + ideal);
            if(turn%2 == 0 ){
                turnsMaleBumped.Add(turn);
                // Debug.Log("male bumped at turn " + turn);
            }
            else{
                turnsFemaleBumped.Add(turn);
                // Debug.Log("female bumped at turn " + turn);
            }
        }

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

        // Get actions possible from new state
        List<char> new_possible_actions = agent.GetAvailableActions();

        // Update Q-Table based on policy
        formula.UpdateQTable(ref qTable, new_possible_actions, state_as_string, chosen, reward, new_state_as_string);
        // Debug.Log("Table updated.");
    }

    public void UpdateManhattanDistance(){
        manhattan_distance = Math.Abs(male.Row - female.Row) + Math.Abs(male.Col - female.Col);
    }

    private void Reset(){
        male.MoveTo(0, 0);
        female.MoveTo(world.Height-1, world.Width-1);
        UpdateManhattanDistance();

        p1.Resources = p1_initial;
        p2.Resources = p2_initial;

        d1.Resources = d1_initial;
        d2.Resources = d2_initial;
        d3.Resources = d3_initial;
        d4.Resources = d4_initial;

        turnsResetCalled.Add(turn);
        ResetCalled?.Invoke(this, EventArgs.Empty);

        if(turnsResetCalled.Count == 1){
            qTable.SaveToCSV("Q-Table-Reset" + turnsResetCalled.Count + ".csv");
        }
    }

    public void WriteStatsToFiles(){
        using (StreamWriter outputFile = new StreamWriter("Output\\Turns-Reset-Called.txt")){
            foreach(int v in turnsResetCalled){
                outputFile.WriteLine(v);
            }
        }

        using (StreamWriter outputFile = new StreamWriter("Output\\Turns-Male-Bumped.txt")){
            foreach(int v in turnsResetCalled){
                outputFile.WriteLine(v);
            }
        }

        using (StreamWriter outputFile = new StreamWriter("Output\\Turns-Female-Bumped.txt")){
            foreach(int v in turnsResetCalled){
                outputFile.WriteLine(v);
            }
        }

        using (StreamWriter outputFile = new StreamWriter("Output\\Manhattan-Distance.txt")){
            foreach(int v in turnsResetCalled){
                outputFile.WriteLine(v);
            }
        }

        qTable.SaveToCSV("Q-Table-Final.csv");
    }
}

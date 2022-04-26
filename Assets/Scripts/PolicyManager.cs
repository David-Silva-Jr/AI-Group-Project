using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

//[On WorldManager]
//Policy manager receives agents' states and operators as inputs and do all necessary computations
public class PolicyManager : MonoBehaviour
{

    [SerializeField] private GameObject policyDropdown;
    [SerializeField] private GameObject formulaDropdown;
    [SerializeField] private GameObject stepInputField;

    private Dictionary<string, double> Qtable = new Dictionary<string, double>();

    int endstep;

    //Manhattan
    int totalManhattan = 0;
    int Manhattancount = 0;
    double avgManhattan = 0;
    [SerializeField] private Text ManhattanText;

    public enum Formulas
    {
        QLEARNING,
        SARSA
    }

    public enum Policies
    {
        PRANDOM,
        PGREEDY,
        PEXPLOIT
    }

    //Displayed stats
    [SerializeField] private Text randomSteps;
    private int randomStep = 0;
    [SerializeField] private Text greedySteps;
    private int greedyStep = 0;
    [SerializeField] private Text exploitSteps;
    private int exploitStep = 0;

    [SerializeField] private Text terminations;
    private int termination = 0;

    //Policy values
    public Formulas formula;
    public Policies policy;
    public float learningRate;
    public float discountRate;

    //States of agent only
    Agent agentA;
    int[] AState;

    Agent agentB;
    int[] BState;

    List<string> femaleOperators; //Available operators
    List<string> maleOperators; //Available operators

    List<string> newFemaleOperators; //Available operators
    List<string> newMaleOperators; //Available operators

    int[] zoneState;//State of zones only

    int[] femaleState = new int[8]; //Full state (posx,posy,distance,carrying,pickup/dropoff,pickup/dropoff,irrelevant/dropoff,irrelevant/dropoff)
    int[] newFemaleState = new int[8];

    int[] maleState = new int[8]; //Full state (posx,posy,distance,carrying,pickup/dropoff,pickup/dropoff,irrelevant/dropoff,irrelevant/dropoff)
    int[] newMaleState = new int[8];

    GridManager gridManager;
    TimeSystem timeSystem;
    int stepToPause = -1;


    // Start is called before the first frame update
    void Start()
    {
        //Get grid manager
        gridManager = GetComponent<GridManager>();

        timeSystem = GetComponent<TimeSystem>();
        //Subscribe to OnTick event
        TimeSystem.OnTick += OnTickHandler;


    }

    //Get the policy from GUIs
    public void GetPolicy()
    {
        policy = (Policies)policyDropdown.GetComponent<Dropdown>().value;
    }

    //Get the formula from GUIs
    public void GetFormula()
    {
        formula = (Formulas)formulaDropdown.GetComponent<Dropdown>().value;
    }

    //Handles OnTick event
    private void OnTickHandler(object sender, TimeSystem.OnTickEventArgs e)
    { 
        //Run female agent first in odd ticks starting with 1
        //Get female operators
        if (e.tick % 2 == 1)
        {
           
            femaleOperators = gridManager.GetAgent("female").GetOperator();
            //Get female state
            femaleState = GetState("female", "male");
            //Do female action
            string femaleAction = GetAction(policy, femaleOperators, femaleState);
            agentA.DoAction(femaleAction);
            newFemaleOperators = gridManager.GetAgent("female").GetOperator();
            newFemaleState = GetState("female", "male");
            updateQTable(femaleState, femaleAction, newFemaleState, newFemaleOperators);
        }
        //Run male agent after in even ticks
        else
        {
            maleOperators = gridManager.GetAgent("male").GetOperator();
            //Get male state
            maleState = GetState("male", "female");
            //Do male action
            string maleAction =  GetAction(policy, maleOperators, maleState);
            agentA.DoAction(maleAction);
            newMaleOperators = gridManager.GetAgent("male").GetOperator();
            newMaleState = GetState("male", "female");
            updateQTable(maleState, maleAction, newMaleState, newMaleOperators);
            AvgManhattan();

            //Display statistics
            CountStep();
        }

        //Pause on step
        if (stepToPause == e.tick / 2)
        {
            timeSystem.Pause();
            gridManager.PauseMenu();
        }
    }

    //Set the variables operators and state for agent of gender A
    private int[] GetState(string genderA, string genderB)
    {
        int[] tempState = new int[8];
        //Get both A and B states as we need the both to compute distance, the B state won't be affected if A moves
        agentA = gridManager.GetAgent(genderA);
        AState = agentA.GetAgentState();


        agentB = gridManager.GetAgent(genderB);
        BState = agentB.GetAgentState();

        //Assign pos x, posy
        for (int i = 0; i < 2; i++)
        {
            tempState[i] = AState[i];
        }

        //Assign distance
        tempState[2] = Math.Abs(AState[0] - BState[0]) + Math.Abs(AState[1] - BState[1]);

        //Assign carrying
        tempState[3] = AState[2];


        //Get Zone states. These may be affected after other agent moves
        zoneState = gridManager.GetZoneState();

        //If agent not carrying block
        if (AState[2] == 0)
        {
            tempState[4] = zoneState[0]; //Pickup zone
            tempState[5] = zoneState[1]; //Pickup zone
            tempState[6] = 0; //Irrelevant
            tempState[7] = 0; //Irrelevant
        }
        //If agent is carrying block
        else for (int i = 4; i < 8; i++)
            {
                tempState[i] = zoneState[i - 2]; //Dropoff zones
            }
        return tempState;
    }

    private string GetAction(Policies policy, List<string> operators, int[] state)
    {
        //Pickup/Dropoff
        if (operators.Contains("p"))
        {
            return "p";
        }
        if (operators.Contains("d"))
        {
            return "d";
        }
        //Movement
        if (policy == Policies.PRANDOM)
        {
            var random = new System.Random();
            int index = random.Next(operators.Count);
            return operators[index];
        }
        else
        {
            double q; //qValue
            List<double> qValue = new List<double>(); //List of all qValue
            //Add all operators' qValue
            foreach (string op in operators)
            {
                if (!Qtable.ContainsKey(GetKey(op, state)))
                    q = 0;
                else q = Qtable[GetKey(op, state)];
                qValue.Add(q);
            }
            //Get operator with highest qValue
            int maxIndex = qValue.IndexOf(qValue.Max());
            if (policy == Policies.PGREEDY)
            {
                return operators[maxIndex];
            }
            else
            {
                System.Random r = new System.Random();
                int rInt = r.Next(0, 100); //for ints
                if (rInt <= 80)
                    return operators[maxIndex];
                else
                {
                    //Make temp operator list
                    List<string> tempOp = new List<string>(operators);
                    //Remove max index
                    tempOp.RemoveAt(maxIndex);
                    //Choose at random
                    var random = new System.Random();
                    int index = random.Next(tempOp.Count);
                    return tempOp[index];
                }

            }
        }
    }

    //Update Qtable
    private void updateQTable(int[] oldState, string op, int[] newState, List<string> newOperators)
    {

        int reward = -1;
        if (op == "d" || op == "p")
        {
            reward = 13;
        }

        if (formula == Formulas.QLEARNING)
        {
            double maxQValue = findMaxQValue(newOperators, newState);

            if (!Qtable.ContainsKey(GetKey(op, oldState)))
            {
                Qtable[GetKey(op, oldState)] = 0;
            }
            Qtable[GetKey(op, oldState)] = (1 - learningRate) * Qtable[GetKey(op, oldState)] + learningRate * (reward + discountRate * maxQValue);
        }
        else
        {
            if (!Qtable.ContainsKey(GetKey(op, oldState)))
            {
                Qtable[GetKey(op, oldState)] = 0;
            }

            string newOp = GetAction(policy, newOperators, newState);

            if (!Qtable.ContainsKey(GetKey(newOp, newState)))
            {
                Qtable[GetKey(newOp, newState)] = 0;
            }

            Qtable[GetKey(op, oldState)] = Qtable[GetKey(op, oldState)] + learningRate * (reward + discountRate * Qtable[GetKey(newOp, newState)] - Qtable[GetKey(op, oldState)]);
        }
    }


    private double findMaxQValue(List<string> operators, int[] state)
    {
        double q; //qValue
        List<double> qValue = new List<double>(); //List of all qValue
                                            //Add all operators' qValue
        foreach (string op in operators)
        {
            if (!Qtable.ContainsKey(GetKey(op, state)))
                q = 0;
            else q = Qtable[GetKey(op, state)];
            qValue.Add(q);
        }
        //Get highest qValue
        return qValue.Max();
    }



    //Get Qtable Key
    private string GetKey(string op, int[] state)
    {
        string key = op;
        foreach (int i in state)
            key += i;
        return key;
    }

    private void CountStep()
    {
        switch (policy)
        {
            case Policies.PRANDOM:
                randomSteps.text = "Step: " + ++randomStep;
                break;
            case Policies.PGREEDY:
                greedySteps.text = "Step: " + ++greedyStep;
                break;
            case Policies.PEXPLOIT:
                exploitSteps.text = "Step: " + ++exploitStep;
                break;
        }
    }

    public void CountTerminal()
    {
        terminations.text = "Terminations: " + ++termination;
    }

    public void PauseOnStep()
    {
        stepToPause = Int32.Parse(stepInputField.GetComponent<Text>().text);
    }

    private void AvgManhattan()
    {
        totalManhattan += maleState[2];
        Manhattancount++;
        avgManhattan = (double)totalManhattan / (double)Manhattancount;
        ManhattanText.text = "Average Manhattan Distance: " + avgManhattan;
    }

}
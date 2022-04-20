using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[On WorldManager]
//Policy manager receives agents' states and operators as inputs and do all necessary computations
public class PolicyManager : MonoBehaviour
{
    //States of agent only
    Agent agentA;
    int[] AState;

    Agent agentB;
    int[] BState;

    List<string> operators; //Available operators
    int[] zoneState;//State of zones only
    int[] state = new int[8]; //Full state (posx,posy,carrying,distance,pickup/dropoff,pickup/dropoff,irrelevant/dropoff,irrelevant/dropoff)

    GridManager gridManager;


    // Start is called before the first frame update
    void Start()
    {
        //Get grid manager
        gridManager = GetComponent<GridManager>();

        TimeSystem timeSystem = FindObjectOfType<TimeSystem>();
        //Subscribe to OnTick event
        TimeSystem.OnTick += OnTickHandler;


    }

    //Handles OnTick event
    private void OnTickHandler(object sender, TimeSystem.OnTickEventArgs e)
    {
        //Run female agent first in odd ticks starting with 1
        if(e.tick%2 == 1)
        {
            SetOperatorAndState("female", "male");
        }
        //Run male agent after in even ticks
        else
        {
            SetOperatorAndState("male", "female");
        }
    }

    //Set the variables operators and state for agent of gender A
    private void SetOperatorAndState(string genderA, string genderB)
    {
        //Get both A and B states as we need the both to compute distance, the B state won't be affected if A moves
        agentA = gridManager.GetAgent(genderA);
        AState = agentA.GetAgentState();


        agentB = gridManager.GetAgent(genderB);
        BState = agentB.GetAgentState();

        //Assign pos x, posy, carrying
        for (int i = 0; i < 3; i++)
        {
            state[i] = AState[i];
        }
        //Assign distance
        state[3] = Math.Abs(AState[0] - BState[0]) + Math.Abs(AState[1] - BState[1]);


        //Get operators and zone states. These may be affected after male moves
        operators = agentA.GetOperator();
        zoneState = gridManager.GetZoneState();

        //If agent not carrying block
        if (AState[2] == 0)
        {
            state[4] = zoneState[0]; //Pickup zone
            state[5] = zoneState[1]; //Pickup zone
            state[6] = 0; //Irrelevant
            state[7] = 0; //Irrelevant
        }
        //If agent is carrying block
        else for (int i = 4; i < 8; i++)
            {
                state[i] = zoneState[i - 2]; //Dropoff zones
            }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[On WorldManager]
//Policy manager receives agents' states and operators as inputs and do all necessary computations
public class PolicyManager : MonoBehaviour
{
    Agent male;
    List<string> maleOpList;
    int[] maleState;


    // Start is called before the first frame update
    void Start()
    {
        TimeSystem timeSystem = FindObjectOfType<TimeSystem>();
        //Subscribe to OnTick event
        TimeSystem.OnTick += OnTickHandler;
    }

    //Handles OnTick event
    private void OnTickHandler(object sender, TimeSystem.OnTickEventArgs e)
    {
        if(e.tick == 1)
            male = FindObjectOfType<Agent>();
        maleOpList = male.GetOperator();
        maleState = male.GetAgentState();
        foreach (string op in maleOpList)
            Debug.Log(op);
        foreach (int state in maleState)
            Debug.Log(state);
    }
}
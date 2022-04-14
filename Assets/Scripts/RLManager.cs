using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Formulas{
    QLEARNING,
    SARSA
}

public enum Policies{
    PRANDOM,
    PGREEDY,
    PEXPLOIT
}

public class RLManager : MonoBehaviour
{
    public int mapSize;

    public Formulas formula;
    public Policies policy;
    public float learningRate;
    public float discountRate;

    public Policies policy2;
    // public float learningRate2;
    // public float discountRate2;

    public int s;
    public Vector2Int sPos;

    public int t;
    public Vector2Int tPos;

    public int u;
    public Vector2Int uPos;

    public int v;
    public Vector2Int vPos;

    public int randomSeed;

    // How many steps to learn before actually visualizing
    public int numRandomSteps;
    public int numTrainingSteps;
    
    public ALT_TimeSystem timer;

    public GameObject cube;
    public Material matpickup;
    public Material matDropoff;

    public GameObject capsule;
    public Material matMale;
    public Material matFemale;

    public Material matCargo;

    private Environment environment;

    private float t_lerp;
    private float moveTime;

    private GameObject maleVis;
    private Vector3 malePos;
    private Vector3 maleTarget;

    private GameObject femaleVis;
    private Vector3 femalePos;
    private Vector3 femaleTarget;


    void Awake()
    {
        // Create learning method
        I_Learning_Formula chosen_formula = FormulaFromSelection(formula, learningRate, discountRate);

        // Create action policy
        I_Policy chosen_policy = PolicyFromSelectiom(policy);

        // Instantiate environment
        environment = new Environment(mapSize, mapSize, chosen_policy, chosen_formula, s, sPos.x, sPos.y, t, tPos.x, tPos.y, u, uPos.x, uPos.y, v, vPos.x, vPos.y);

        // Create map from cubes
        for(int i = 0; i < mapSize; i++){
            for(int j = 0; j < mapSize; j++){
                GameObject tileCube = Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity, transform);
                if(environment.World[i, j].IsPickup){
                    tileCube.GetComponent<MeshRenderer>().sharedMaterial = matpickup;
                }
                else if(environment.World[i, j].IsDropoff){
                    tileCube.GetComponent<MeshRenderer>().sharedMaterial = matDropoff;
                }
            }
        }

        // Do exploration and training steps
        for(int i = 0; i < numRandomSteps; i++){
            environment.DoTurn();
        }
        environment.Policy = PolicyFromSelectiom(policy2);
        for(int i = 0; i < numTrainingSteps; i++){
            environment.DoTurn();
        }

        // Start visualization stuff
        moveTime = timer.GetMaxTickTimer() / 5;
        t_lerp = 0;

        // Create agents
        malePos = new Vector3(environment.Male.Row, 1.5f, environment.Male.Col);
        maleTarget = malePos;
        maleVis = Instantiate(capsule, new Vector3(environment.Male.Row, 1.5f, environment.Male.Col), Quaternion.identity, transform);
        maleVis.GetComponent<MeshRenderer>().sharedMaterial = matMale;

        femalePos = new Vector3(environment.Female.Row, 1.5f, environment.Female.Col);
        femaleTarget = femalePos;
        femaleVis = Instantiate(capsule, new Vector3(environment.Female.Row, 1.5f, environment.Female.Col), Quaternion.identity, transform);
        femaleVis.GetComponent<MeshRenderer>().sharedMaterial = matFemale;

        // Set up event actions
        ALT_TimeSystem.Tick += OnTick;
        environment.Male.LocationChanged += OnMaleMove;
        environment.Male.HasCargoChanged += OnMaleCargo;

        environment.Female.LocationChanged += OnFemaleMove;
        environment.Female.HasCargoChanged += OnFemaleCargo;

        // print(environment.QTable.DrawRows(0, environment.QTable.Size));
    }

    I_Learning_Formula FormulaFromSelection(Formulas selection, float alpha, float gamma){
        if(selection == Formulas.QLEARNING){
            return new Learning_Q(learningRate, discountRate);
        }
        else if(selection == Formulas.SARSA){
            return new Learning_SARSA(learningRate, discountRate);
        }
        else{
            return new Learning_Q(learningRate, discountRate);
        }
    }

    I_Policy PolicyFromSelectiom(Policies selection){
        if(selection == Policies.PRANDOM){
            return new PRandom(randomSeed);
        }
        else if(selection == Policies.PEXPLOIT){
            return new PExploit(randomSeed);
        }
        else{
            return new PRandom(randomSeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Moves agents smoothly from start to target positions
        //    Set position to target if they get close enough because
        // t gets reset every tick and we don't want one agent to
        // scoot back every time the other moves
        t_lerp += Time.deltaTime / moveTime;
        maleVis.transform.position = Vector3.Lerp(malePos, maleTarget, t_lerp);
        if((maleVis.transform.position - maleTarget).sqrMagnitude < 0.01f){
            malePos = maleTarget;
        }

        femaleVis.transform.position = Vector3.Lerp(femalePos, femaleTarget, t_lerp);
        if((femaleVis.transform.position - femaleTarget).sqrMagnitude < 0.01f){
            femalePos = femaleTarget;
        }
    }

    // Do a turn every tick
    void OnTick(object sender, ALT_TimeSystem.OnTickEventArgs e){
        environment.DoTurn();
        t_lerp = 0;
    }

    // Set positions on agent move
    void OnMaleMove(object sender, EventArgs e){
        maleTarget = new Vector3(environment.Male.Row, 1.5f, environment.Male.Col);
    }

    void OnFemaleMove(object sender, EventArgs e){
        femaleTarget = new Vector3(environment.Female.Row, 1.5f, environment.Female.Col);
    }

    // Set color on agent pickup/dropoff
    void OnMaleCargo(object sender, DAgent.PropertyChangedEventArgs<bool> e){
        if(e.newValue){
            maleVis.GetComponent<MeshRenderer>().sharedMaterial = matCargo;
        }
        else{
            maleVis.GetComponent<MeshRenderer>().sharedMaterial = matMale;
        }
    }

    void OnFemaleCargo(object sender, DAgent.PropertyChangedEventArgs<bool> e){
        if(e.newValue){
            femaleVis.GetComponent<MeshRenderer>().sharedMaterial = matCargo;
        }
        else{
            femaleVis.GetComponent<MeshRenderer>().sharedMaterial = matFemale;
        }
    }
}

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
    public float learningRate;
    public float discountRate;

    public Formulas formula;
    public Policies policy;

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
    public int numTrainingSteps;
    
    public ALT_TimeSystem timer;

    private Environment environment;

    void Awake()
    {
        // Create learning method
        I_Learning_Formula chosen_formula;
        if(formula == Formulas.QLEARNING){
            chosen_formula = new Learning_Q(learningRate, discountRate);
        }
        else if(formula == Formulas.SARSA){
            chosen_formula = new Learning_SARSA(learningRate, discountRate);
        }
        else{
            chosen_formula = new Learning_Q(learningRate, discountRate);
        }

        // Create action policy
        I_Policy chosen_policy;
        if(policy == Policies.PRANDOM){
            chosen_policy = new PRandom(randomSeed);
        }
        else{
            chosen_policy = new PRandom(randomSeed);
        }

        // Instantiate environment
        environment = new Environment(mapSize, mapSize, chosen_policy, chosen_formula, s, sPos.x, sPos.y, t, tPos.x, tPos.y, u, uPos.x, uPos.y, v, vPos.x, vPos.y);

        for(int i = 0; i < mapSize; i++){
            for(int j = 0; j < mapSize; j++){
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

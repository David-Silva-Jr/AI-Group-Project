using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E4Visualizer : MonoBehaviour
{
    public int mapSize;

    public Formulas formula;
    public Policies policy;
    public float learningRate;
    public float discountRate;

    public Policies policy2;

    public Vector2Int maleLocation;
    public Vector2Int femaleLocation;

    public int randomSeed;

    // Pickups
    public int p1;
    public Vector2Int p1Pos;

    public int p2;
    public Vector2Int p2Pos;

    // Dropoffs
    public int d1;
    public Vector2Int d1Pos;

    public int d2;
    public Vector2Int d2Pos;

    public int d3;
    public Vector2Int d3Pos;

    public int d4;
    public Vector2Int d4Pos;

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

    public bool drawArrows;
    public GameObject arrow;

    private Experiment4 environment;

    private float t_lerp;
    private float moveTime;

    private GameObject maleVis;
    private Vector3 malePos;
    private Vector3 maleTarget;

    private GameObject femaleVis;
    private Vector3 femalePos;
    private Vector3 femaleTarget;

    private List<List<GameObject>> tileCubes;
    private List<List<Transform>> arrows;

    // Statistics


    void Awake()
    {
        // Create learning method
        I_Learning_Formula chosen_formula = FormulaFromSelection(formula, learningRate, discountRate);

        // Create action policy
        I_Policy chosen_policy = PolicyFromSelectiom(policy);

        // Instantiate environment
        environment = new Experiment4(mapSize, mapSize, chosen_policy, chosen_formula);
        environment.InitializePAndDValues(p1, p2, d1, d2, d3, d4);
        environment.InitializePAndDTiles(
            p1Pos,
            p2Pos,
            d1Pos,
            d2Pos,
            d3Pos,
            d4Pos
        );

        // Create map from cubes
        tileCubes = new List<List<GameObject>>();
        for(int i = 0; i < mapSize; i++){
            List<GameObject> cubeRow = new  List<GameObject>();
            for(int j = 0; j < mapSize; j++){
                GameObject tileCube = Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity, transform);
                if(environment.World[i, j].IsPickup){
                    tileCube.GetComponent<MeshRenderer>().sharedMaterial = matpickup;
                }
                else if(environment.World[i, j].IsDropoff){
                    tileCube.GetComponent<MeshRenderer>().sharedMaterial = matDropoff;
                }
                cubeRow.Add(tileCube);
            }
            tileCubes.Add(cubeRow);
        }

        // Draw arrows for direction of most value
        if(drawArrows){
            arrows = new List<List<Transform>>();
            CreateArrows();
        }

        // Do exploration and training steps
        for(int i = 0; i < numRandomSteps; i++){
            environment.DoTurn();
        }
        environment.Policy = PolicyFromSelectiom(policy2);
        for(int i = 0; i < numTrainingSteps; i++){
            environment.DoTurn();
        }

        environment.WriteStatsToFiles();

        Debug.Log(environment.turnsResetCalled.Count + " terminal states reached.");
        Debug.Log(environment.distancePerTurn.Count + " distances counted.");
        Debug.Log(environment.turnsMaleBumped.Count + " times male bumped.");
        Debug.Log(environment.turnsFemaleBumped.Count + " turns female bumped.");

        // Start visualization stuff
        moveTime = timer.GetMaxTickTimer() / 5;
        t_lerp = 0;

        // Create agents
        environment.Male.MoveTo(maleLocation.x, maleLocation.y);
        malePos = new Vector3(environment.Male.Row, 1.5f, environment.Male.Col);
        maleTarget = malePos;
        maleVis = Instantiate(capsule, new Vector3(environment.Male.Row, 1.5f, environment.Male.Col), Quaternion.identity, transform);
        maleVis.GetComponent<MeshRenderer>().sharedMaterial = matMale;
        if(environment.Male.HasCargo){
            maleVis.GetComponent<MeshRenderer>().sharedMaterial = matCargo;
        }

        environment.Female.MoveTo(femaleLocation.x, femaleLocation.y);
        femalePos = new Vector3(environment.Female.Row, 1.5f, environment.Female.Col);
        femaleTarget = femalePos;
        femaleVis = Instantiate(capsule, new Vector3(environment.Female.Row, 1.5f, environment.Female.Col), Quaternion.identity, transform);
        femaleVis.GetComponent<MeshRenderer>().sharedMaterial = matFemale;
        if(environment.Female.HasCargo){
            femaleVis.GetComponent<MeshRenderer>().sharedMaterial = matCargo;
        }

        // Set up event actions
        ALT_TimeSystem.Tick += OnTick;
        ALT_TimeSystem.Tick += UpdateTileCubesOnTick;
        environment.Male.LocationChanged += OnMaleMove;
        environment.Male.HasCargoChanged += OnMaleCargo;

        environment.Female.LocationChanged += OnFemaleMove;
        environment.Female.HasCargoChanged += OnFemaleCargo;

        // print(environment.QTable.DrawRows(0, environment.QTable.Size));
    }

    // Convert user input to learning formula
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

    // convert user input to RL policy
    I_Policy PolicyFromSelectiom(Policies selection){
        if(selection == Policies.PRANDOM){
            Debug.Log("Selected PRandom");
            return new PRandom(randomSeed);
        }
        else if(selection == Policies.PEXPLOIT){
            Debug.Log("Selected PExploit");
            return new PExploit(randomSeed);
        }
        else if(selection == Policies.PGREEDY){
            Debug.Log("Selected PGreedy");
            return new PGreedy(randomSeed);
        }
        else{
            Debug.Log("Selected default (PRandom)");
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

    void CreateArrows(){
        Debug.Log("Creating arrows");
        for (int r = 0; r < tileCubes.Count; r++){
            List<Transform> arrow_row = new List<Transform>();
            for(int  c = 0; c < tileCubes[r].Count; c++){
                GameObject direction = Instantiate(arrow, tileCubes[r][c].transform.position + Vector3.up*.501f, Quaternion.AngleAxis(90, Vector3.right), tileCubes[r][c].transform);
                direction.name = "Arrow";
                if(environment.World[r, c].IsPickup || environment.World[r, c].IsDropoff){
                    direction.SetActive(false);
                }
                arrow_row.Add(direction.transform);
            }
            arrows.Add(arrow_row);
        }
    }

    // Function to set locations of agents

    // Can probably be sped up
    void UpdateArrowDirections(){
        for(int r = 0; r < environment.World.Height; r++){
            for(int c = 0; c < environment.World.Width; c++){
                Transform arrw = arrows[r][c];

                // Implement some way to find the highest value direction of a specific tile over all states on that tile
                List<string> relevantStates = new List<string>(environment.QTable.States.Where(e => e.IndexOf(r.ToString() + " " + c.ToString() + " ") == 0 )); //&& e.Split(' ')[3] == "1"));

                // Debug.Log(relevantStates.Count + " relevant states detected.");

                float nSum = 0;
                float eSum = 0;
                float sSum = 0;
                float wSum = 0;
                foreach(string k in relevantStates){
                    Dictionary<char, float> pairs = environment.QTable[k];
                    nSum += pairs['n'];
                    eSum += pairs['e'];
                    sSum += pairs['s'];
                    wSum += pairs['w'];
                }
                
                List<float> sums = new List<float>();
                List<KeyValuePair<char, float>> sumsPairs = new List<KeyValuePair<char, float>>();
                sumsPairs.Add(new KeyValuePair<char, float>('n', nSum));
                sumsPairs.Add(new KeyValuePair<char, float>('e', eSum));
                sumsPairs.Add(new KeyValuePair<char, float>('s', sSum));
                sumsPairs.Add(new KeyValuePair<char, float>('w', wSum));

                sums.Add(nSum);
                sums.Add(eSum);
                sums.Add(sSum);
                sums.Add(wSum);

                List<char> possibleMoves = RLFramework.GetPossibleMovementOperators(environment.World, r, c);

                int rotVal = 1 + sums.IndexOf(sumsPairs.Where(e => possibleMoves.Contains(e.Key)).Select(e => e.Value).Max());

                arrw.rotation = Quaternion.AngleAxis(90*rotVal, Vector3.up)*Quaternion.AngleAxis(90, Vector3.right);
            }
        }
    }

    void UpdateTileCubesOnTick(object sender, ALT_TimeSystem.OnTickEventArgs e){
        for(int i = 0; i < mapSize; i++){
            for(int j = 0; j < mapSize; j++){
                if(environment.World[i, j].IsPickup){
                    tileCubes[i][j].GetComponent<MeshRenderer>().sharedMaterial = matpickup;
                }
                else if(environment.World[i, j].IsDropoff){
                    tileCubes[i][j].GetComponent<MeshRenderer>().sharedMaterial = matDropoff;
                }
                else{
                    tileCubes[i][j].GetComponent<MeshRenderer>().sharedMaterial =  new Material(Shader.Find("Standard"));
                }
            }
        }
    }

    // Do a turn every tick
    void OnTick(object sender, ALT_TimeSystem.OnTickEventArgs e){
        environment.DoTurn();
        t_lerp = 0;

        if(drawArrows){
            UpdateArrowDirections();
        }
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

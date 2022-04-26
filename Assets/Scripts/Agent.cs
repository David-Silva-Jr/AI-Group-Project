using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//[On agents]
//
public class Agent : MonoBehaviour
{

    [SerializeField] private GameObject cargo;

    private float t; //Divided value for moving between points
    private Vector3 startPosition; //3D position for moving in world
    private int posX;   //2D position as grid data
    private int posY;
    int carrying = 0; //Actually bool
    private List<GameObject> moves = new List<GameObject>(); //List of 4 possible moves
    private List<string> operators = new List<string>(); //List of moveable directions
    int dirIndex; //Index of selected direction
    private float timeToReachTarget;

    //Bank account
    private Text bankText;
    private int bank = 0;

    TimeSystem timeSystem;

    private enum direction
    {
        n,
        e,
        s,
        w
    }

    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeSystem = FindObjectOfType<TimeSystem>();
        //The agents move twice as fast as the tick count so that they don't lag behind
        

        startPosition = transform.position;


        //Assign bank account
        if (name.Contains("Male Agent"))
        {
            bankText = GameObject.Find("MBankVal").GetComponent<Text>();
        }
        else
        {
            bankText = GameObject.Find("FBankVal").GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / timeToReachTarget;
        transform.position = Vector3.Lerp(startPosition, transform.parent.transform.position, t);

    }


    public List<string> GetOperator()
    {
        //Clear lists
        moves.Clear();
        operators.Clear();
        //Add all directions to direction list
        moves.Add(GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY + 1).ToString() + ")")); //Add north
        moves.Add(GameObject.Find("(X: " + (posX + 1).ToString() + "Y: " + (posY).ToString() + ")")); //Add east
        moves.Add(GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY - 1).ToString() + ")")); //Add south
        moves.Add(GameObject.Find("(X: " + (posX - 1).ToString() + "Y: " + (posY).ToString() + ")")); //Add west

        for(int i = 0;i < 4; i++)
        {
            Transform ground = moves[i].GetComponent<Cell>().GetGround();
            //If there is ground not occupied by another agent
            if (moves[i] != null && ground != null && ground.childCount == 4)
                operators.Add(((direction)i).ToString()); //Add a move to that ground to operator list
        }



        //Add pickup and dropoff operators if available
        string zoneOperator = GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY).ToString() + ")").GetComponent<Cell>().GetZoneOperator();
        if ((zoneOperator == "p" && carrying == 0) || (zoneOperator == "d" && carrying == 1))
            operators.Add(zoneOperator);

        return operators;
    }

    public void DoAction(string action)
    {
        //reset agent speed
        timeToReachTarget = timeSystem.GetMaxTickTimer() / 2;

        switch (action)
        {
            case "p":
                GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY).ToString() + ")").GetComponent<Cell>().ZoneAction(action);
                carrying = 1;
                cargo.SetActive(true);
                Banking(13);
                break;
            case "d":
                GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY).ToString() + ")").GetComponent<Cell>().ZoneAction(action);
                carrying = 0;
                cargo.SetActive(false);
                Banking(13);
                break;
            case "n":
                MoveTo(moves[0].GetComponent<Cell>());
                Banking(-1);
                moves[0].GetComponent<Cell>().IncrementColor();
                break;
            case "e":
                MoveTo(moves[1].GetComponent<Cell>());
                Banking(-1);
                moves[1].GetComponent<Cell>().IncrementColor();
                break;
            case "s":
                MoveTo(moves[2].GetComponent<Cell>());
                Banking(-1);
                moves[2].GetComponent<Cell>().IncrementColor();
                break;
            case "w":
                MoveTo(moves[3].GetComponent<Cell>());
                Banking(-1);
                moves[3].GetComponent<Cell>().IncrementColor();
                break;
        }
 
    }

    public void MoveTo(Cell destination)
    {
        t = 0;
        startPosition = transform.position;
        transform.SetParent(destination.GetGround());
        posX = destination.GetPosition().x;
        posY = destination.GetPosition().y;
    }

    public int[] GetAgentState()
    {
        int[] state = new int[] {posX, posY, carrying};
        return state;
    }

    //Change bank account
    private void Banking(int change)
    {
        bank += change;
        bankText.text = "bank: " + bank;
    }
}

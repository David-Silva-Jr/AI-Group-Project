using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[On agents]
//
public class Agent : MonoBehaviour
{
    private float t; //Divided value for moving between points
    private Vector3 startPosition; //3D position for moving in world
    private int posX;   //2D position as grid data
    private int posY;
    int carrying = 0; //Actually bool
    private List<GameObject> moves = new List<GameObject>(); //List of 4 possible moves
    private List<string> operators = new List<string>(); //List of moveable directions
    int dirIndex; //Index of selected direction
    private float timeToReachTarget;

    private enum direction
    {
        north,
        east,
        south,
        west
    }

    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeSystem timeSystem = FindObjectOfType<TimeSystem>();
        //The agents move twice as fast as the tick count so that they don't lag behind
        timeToReachTarget = timeSystem.GetMaxTickTimer()/5;

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / timeToReachTarget;
        transform.position = Vector3.Lerp(startPosition, transform.parent.transform.position, t);
    }

    /*
    private void RandomMovement()
    {
        //To edit: instead of adding availabledDirections
        //Save each direction in their own var (east, west,...)
        //Check each direction and then send them as string
        //+ agent pos + zone states = agent state -> send


        //Reset lists of directions
        directions.Clear();
        availableDirections.Clear();
        //Add all directions to direction list
        directions.Add(GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY + 1).ToString() + ")")); //Add north
        directions.Add(GameObject.Find("(X: " + (posX + 1).ToString() + "Y: " + (posY).ToString() + ")")); //Add east
        directions.Add(GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY - 1).ToString() + ")")); //Add south
        directions.Add(GameObject.Find("(X: " + (posX - 1).ToString() + "Y: " + (posY).ToString() + ")")); //Add west
        //Add moveable directions to moveable directions list
        //We do this instead of removing directions to avoid modifying the directions list while iterating it
        foreach (GameObject direction in directions)
            if (direction != null && direction.GetComponent<Cell>().GetGround() != null)
                availableDirections.Add(direction);
        //get a random no depending on list length
        dirIndex = Random.Range(0, availableDirections.Count);
        //number corespond to selected direction
        t = 0;
        startPosition = transform.position;
        transform.SetParent(availableDirections[dirIndex].GetComponent<Cell>().GetGround());
        posX = availableDirections[dirIndex].GetComponent<Cell>().GetPosition().x;
        posY = availableDirections[dirIndex].GetComponent<Cell>().GetPosition().y;
    }
    */

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
        if (GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY).ToString() + ")").GetComponent<Cell>().GetZone() != null)
            operators.Add(GameObject.Find("(X: " + (posX).ToString() + "Y: " + (posY).ToString() + ")").GetComponent<Cell>().GetZone());

        return operators;
    }

    public int[] GetAgentState()
    {
        int[] state = new int[] {posX, posY, carrying};
        return state;
    }
}

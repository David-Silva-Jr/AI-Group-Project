using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private float t; //Divided value for moving between points
    private Vector3 startPosition; //3D position for moving in world
    private int posX;   //2D position as grid data
    private int posY;
    private List<GameObject> directions = new List<GameObject>(); //List of directions
    private List<GameObject> availableDirections = new List<GameObject>(); //List of moveable directions
    int dirIndex; //Index of selected direction
    private float timeToReachTarget;

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
        timeToReachTarget = timeSystem.GetMaxTickTimer()/2;

        startPosition = transform.position;
        //Subscribe to OnTick event
        TimeSystem.OnTick += OnTickHandler;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / timeToReachTarget;
        transform.position = Vector3.Lerp(startPosition, transform.parent.transform.position, t);
    }

    private void RandomMovement()
    {
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

    //Handles OnTick event
    private void OnTickHandler(object sender, TimeSystem.OnTickEventArgs e)
    {
        RandomMovement();
    }

    private void OnDestroy()
    {
        //Unsubscribe when destroyed
        TimeSystem.OnTick -= OnTickHandler;
    }
}

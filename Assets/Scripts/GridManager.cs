using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[On WorldManager]
//The grid manager manages everything on the grid as well as some UI elements to aid in managing grid objects
//Creating and deleting grid objects (agents, pickup/dropoff locations)
//Holds references to grid objects and can pass it to policy manager
public class GridManager : MonoBehaviour
{
    Grid grid;
    [SerializeField] private LayerMask cellLayer;
    [SerializeField] private GameObject pickupPopup;
    [SerializeField] private GameObject dropoffPopup;
    [SerializeField] private GameObject pickupInputField;
    [SerializeField] private GameObject dropoffInputField;

    private bool paused = true;
    private bool pauseTerminal = false;

    private  PickupZone[] pickupZones = new PickupZone[2]; //2 pickup zones
    private DropoffZone[] dropoffZones = new DropoffZone[4]; //4 dropoff zones
    private int currZone; //Index of current zone
    private Cell currentCell;
    private string placing = null;
    private Agent maleAgent;
    private Agent femaleAgent;

    //For checking terminal
    private int[] zoneState;
    private bool terminal;
    TimeSystem timeSystem;

    //Initial values
    private int[] pickupVal = new int[2]; //2 initial pickup vals
    private int[] dropoffVal = new int[4]; //4 initial dropoff vals
    private Cell femaleStartingCell;
    private Cell maleStartingCell;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
        timeSystem = FindObjectOfType<TimeSystem>();
    }

    private void Update()
    {
        currentCell = GetCurrentCell();

        //Can only edit while paused
        if (paused)
        {
            //Tell cell to drawGround
            if (Input.GetMouseButton(0) && currentCell != null)
                switch (placing)
                {
                    case "Ground":
                        currentCell.DrawGround();
                        break;
                    case "DeleteGround":
                        currentCell.DeleteGround();
                        break;
                    default:
                        break;
                }

            //Tell cell to placeAgent
            if (Input.GetMouseButtonDown(0) && currentCell != null)
                switch (placing)
                {
                    case "Agent":
                        if (femaleAgent == null)
                        {
                            femaleAgent = currentCell.PlaceAgent("female");
                            femaleStartingCell = currentCell;
                            //send to policymanager as well
                        }  
                        else if (maleAgent == null)
                        {
                            maleAgent = currentCell.PlaceAgent("male");
                            maleStartingCell = currentCell;
                        }
                        break;
                    case "DeleteAgent":
                        currentCell.DeleteAgent();
                        break;
                    case "Pickup":
                        for (int i = 0; i<2; i++)
                        {
                            if (pickupZones[i] == null) //If we can place a pickupZone
                            {
                                pickupZones[i] = currentCell.placePickupZone(); //place pickupZone
                                if (pickupZones[i] != null) //If what we just placed isn't null
                                {
                                    pickupPopup.SetActive(true);
                                    PauseMenu(); //Pause until PickupPopup is resolved
                                    currZone = i; //Set this value to i to be used in assigning nPickups
                                    break;
                                }
                            }
                        }                     
                        break;
                    case "Dropoff":
                        for (int i = 0; i < 4; i++)
                        {
                            if (dropoffZones[i] == null) //If we can place a dropoffZone
                            {
                                dropoffZones[i] = currentCell.placeDropoffZone(); //place dropoffZone
                                if (dropoffZones[i] != null) //If what we just placed isn't null
                                {
                                    dropoffPopup.SetActive(true);
                                    PauseMenu(); //Pause until dropoffPopup is resolved
                                    currZone = i; //Set this value to i to be used in assigning nDropoff
                                    break;
                                }
                            }
                        }
                        break;
                    case "DeleteZone":
                        currentCell.DeleteZone();
                        break;
                    default:
                        break;
                }
        }

        else //Not paused
        {
            //Checking for terminal state
            zoneState = GetZoneState();
            terminal = true;
            foreach (int zone in zoneState)
            {
                if (zone == 1)
                    terminal = false;
            }
            //Terminate, pause and reset
            if (terminal)
            {
                if (pauseTerminal)
                {
                    //Pause game
                    timeSystem.Pause();
                    //Unpause menu
                    PauseMenu();
                }
                //Increase termination count
                GetComponent<PolicyManager>().CountTerminal();
                //Reset pickup/dropoff
                for (int i = 0; i < 2; i++)
                    pickupZones[i].GetComponent<PickupZone>().SetnPickups(pickupVal[i]);

                for (int i = 0; i < 4; i++)
                    dropoffZones[i].GetComponent<DropoffZone>().SetnDropoffs(dropoffVal[i]);
                //Reset agent location
                maleAgent.MoveTo(maleStartingCell);
                femaleAgent.MoveTo(femaleStartingCell);
            }
        }
       

    }

    //Popup window asking user to input number of pickup objects for zone
    public void PickupPopup()
    {
        int nPickups = Int32.Parse(pickupInputField.GetComponent<Text>().text);
        pickupZones[currZone].GetComponent<PickupZone>().SetnPickups(nPickups);
        //Set initial val
        pickupVal[currZone] = nPickups;
        pickupPopup.SetActive(false);
        PauseMenu();
    }

    //Popup window asking user to input capacity for dropoff zone
    public void DropoffPopup()
    {
        int nDropoffs = Int32.Parse(dropoffInputField.GetComponent<Text>().text);
        dropoffZones[currZone].GetComponent<DropoffZone>().SetnDropoffs(nDropoffs);
        //Set initial val
        dropoffVal[currZone] = nDropoffs;
        dropoffPopup.SetActive(false);
        PauseMenu();
    }

    //Return the cell component of the clicked cell
    private Cell GetCurrentCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, cellLayer))
        {
            return hitInfo.transform.GetComponent<Cell>();
        }
        else return null;
    }

    //For the policy manager to get reference to agents
    public Agent GetAgent(string gender)
    {
        if (gender == "female")
            return femaleAgent;
        else return maleAgent;
    }

    //For the policy manger to get zone states
    public int[] GetZoneState()
    {
        int[] state = new int[] { pickupZones[0].GetState(), pickupZones[1].GetState(), dropoffZones[0].GetState(), dropoffZones[1].GetState(), dropoffZones[2].GetState(), dropoffZones[3].GetState() };
        return state;
    }

    //Get the object to place from GUIs
    public void Placement(string objectToPlace)
    {
        placing = objectToPlace;
    }

    //PauseMenu when window pops up
    public void PauseMenu()
    {
        paused = !paused;
    }

    public void pauseWhenTerminal()
    {
        pauseTerminal = !pauseTerminal;
    }
}

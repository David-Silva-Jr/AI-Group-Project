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

    private  PickupZone[] pickupZones = new PickupZone[2]; //2 pickup zones
    private DropoffZone[] dropoffZones = new DropoffZone[6]; //4 dropoff zones
    private int currZone; //Index of current zone
    private Cell currentCell;
    private string placing = null;
    private Agent maleAgent;
    private Agent femaleAgent;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
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
                            //send to policymanager as well
                        }  
                        else if (maleAgent == null)
                        {
                            maleAgent = currentCell.PlaceAgent("male");
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
                                    Pause(); //Pause until PickupPopup is resolved
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
                                    Pause(); //Pause until dropoffPopup is resolved
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
       
    }

    //Popup window asking user to input number of pickup objects for zone
    public void PickupPopup()
    {
        int nPickups = Int32.Parse(pickupInputField.GetComponent<Text>().text);
        pickupZones[currZone].GetComponent<PickupZone>().SetnPickups(nPickups);
        pickupPopup.SetActive(false);
        Pause();
    }

    //Popup window asking user to input capacity for dropoff zone
    public void DropoffPopup()
    {
        int nDropoffs = Int32.Parse(dropoffInputField.GetComponent<Text>().text);
        dropoffZones[currZone].GetComponent<DropoffZone>().SetnDropoffs(nDropoffs);
        dropoffPopup.SetActive(false);
        Pause();
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

    //Pause when window pops up
    public void Pause()
    {
        paused = !paused;
    }
}

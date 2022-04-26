using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//[On cells]
//The cell holds references to the grid objects on that cell (ground, agents, pickup/dropoff locations) 
//which can then be passed to the more general GridManager
//The cell also handles call from the GridManager to instantiate grid objects on that cell

public class Cell : MonoBehaviour
{
    private int posX;
    private int posY;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject heatmapPrefab;
    [SerializeField] private GameObject pickupZonePrefab;
    [SerializeField] private GameObject dropoffZonePrefab;
    [SerializeField] private GameObject maleAgentPrefab;
    [SerializeField] private GameObject femaleAgentPrefab;

    private Toggle heatmapToggle;

    private readonly GameObject[] cellFeatures = new GameObject[3]; //Cell has 2 features Ground (0), Dropoff/Pickup zone(1), Heatmaptile (2)
    private GameObject agent;


    private void Start()
    {
        heatmapToggle = GameObject.Find("Heatmap").GetComponent<Toggle>();
    }
    private void Update()
    {
        if (cellFeatures[2] != null)
        {
            if (cellFeatures[2].activeSelf != heatmapToggle.isOn)
                cellFeatures[2].SetActive(heatmapToggle.isOn);
        }
    }
    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }

    public Transform GetGround()
    {
        if (cellFeatures[0] != null) //Return ground if not null
            return cellFeatures[0].transform;
        else return null;
    }

    public string GetZoneOperator()
    {
        if (cellFeatures[1] == null) //Return null
            return null;
        else if (cellFeatures[1].name == "Pickup Zone(Clone)") //Return pickup if available
        {
            if (cellFeatures[1].GetComponent<PickupZone>().GetState() == 1)
                return "p";
        }

        else if (cellFeatures[1].GetComponent<DropoffZone>().GetState() == 1)
            return "d";

        return null;

    }

    public void ZoneAction(string action)
    {
        if (action == "p")
            cellFeatures[1].GetComponent<PickupZone>().PickUp();
        else 
            cellFeatures[1].GetComponent<DropoffZone>().Dropoff();
    }

    private void OnMouseOver()
    {
        HighlightCell();//Highlight cell red when mouse over
    }

    private void OnMouseExit()
    {
        UnhighlightCell();//Unhighlight to blue when mouse exit
    }


    private void HighlightCell()
    {
        Renderer[] renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.material.color = Color.red;
    }

    private void UnhighlightCell()
    {
        Renderer[] renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.material.color = Color.blue;
    }

    public void DrawGround()
    {
        if (cellFeatures[0] == null) //Draw ground if ground is null
        {
            cellFeatures[0] = Instantiate(groundPrefab, transform.GetChild(0).GetComponent<Renderer>().bounds.center + Vector3.down * 3, Quaternion.identity);
            cellFeatures[2] = Instantiate(heatmapPrefab, transform.GetChild(0).GetComponent<Renderer>().bounds.center + Vector3.down * 3 - Vector3.forward /10, Quaternion.identity);
            cellFeatures[2].SetActive(false);
        }
    }

    public void DeleteGround()
    {
        Destroy(cellFeatures[0]);
        Destroy(cellFeatures[1]);
        Destroy(cellFeatures[2]);
    }

    public Agent PlaceAgent(string gender)
    {
        if (cellFeatures[0] != null && cellFeatures[0].transform.childCount == 4) //Place agent if there is ground and there isn't already an agent on that space
        {
            if(gender == "male")
                //Create male agent set its parent to current ground and set it's position to current cell
                agent = Instantiate(maleAgentPrefab, cellFeatures[0].transform.position, Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right));
            if (gender == "female")
                //Create female agent set its parent to current ground and set it's position to current cell
                agent = Instantiate(femaleAgentPrefab, cellFeatures[0].transform.position, Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right));
            agent.transform.SetParent(cellFeatures[0].transform);
            agent.GetComponent<Agent>().SetPosition(posX, posY);
            return agent.GetComponent<Agent>();
        }
        return null;
    }

    public void DeleteAgent()
    {
        Destroy(cellFeatures[0].transform.GetChild(4).gameObject);
    }
    public PickupZone placePickupZone()
    {
        if (cellFeatures[0] != null && cellFeatures[1] == null) //Place zone if there's ground and no zone
        {
            cellFeatures[1] = Instantiate(pickupZonePrefab, transform.GetChild(0).GetComponent<Renderer>().bounds.center + Vector3.down * 3 + Vector3.back/5, Quaternion.identity);
            return cellFeatures[1].GetComponent<PickupZone>();
        }
        return null;
    }

    public DropoffZone placeDropoffZone()
    {
        if (cellFeatures[0] != null && cellFeatures[1] == null) //Place zone if there's ground and no zone
        {
            cellFeatures[1] = Instantiate(dropoffZonePrefab, transform.GetChild(0).GetComponent<Renderer>().bounds.center + Vector3.down * 3 + Vector3.back/5, Quaternion.identity);
            return cellFeatures[1].GetComponent<DropoffZone>(); ;
        }
        return null;
    }

    public void DeleteZone()
    {
        Destroy(cellFeatures[1]);
    }

    public void IncrementColor()
    {
        cellFeatures[2].GetComponent<Heatmap>().IncrementColor();
    }
}
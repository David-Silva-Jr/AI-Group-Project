using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private int posX;
    private int posY;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject agentPrefab;

    private readonly GameObject[] cellFeatures = new GameObject[2]; //Cell has 2 features Ground (0), Dropoff/Pickup zone(1)
    private GameObject agent;

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
        }
    }

    public void DeleteGround()
    {
        Destroy(cellFeatures[0]);
        Destroy(cellFeatures[1]);
    }

    public GameObject PlaceAgent()
    {
        if (cellFeatures[0] != null && cellFeatures[0].transform.childCount == 4) //Place agent if there is ground and there isn't already an agent on that space
        {
            //Create agent set its parent to current ground and set it's position to current cell
            agent = Instantiate(agentPrefab, cellFeatures[0].transform.position , Quaternion.identity * Quaternion.AngleAxis(90, Vector3.right));
            agent.transform.SetParent(cellFeatures[0].transform);
            agent.GetComponent<Agent>().SetPosition(posX, posY);
        }
        return null;
    }

    public void DeleteAgent()
    {
        Destroy(cellFeatures[0].transform.GetChild(4).gameObject);
    }
}
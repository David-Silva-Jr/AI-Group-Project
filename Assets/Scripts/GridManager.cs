using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    Grid grid;
    [SerializeField] private LayerMask cellLayer;

    private Cell currentCell;
    private string placing = null;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        currentCell = GetCurrentCell();

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
                    currentCell.PlaceAgent();
                    break;
                case "DeleteAgent":
                    currentCell.DeleteAgent();
                    break;
                case "Zone":
                    PlaceZone();
                    break;
                default:
                    break;
            }
       
    }

    private Cell GetCurrentCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, cellLayer))
        {
            return hitInfo.transform.GetComponent<Cell>();
        }
        else return null;
    }

    public void Placement(string objectToPlace)
    {
        placing = objectToPlace;
    }

    private void PlaceZone()
    {

    }
}

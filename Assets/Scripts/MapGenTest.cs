using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenTest : MonoBehaviour
{
    private List<Node> map = new List<Node>();
    [SerializeField] private int gridSize = 10;
    [SerializeField] private float cellSize = 1;

    [SerializeField] private GameObject cell;
    [SerializeField] private Vector3 origin = new Vector3(0,0,0);

    public AgentVisualizer fellaPrefab;
    private AgentVisualizer fella;

    private void Awake() {
        List<Node> nodeList = new List<Node>();

        if(cell == null){
            return;
        }

        Vector3 pos = origin;

        List<List<Node>> grid = new List<List<Node>>();

        for(int i = 0; i < gridSize; i++){
            List<Node> temp = new List<Node>();
            for(int j = 0; j < gridSize; j++){
                temp.Add(new Node(0));
                if(j > 0){
                    temp[j].AdjacentNodes.Add(temp[j-1]);
                    temp[j-1].AdjacentNodes.Add(temp[j]);
                }
                if(i > 0){
                    temp[j].AdjacentNodes.Add(grid[i-1][j]);
                    grid[i-1][j].AdjacentNodes.Add(temp[j]);
                }

                Instantiate(cell, origin + new Vector3(i,0,j), Quaternion.identity).name = "Cell " + (i*gridSize+j);

                map.Add(temp[j]);
            }
            grid.Add(temp);
        }

        fella = Instantiate(fellaPrefab, transform.position, Quaternion.identity);    
        Instantiate(fellaPrefab, transform.position, Quaternion.identity);
        Instantiate(fellaPrefab, transform.position, Quaternion.identity);    
    }

    public List<Node> Map{
        get {return map;}
    }
}

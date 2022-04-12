using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenTest : MonoBehaviour
{
    private List<Tile> map = new List<Tile>();
    [SerializeField] private int gridSize = 10;
    [SerializeField] private float cellSize = 1;

    [SerializeField] private GameObject cell;
    [SerializeField] private Vector3 origin = new Vector3(0,0,0);

    public AgentVisualizer fellaPrefab;
    private AgentVisualizer fella;

    private void Awake() {
        List<Tile> TileList = new List<Tile>();

        if(cell == null){
            return;
        }

        Vector3 pos = origin;

        List<List<Tile>> grid = new List<List<Tile>>();

        for(int i = 0; i < gridSize; i++){
            List<Tile> temp = new List<Tile>();
            for(int j = 0; j < gridSize; j++){
                temp.Add(new Tile(0));
                if(j > 0){
                    temp[j].AdjacentTiles.Add(temp[j-1]);
                    temp[j-1].AdjacentTiles.Add(temp[j]);
                }
                if(i > 0){
                    temp[j].AdjacentTiles.Add(grid[i-1][j]);
                    grid[i-1][j].AdjacentTiles.Add(temp[j]);
                }

                Instantiate(cell, origin + new Vector3(i,0,j), Quaternion.identity).name = "Cell " + (i*gridSize+j);

                map.Add(temp[j]);
            }
            grid.Add(temp);
        }

        fella = Instantiate(fellaPrefab, transform.position, Quaternion.identity);    
        Instantiate(fellaPrefab, transform.position, Quaternion.identity);
        Instantiate(fellaPrefab, transform.position, Quaternion.identity);    
        Instantiate(fellaPrefab, transform.position, Quaternion.identity);  
        Instantiate(fellaPrefab, transform.position, Quaternion.identity);  
        Instantiate(fellaPrefab, transform.position, Quaternion.identity);  
    }

    public List<Tile> Map{
        get {return map;}
    }
}

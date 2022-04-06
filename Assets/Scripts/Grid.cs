using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[On Grid]
//The Grid draws the entire grid (not just cells with ground)
public class Grid : MonoBehaviour
{
    [SerializeField] private int height;
    [SerializeField] private int width;
    private readonly float gridSpaceSize = 5f;

    [SerializeField] private GameObject cellPrefab;
    private GameObject[,] grid; //2d array

    private void Start()
    {
        CreateGrid(); //Create grid on start
    }

    private void CreateGrid()
    {

        grid = new GameObject[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x <width;x++)
            {
                grid[x, y] = Instantiate(cellPrefab, new Vector3(x * gridSpaceSize, y * gridSpaceSize), Quaternion.identity);
                grid[x, y].GetComponent<Cell>().SetPosition(x, y);
                grid[x, y].transform.parent = transform;
                grid[x, y].gameObject.name = "(X: " + x.ToString() +"Y: "+ y.ToString() + ")";
            }
        }
    }

    //Convert a world position to grid space
    public Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / gridSpaceSize);
        int y = Mathf.FloorToInt(worldPos.y / gridSpaceSize);

        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(x, 0, height);

        return new Vector2Int(x, y);
    }
}

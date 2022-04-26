using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    private int width;
    private int height;
    private List<List<Tile>> tiles;  // Actual map data is all tiles

    public Map(int _width, int _height){
        width = _width;
        height = _height;

        // Create map tiles
        tiles = new List<List<Tile>>();
        for(int r = 0; r < _height; r++){
            List<Tile> row = new List<Tile>();
            for(int c = 0; c < _width; c++){
                row.Add(new Tile(0));
            }
            tiles.Add(row);
        }
    }
    
    // The way the accessors are now, you can't directly add or remove tiles once the map is generated
    // I'm pretty sure you can replace tiles though
    public int Width{
        get{return width;}
    }

    public int Height{
        get{return height;}
    }

    public List<List<Tile>> World{
        get{return tiles;}
    }

    // Allows access to specific map tiles by MapObject[r, c]
    public Tile this[int r, int c]{
        get{return tiles[r][c];}
    }

    // Allows access to specific map tiles by vector2int
    public Tile this[Vector2Int pos]{
        get{return tiles[pos.x][pos.y];}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private Tile[,] tiles = new Tile[1, 1];
    // The position of the bottom left corner of the tiles array
    private Vector2Int arrayCorner = Vector2Int.zero;

    void Start()
    {
        foreach (Tile tile in GetComponentsInChildren<Tile>())
        {
            Vector2Int tilePosition = new Vector2Int(Mathf.RoundToInt(tile.transform.localPosition.x), Mathf.RoundToInt(tile.transform.localPosition.y));
            InsertIntoArray(tile, tilePosition);
        }

        //DebugArrayContents();
    }


    public Tile GetTileAtPosition(Vector2Int position)
    {

        if (IsOutsideArray(position)) return null;
        return tiles[position.x - arrayCorner.x, position.y - arrayCorner.y];
    }


    public void ClearArrayValue(Vector2Int tilePosition)
    {
        tiles[tilePosition.x - arrayCorner.x, tilePosition.y - arrayCorner.y] = null;
    }


    public void InsertIntoArray(Tile tile, Vector2Int tilePosition)
    {

        // if it's outside the grid
        if (IsOutsideArray(tilePosition))
        {
            // expand the grid
            ExpandArrayToIncludePosition(tilePosition);
            Debug.Log("array resized");
        }

        // insert it into the grid
        tiles[tilePosition.x - arrayCorner.x, tilePosition.y - arrayCorner.y] = tile;
    }


    private bool IsOutsideArray(Vector2Int position)
    {
        return
            position.x < arrayCorner.x ||
            position.y < arrayCorner.y ||
            position.x > arrayCorner.x + tiles.GetLength(0) - 1 ||
            position.y > arrayCorner.y + tiles.GetLength(1) - 1;
    }


    private void ExpandArrayToIncludePosition(Vector2Int position)
    {
        // Get the corner position and size of the new array
        Vector2Int newArrayCorner = arrayCorner;
        Vector2Int newArraySize = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));
        if (position.x < arrayCorner.x)
        {
            newArrayCorner.x = position.x;
            newArraySize.x += arrayCorner.x - newArrayCorner.x;
        }
        if (position.y < arrayCorner.y)
        {
            newArrayCorner.y = position.y;
            newArraySize.y += arrayCorner.y - newArrayCorner.y;
        }
        if (position.x > arrayCorner.x + tiles.GetLength(0) - 1)
        {
            newArraySize.x = position.x - newArrayCorner.x + 1;
        }
        if (position.y > arrayCorner.y + tiles.GetLength(0) - 1)
        {
            newArraySize.y = position.y - newArrayCorner.y + 1;
        }

        // Create the new array and populate it with the contents of the original one 
        Tile[,] newTiles = new Tile[newArraySize.x, newArraySize.y];
        for (int y = 0; y < tiles.GetLength(1); y++)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                newTiles[arrayCorner.x + x - newArrayCorner.x, arrayCorner.y + y - newArrayCorner.y] = tiles[x, y];
            }
        }

        tiles = newTiles;
        arrayCorner = newArrayCorner;
    }


    private void DebugArrayContents()
    {
        Debug.Log("Array size: " + new Vector2Int(tiles.GetLength(0), tiles.GetLength(1)));
        Debug.Log("Array corner: " + arrayCorner);

        for (int y = 0; y < tiles.GetLength(1); y++)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                Debug.Log(new Vector2Int(x, y) + ((tiles[x, y] == null) ? ": empty" : ": tile"));
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator 
{
    LevelSettings settings;

    GridTileType[,] grid;

    Vector2Int lastConnPoint;

    public void GenerateWorld(LevelSettings settings)
    {
        if(settings == null)
        {
            Debug.LogError("LevelSettings is null! Stopping generation");
            return;
        }

        this.settings = settings;
        grid = new GridTileType[settings.width, settings.height];

        for(int i = 0; i < 10; i++)
        {
            SpawnRoom(GetRandomRoom());
        }
    }


    void SpawnRoom(LevelData roomData)
    {
        if (roomData == null) return;
        int xOffset = 0;

        // find the entry point tile first
        for (int i = 0; i < roomData.data.Length; i++)
        {
            if ((roomData.data[i] == GridTileType.GeneratorStartPoint))
            {
                // xoffset is negative, since it needs a nudge to the left
                xOffset = -i;
                break;
            }
        }

        Vector2Int zeroPos = new Vector2Int(lastConnPoint.x - xOffset, lastConnPoint.y + 1);

        // tiled maps go from left to right, top to bottom, but i need to draw them left to right, bottom to top
        for (int i = 0; i < roomData.width; i++)
        {
            GridTileType tileType = roomData.data[i];
            Set(tileType, zeroPos.x + i % roomData.width, zeroPos.y + Mathf.FloorToInt(i / roomData.width));
        }

        // find the exit point tile and set the connpoint for next iteration
        for (int i = 0; i < roomData.data.Length; i++)
        {
            if ((roomData.data[i] == GridTileType.GeneratorEndPoint))
            {
                lastConnPoint = lastConnPoint + new Vector2Int(i, roomData.height);
                break;
            }
        }
    }

    LevelData GetRandomRoom()
    {
        int rnd = Random.Range(0, settings.rooms.Length-1);
        return settings.rooms[rnd];
    }

    /// <summary>
    /// Returns the tile at this position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public GridTileType Get(int x, int y)
    {
        Vector2Int pos = WrapPos(x, y);

        return grid[pos.x, pos.y];
    }

    /// <summary>
    /// Places a specific tile at the position. Use ID 0 to remove.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public void Set(GridTileType id, int x, int y)
    {
        Vector2Int truePos = WrapPos(x, y);

    }

    /// <summary>
    /// Wraps and corrects a given 2D point to a valid one for this level
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    Vector2Int WrapPos(int x, int y)
    {
        return new Vector2Int(Mathf.Clamp(x % (settings.width + 1), 0, settings.width),
            Mathf.Clamp(y, 0, settings.height));
    }
}

public enum GridTileType : byte
{
    Empty, Wall, Ladder, StairRight, StairLeft, LightSource, GeneratorStartPoint, GeneratorEndPoint, Chest
}


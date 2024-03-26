using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator 
{
    LevelSettings settings;

    GridTileType[,] grid;

    Vector2Int lastConnPoint;
    int lastRoomIndex;

    public void GenerateWorld(LevelSettings settings)
    {
        if(settings == null)
        {
            Debug.LogError("LevelSettings is null! Stopping generation");
            return;
        }

        this.settings = settings;
        grid = new GridTileType[settings.width, settings.height];

        for(int i = 0; i < settings.numberOfRooms; i++)
        {
            if (i == 0) 
            { 
                SpawnRoom(settings.firstRoom); 
            }
            else if (i == settings.numberOfRooms - 1) 
            { 
                SpawnRoom(settings.lastRoom); 
            }
            else
            {
                SpawnRoom(GetRandomRoom());
            }
        }
    }


    void SpawnRoom(LevelData roomData)
    {
        if (roomData == null) return;
        int xStartPos = 0;

        // find the entry point tile first
        for (int i = 0; i < roomData.data.Length; i++)
        {
            if ((roomData.data[i] == GridTileType.GeneratorStartPoint))
            {
                xStartPos = i % roomData.width;
                break;
            }
        }

        Vector2Int zeroPos = new Vector2Int(lastConnPoint.x - xStartPos, lastConnPoint.y + 1);
        //Debug.Log($"xStartPos {xStartPos}, lastConnPoint.x {lastConnPoint.x}, zeroPos.x {zeroPos.x}");
        int totalRoomArea = roomData.width * roomData.height;
        for (int i = 0; i < totalRoomArea; i++)
        {
            GridTileType tileType = roomData.data[i];
            int x = zeroPos.x + (i % roomData.width);
            int y = zeroPos.y + roomData.height - Mathf.FloorToInt(i / roomData.width);
            Set(tileType, x, y);
        }

        // find the exit point tile and set the connpoint for next iteration
        for (int i = 0; i < roomData.data.Length; i++)
        {
            if ((roomData.data[i] == GridTileType.GeneratorEndPoint))
            {
                lastConnPoint = new Vector2Int(i % roomData.width + zeroPos.x, roomData.height + lastConnPoint.y);
                break;
            }
        }

    }

    LevelData GetRandomRoom()
    {
        int rnd = 0;
        while(settings.rooms.Length > 1 && rnd == lastRoomIndex)
        {
            rnd = Random.Range(0, settings.rooms.Length);
        }

        lastRoomIndex = rnd;
        return settings.rooms[rnd];
    }

    public GridTileType Get (Vector2Int pos) => Get(pos.x, pos.y);
    /// <summary>
    /// Returns the tile at this position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public GridTileType Get(int x, int y)
    {
        Vector2Int pos = settings.WrapPos(x, y);

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
        Vector2Int truePos = settings.WrapPos(x, y);

        grid[truePos.x, truePos.y] = id;
    }

    
}

public enum GridTileType : byte
{
    Empty, Wall, Ladder, StairRight, StairLeft, LightSource, GeneratorStartPoint, GeneratorEndPoint, Chest

}

public static class GridTileTypeHelper
{
    public static bool IsTileEmpty(GridTileType type)
    {
        return (type == GridTileType.Empty || type == GridTileType.LightSource);
    }

    public static bool IsTileClimbable(GridTileType type)
    {
        return (type == GridTileType.Ladder || type == GridTileType.GeneratorEndPoint || type == GridTileType.GeneratorStartPoint);
    }

    public static bool IsTileSolid(GridTileType type)
    {
        return (type == GridTileType.Wall);
    }


}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator 
{
    LevelSettings settings;

    GridTileGeometry[,] grid;

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
        grid = new GridTileGeometry[settings.width, settings.height];

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
        if (roomData.TryGetGeneratorStartPoint(out Vector2Int startPos))
        {
            xStartPos = startPos.x;
        }

        Vector2Int zeroPos = new Vector2Int(lastConnPoint.x - xStartPos, lastConnPoint.y + 1);
        int totalRoomArea = roomData.width * roomData.height;

        // set to grid
        for (int i = 0; i < totalRoomArea; i++)
        {
            GridTileGeometry tileType = roomData.geometryData[i];
            int x = zeroPos.x + (i % roomData.width);
            int y = zeroPos.y + roomData.height - Mathf.FloorToInt(i / roomData.width);
            Set(tileType, x, y);
        }

        // find the exit point tile and set the connpoint for next iteration
        if(roomData.TryGetGeneratorEndPoint(out Vector2Int endPos))
        {
            lastConnPoint = new Vector2Int(endPos.x + zeroPos.x, endPos.y + lastConnPoint.y);
        }

    }

    LevelData GetRandomRoom()
    {
        int rnd = 0;
        while(settings.rooms.Length > 1 && rnd == lastRoomIndex)
        {
            rnd = UnityEngine.Random.Range(0, settings.rooms.Length);
        }

        lastRoomIndex = rnd;
        return settings.rooms[rnd];
    }

    public GridTileGeometry Get (Vector2Int pos) => Get(pos.x, pos.y);
    /// <summary>
    /// Returns the tile at this position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public GridTileGeometry Get(int x, int y)
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
    public void Set(GridTileGeometry id, int x, int y)
    {
        Vector2Int truePos = settings.WrapPos(x, y);

        grid[truePos.x, truePos.y] = id;
    }

    
}



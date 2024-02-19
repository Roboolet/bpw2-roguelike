using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator 
{
    GridTileType[,] grid;

    LevelSettings settings;

    public LevelGenerator()
    {
    }

    public void GenerateWorld(LevelSettings settings)
    {
        if(settings == null)
        {
            Debug.LogError("LevelSettings is null! Stopping generation");
            return;
        }

        this.settings = settings;
        grid = new GridTileType[settings.width, settings.height];
    }

    /// <summary>
    /// Places a specific tile at the position. Use ID 0 to remove.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public void Place(GridTileType id, int x, int y)
    {
    }

    /// <summary>
    /// Returns the tile at this position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public GridTileType Get(int x, int y)
    {
        (int, int) pos = WrapPos(x, y);

        return grid[pos.Item1, pos.Item2];
    }

    /// <summary>
    /// Wraps and corrects a given 2D point to a valid one for this level
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public (int,int) WrapPos(int x, int y)
    {
        return (Mathf.Clamp(x % (settings.width + 1), 0, settings.width),
            Mathf.Clamp(y, 0, settings.height));
    }
}

public enum GridTileType : byte
{
    Empty, Wall, Ladder, StairRight, StairLeft, LightSource, GeneratorStartPoint, GeneratorEndPoint, Chest
}


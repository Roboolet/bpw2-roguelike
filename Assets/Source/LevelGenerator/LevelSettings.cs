using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "LevelGen/LevelSettings", order = 0)]
public class LevelSettings : ScriptableObject
{
    public int width, height;
    public GameGridTileset tileset;
    public LevelData[] rooms;

    /// <summary>
    /// Wraps and corrects a given 2D point to a valid one for this level
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector2Int WrapPos(int x, int y)
    {
        // just a single modulus doesn't account for numbers below 0, so without this it would cause maps to infinitely repeat to the left
        int xWrapped = ((x % width) + width) % width;
        return new Vector2Int(xWrapped,
            Mathf.Clamp(y, 0, height - 1));
    }
}

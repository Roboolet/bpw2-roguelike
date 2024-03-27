using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData : ScriptableObject
{
    // fields set by the importer
    public int width;
    public int height;
    public GridTileGeometry[] geometryData;
    public GridTileSpawns[] spawnsData;
    public GridTileBackground[] backgroundData;

    public bool TryGetGeneratorEndPoint(out Vector2Int roomPosition)
    {
        roomPosition = Vector2Int.zero;

        for (int i = 0; i < geometryData.Length; i++)
        {
            if (geometryData[i] == GridTileGeometry.GeneratorEndPoint)
            {
                roomPosition = new Vector2Int(i % width, Mathf.FloorToInt(i / width));
                return true;
            }
        }

        return false;
    }

    public bool TryGetGeneratorStartPoint(out Vector2Int roomPosition)
    {
        roomPosition = Vector2Int.zero;

        for (int i = 0; i < geometryData.Length; i++)
        {
            if (geometryData[i] == GridTileGeometry.GeneratorStartPoint)
            {
                roomPosition = new Vector2Int(i % width, Mathf.FloorToInt(i / width));
                return true;
            }
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData : ScriptableObject
{
    public int width;
    public int height;
    public GridTileGeometry[] geometryData;
    public GridTileSpawns[] spawnsData;
    public GridTileBackground[] backgroundData;
}

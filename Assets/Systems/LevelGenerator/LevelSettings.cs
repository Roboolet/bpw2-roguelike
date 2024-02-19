using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "LevelGen/LevelSettings", order = 0)]
public class LevelSettings : ScriptableObject
{
    public int width, height;
    GameGridTileset tileset;
}

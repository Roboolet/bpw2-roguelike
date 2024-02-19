using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "GameGrid/Tileset", order = 0)]
public class GameGridTileset : ScriptableObject
{
    public Texture2D[] t_wall;
}

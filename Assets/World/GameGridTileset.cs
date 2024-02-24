using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "GameGrid/Tileset", order = 0)]
public class GameGridTileset : ScriptableObject
{
    const int textureSeed = 2003;
    public Sprite[] t_wall;

    public Sprite GetSpriteByTileType(GridTileType type, int x, int y)
    {
        float seedVal = Mathf.PerlinNoise(textureSeed + x, textureSeed + y);

        switch (type)
        {
            case GridTileType.Wall: return t_wall[Mathf.RoundToInt(seedVal * (t_wall.Length - 1))];

            default: return null;
        }
    }
}

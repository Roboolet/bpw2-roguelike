using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "GameGrid/Tileset", order = 0)]
public class GameGridTileset : ScriptableObject
{
    const int textureSeed = 2003;
    public Sprite[] t_wall, t_ladder, t_stairRight, t_stairLeft, t_light, t_chest;

    public Sprite GetSpriteByTileType(GridTileType type, int x, int y)
    {
        float seedVal = Mathf.PerlinNoise(textureSeed + x, textureSeed + y);

        switch (type)
        {
            case GridTileType.Wall: return t_wall[Mathf.RoundToInt(seedVal * (t_wall.Length - 1))];

            case GridTileType.GeneratorStartPoint:
            case GridTileType.GeneratorEndPoint:
            case GridTileType.Ladder: return t_ladder[Mathf.RoundToInt(seedVal * (t_ladder.Length - 1))];

            case GridTileType.StairRight: return t_stairRight[Mathf.RoundToInt(seedVal * (t_stairRight.Length - 1))];

            case GridTileType.StairLeft: return t_stairLeft[Mathf.RoundToInt(seedVal * (t_stairLeft.Length - 1))];

            case GridTileType.LightSource: return t_light[Mathf.RoundToInt(seedVal * (t_light.Length - 1))];

            case GridTileType.Chest: return t_chest[Mathf.RoundToInt(seedVal * (t_chest.Length - 1))];

            default: return null;
        }
    }
}

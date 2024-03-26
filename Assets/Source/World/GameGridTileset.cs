using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "GameGrid/Tileset", order = 0)]
public class GameGridTileset : ScriptableObject
{
    const int textureSeed = 2003;
    public Sprite[] t_wall, t_ladder, t_stairRight, t_stairLeft, t_light, t_chest;

    public Sprite GetSpriteByTileType(GridTileGeometry type, float x, float y)
    {
        float seedVal = Mathf.PerlinNoise(textureSeed + x * 0.1f, textureSeed + y * 0.1f);

        switch (type)
        {
            case GridTileGeometry.Wall: return t_wall[Mathf.RoundToInt(seedVal * (t_wall.Length - 1))];

            case GridTileGeometry.GeneratorStartPoint:
            case GridTileGeometry.GeneratorEndPoint:
            case GridTileGeometry.Ladder: return t_ladder[Mathf.RoundToInt(seedVal * (t_ladder.Length - 1))];

            case GridTileGeometry.StairRight: return t_stairRight[Mathf.RoundToInt(seedVal * (t_stairRight.Length - 1))];

            case GridTileGeometry.StairLeft: return t_stairLeft[Mathf.RoundToInt(seedVal * (t_stairLeft.Length - 1))];

            //case GridTileGeometry.LightSource: return t_light[Mathf.RoundToInt(seedVal * (t_light.Length - 1))];

            //case GridTileGeometry.Chest: return t_chest[Mathf.RoundToInt(seedVal * (t_chest.Length - 1))];

            default: return null;
        }
    }
}

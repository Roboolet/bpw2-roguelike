using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArcher : GridEntity
{
    [SerializeField] int shootRange;

    protected override void BeforeEvaluation(int turnNumber)
    {
        Vector2Int playerPos = entityManager.playerEntityReference.gridPosition;

        if (playerPos.x < gridPosition.x)
        {
            if (playerPos.x > gridPosition.x - shootRange)
            {
                selectedEntityActionPreset = EntityActionPreset.AttackLeft;
            }
            else if(!GridTileTypeHelper.IsTileEmpty(adjacentTiles.leftUnder))
            {
                selectedEntityActionPreset = EntityActionPreset.MoveLeft;
            }
        }
        else if (playerPos.x > gridPosition.x)
        {
            if (playerPos.x < gridPosition.x + shootRange)
            {
                selectedEntityActionPreset = EntityActionPreset.AttackRight;
            }
            else if (!GridTileTypeHelper.IsTileEmpty(adjacentTiles.rightUnder))
            {
                selectedEntityActionPreset = EntityActionPreset.MoveRight;
            }
        }
    }
}

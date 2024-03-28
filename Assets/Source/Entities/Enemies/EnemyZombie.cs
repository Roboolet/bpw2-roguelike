using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombie : GridEntity
{

    protected override void BeforeEvaluation(int turnNumber)
    {
        Vector2Int playerPos = entityManager.playerEntityReference.gridPosition;

        if(playerPos.y > gridPosition.y - 3 && GridTileTypeHelper.IsTileClimbable(adjacentTiles.current))
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUp;
        }
        else if (playerPos.x < gridPosition.x)
        {
            if(playerPos == gridPosition + Vector2Int.left)
            {
                selectedEntityActionPreset = EntityActionPreset.AttackLeft;
            }
            else
            {
                selectedEntityActionPreset = EntityActionPreset.MoveLeft;
            }
        }
        else if (playerPos.x > gridPosition.x)
        {
            if (playerPos == gridPosition + Vector2Int.right)
            {
                selectedEntityActionPreset = EntityActionPreset.AttackRight;
            }
            else
            {
                selectedEntityActionPreset = EntityActionPreset.MoveRight;
            }
        }
    }
}

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
            SetActionPreset(EntityActionPreset.MoveUp);
        }
        else if (playerPos.x < gridPosition.x)
        {
            if(playerPos == gridPosition + Vector2Int.left)
            {
                SetActionPreset(EntityActionPreset.AttackLeft);
            }
            else
            {
                SetActionPreset(EntityActionPreset.MoveLeft);
            }
        }
        else if (playerPos.x > gridPosition.x)
        {
            if (playerPos == gridPosition + Vector2Int.right)
            {
                SetActionPreset(EntityActionPreset.AttackRight);
            }
            else
            {
                SetActionPreset(EntityActionPreset.MoveRight);
            }
        }
    }
}

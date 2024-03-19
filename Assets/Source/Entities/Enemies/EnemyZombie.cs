using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombie : GridEntity
{

    protected override void BeforeEvaluation(int turnNumber)
    {
        Vector2Int playerPos = entityManager.playerEntityReference.gridPosition;

        if (playerPos.x < gridPosition.x)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveLeft;
        }
        else if (playerPos.x > gridPosition.x)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveRight;
        }
    }
}

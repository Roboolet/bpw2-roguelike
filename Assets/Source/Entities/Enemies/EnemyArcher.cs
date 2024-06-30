using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArcher : GridEntity
{
    [SerializeField] int shootRange;

    protected override void BeforeEvaluation(int turnNumber)
    {
        Vector2Int playerPos = entityManager.playerEntityReference.gridPosition;

        // out of range
        if (playerPos.y > gridPosition.y + 2 || playerPos.y < gridPosition.y - 2) return;

        if (playerPos.x < gridPosition.x)
        {
            if (playerPos.x >= gridPosition.x - shootRange)
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
            if (playerPos.x <= gridPosition.x + shootRange)
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

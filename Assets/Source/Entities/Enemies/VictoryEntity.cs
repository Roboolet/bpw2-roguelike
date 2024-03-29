using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryEntity : GridEntity
{
    protected override void BeforeEvaluation(int turnNumber)
    {
        if (entityManager.playerEntityReference.gridPosition.y > gridPosition.y - 2)
        {
            GameStateManager.instance.ChangeGameState(GameState.Victory);
        }
    }
}

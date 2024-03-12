using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : GridEntity
{
    PlayerTurnActionType selectedActionType;

    // has to use ID to be compatible with inspector buttons
    public void SetActionType(int id)
    {
        selectedActionType = (PlayerTurnActionType)id;
    }

    public override TurnAction EvaluateNextAction(int turnNumber)
    {
        // TODO: check if the move is actually possible
        // for instance: you cannot move up without a ladder

        switch (selectedActionType)
        {
            default: return TurnAction.CreateIdleAction(this, turnNumber);

            case PlayerTurnActionType.MoveUp: 
                return TurnAction.CreateMoveAction(this, Vector2Int.up, turnNumber, priority: -1);
            case PlayerTurnActionType.MoveLeft: 
                return TurnAction.CreateMoveAction(this, Vector2Int.left, turnNumber, priority: -1);
            case PlayerTurnActionType.MoveDown: 
                return TurnAction.CreateMoveAction(this, Vector2Int.down, turnNumber, priority: -1);
            case PlayerTurnActionType.MoveRight: 
                return TurnAction.CreateMoveAction(this, Vector2Int.right, turnNumber, priority: -1);
        }

    }

    public override void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    public enum PlayerTurnActionType
    {
        Idle = 0,
        MoveUp = 1, MoveLeft = 2, MoveDown = 3, MoveRight = 4,
        AttackUp = 5, AttackLeft = 6, AttackDown = 7, AttackRight = 8,
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    public Vector2Int gridPosition;
    public int health;

    public virtual TurnAction EvaluateNextAction(int turnNumber)
    {
        return TurnAction.CreateIdleAction(this, turnNumber + 1);
    }
    
}

public struct TurnAction
{
    public int priority;            // lower priority is executed first
    public int executionTurn;       // the turn when this action should be executed
    public int cooldown;            // how many turns to wait before being able to do another action
    public TurnActionType type;
    public Vector2Int[] targets;
    public GridEntity caster;

    public static TurnAction CreateMoveAction(GridEntity caster, Vector2Int positionOffset, int untilTurn, int priority = 0)
    {
        TurnAction newAction = new TurnAction();
        newAction.type = TurnActionType.Move;
        newAction.priority = priority;
        newAction.caster = caster;
        newAction.executionTurn = untilTurn;
        newAction.targets = new Vector2Int[1] { caster.gridPosition + positionOffset };

        return newAction;
    }

    public static TurnAction CreateIdleAction(GridEntity caster, int untilTurn)
    {
        TurnAction newAction = new TurnAction();
        newAction.type = TurnActionType.Idle;
        newAction.executionTurn = untilTurn;
        newAction.caster = caster;

        return newAction;
    }
}

public enum TurnActionType
{
    Idle, Move, Attack
}

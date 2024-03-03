using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    public int health;

    public virtual TurnAction EvaluateTurn(int turnNumber)
    {
        return TurnAction.CreateIdleAction(this, turnNumber + 1);
    }
    
}

public struct TurnAction
{
    public int priority;
    // the turn when this action should be executed
    public int executionTurn;
    public TurnActionType type;
    public Vector2Int[] targets;
    public GridEntity caster;

    public static TurnAction CreateMoveAction(GridEntity caster, Vector2Int newPosition, int untilTurn, int priority = 0)
    {
        TurnAction newAction = new TurnAction();
        newAction.type = TurnActionType.Move;
        newAction.priority = priority;
        newAction.caster = caster;
        newAction.executionTurn = untilTurn;
        newAction.targets = new Vector2Int[1] { newPosition };

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridEntity : MonoBehaviour
{
    public EntityActionPreset selectedEntityActionPreset;
    public Vector2Int gridPosition;

    [Header("Stats")]
    public int health;

    [Header("Entity Properties")]
    public bool canFly;
    public int baseTurnDelay;
    public int basePriority;

    public virtual TurnAction EvaluateNextAction(int turnNumber)
    {
        GridTileType t_this = GameGrid.instance.GetTileAtGridPosition(gridPosition);
        GridTileType t_under = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.down);
        GridTileType t_above = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.up);
        GridTileType t_left = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.left);
        GridTileType t_right = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.right);

        // you cannot walk into walls, set preset to none/idle
        if ((selectedEntityActionPreset == EntityActionPreset.MoveDown && t_under == GridTileType.Wall) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveUp && t_above == GridTileType.Wall) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveLeft && t_left == GridTileType.Wall) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveRight && t_right == GridTileType.Wall))
        {
            selectedEntityActionPreset = EntityActionPreset.None;
        }

        // walk up stairs
        // this does not account for having a wall directly above a stair... surely that will never happen
        else if(selectedEntityActionPreset == EntityActionPreset.MoveLeft && t_left == GridTileType.Wall)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUpLeft;
        }
        else if(selectedEntityActionPreset == EntityActionPreset.MoveRight && t_right == GridTileType.Wall)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUpRight;
        }

        if (!canFly)
        {
            // if no ladder here and tries to move up, set preset to none
            if (selectedEntityActionPreset == EntityActionPreset.MoveUp
                && (t_this == GridTileType.Ladder || t_above == GridTileType.GeneratorEndPoint || t_above == GridTileType.GeneratorStartPoint))
            {
                selectedEntityActionPreset = EntityActionPreset.None;
            }

            // if no floor underneath entity, force it to fall down
            // this step has to happen last
            if (t_under == GridTileType.Empty || t_under == GridTileType.LightSource)
            {
                selectedEntityActionPreset = EntityActionPreset.MoveDown;
            }

        }

        switch (selectedEntityActionPreset)
        {
            default: return EvaluateNonPresetAction(turnNumber);

            case EntityActionPreset.MoveUp:
                return TurnAction.CreateMoveAction(this, Vector2Int.up, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveLeft:
                return TurnAction.CreateMoveAction(this, Vector2Int.left, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveDown:
                return TurnAction.CreateMoveAction(this, Vector2Int.down, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveRight:
                return TurnAction.CreateMoveAction(this, Vector2Int.right, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveUpLeft:
                return TurnAction.CreateMoveAction(this, Vector2Int.left + Vector2Int.up, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveUpRight:
                return TurnAction.CreateMoveAction(this, Vector2Int.right + Vector2Int.up, turnNumber + baseTurnDelay, priority: basePriority);

        }
    }

    protected virtual TurnAction EvaluateNonPresetAction(int turnNumber)
    {
        return TurnAction.CreateIdleAction(this, turnNumber);
    }

    public virtual void OnDeath()
    {
        // drop loot
    }
    
}

public struct TurnAction
{
    public int priority;            // lower priority is executed first
    public int executionTurn;       // the turn when this action should be executed
    public int cooldown;            // how many turns to wait before being able to do another action
    public TurnActionType type;
    public Vector2Int[] values;
    public GridEntity caster;

    public static TurnAction CreateMoveAction(GridEntity caster, Vector2Int positionOffset, int untilTurn, int priority = 0)
    {
        TurnAction newAction = new TurnAction();
        newAction.type = TurnActionType.Move;
        newAction.priority = priority;
        newAction.caster = caster;
        newAction.executionTurn = untilTurn;
        newAction.values = new Vector2Int[1] { positionOffset };

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

public enum EntityActionPreset
{
    None = 0,
    MoveUp = 1, MoveLeft = 2, MoveDown = 3, MoveRight = 4,
    AttackUp = 5, AttackLeft = 6, AttackDown = 7, AttackRight = 8,

    // used with stairs
    MoveUpLeft = 9, MoveUpRight = 10
}


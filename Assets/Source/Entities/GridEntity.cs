using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridEntity : MonoBehaviour
{
    public EntityActionPreset selectedEntityActionPreset;
    public Vector2Int gridPosition;
    [HideInInspector] public EntityManager entityManager;

    [Header("Stats")]
    public int health;

    [Header("Entity Properties")]
    public bool canFly;
    public int baseTurnDelay;
    public int basePriority;

    /// <summary>
    /// Run behaviour here
    /// </summary>
    /// <param name="turnNumber"></param>
    protected virtual void BeforeEvaluation(int turnNumber)
    {

    }

    public TurnAction EvaluateNextAction(int turnNumber)
    {
        BeforeEvaluation(turnNumber);

        GridTileType t_this = GameGrid.instance.GetTileAtGridPosition(gridPosition);
        GridTileType t_under = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.down);
        GridTileType t_above = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.up);
        GridTileType t_left = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.left);
        GridTileType t_right = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.right);
        GridTileType t_leftUnder = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.left + Vector2Int.down);
        GridTileType t_rightUnder = GameGrid.instance.GetTileAtGridPosition(gridPosition + Vector2Int.right + Vector2Int.down);

        // you cannot walk into walls, set preset to none/idle
        if ((selectedEntityActionPreset == EntityActionPreset.MoveDown && GridTileTypeHelper.IsTileSolid(t_under)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveUp && GridTileTypeHelper.IsTileSolid(t_above)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveLeft && GridTileTypeHelper.IsTileSolid(t_left)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveRight && GridTileTypeHelper.IsTileSolid(t_right)))
        {
            selectedEntityActionPreset = EntityActionPreset.None;
        }

        // walk up stairs
        // this does not account for having a wall directly above a stair... surely that will never happen
        else if(selectedEntityActionPreset == EntityActionPreset.MoveLeft && t_left == GridTileType.StairLeft)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUpLeft;
        }
        else if(selectedEntityActionPreset == EntityActionPreset.MoveRight && t_right == GridTileType.StairRight)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUpRight;
        }
        else if (selectedEntityActionPreset == EntityActionPreset.MoveLeft && GridTileTypeHelper.IsTileEmpty(t_leftUnder))
        {
            selectedEntityActionPreset = EntityActionPreset.MoveDownLeft;
        }
        else if (selectedEntityActionPreset == EntityActionPreset.MoveRight && GridTileTypeHelper.IsTileEmpty(t_rightUnder))
        {
            selectedEntityActionPreset = EntityActionPreset.MoveDownRight;
        }

        if (!canFly)
        {
            // if no floor underneath entity and not on ladder, force it to fall down
            // this step has to happen last
            if (GridTileTypeHelper.IsTileEmpty(t_under) && !GridTileTypeHelper.IsTileClimbable(t_this))
            {
                selectedEntityActionPreset = EntityActionPreset.MoveDown;
            }

        }

        // create the move action
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

            // diagonal movement
            case EntityActionPreset.MoveUpLeft:
                return TurnAction.CreateMoveAction(this, Vector2Int.left + Vector2Int.up, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveUpRight:
                return TurnAction.CreateMoveAction(this, Vector2Int.right + Vector2Int.up, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveDownLeft:
                return TurnAction.CreateMoveAction(this, Vector2Int.left + Vector2Int.down, turnNumber + baseTurnDelay, priority: basePriority);
            case EntityActionPreset.MoveDownRight:
                return TurnAction.CreateMoveAction(this, Vector2Int.right + Vector2Int.down, turnNumber + baseTurnDelay, priority: basePriority);

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

    // used with stairs and falling
    MoveUpLeft = 9, MoveUpRight = 10, MoveDownLeft = 11, MoveDownRight = 12
}


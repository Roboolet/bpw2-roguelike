using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridEntity : MonoBehaviour
{
    public EntityActionPreset selectedEntityActionPreset;
    public Vector2Int gridPosition;
    [HideInInspector] public EntityManager entityManager;
    [Header("Equipment")]
    public Weapon weapon;

    [Header("Stats")]
    public int health;

    [Header("Entity Properties")]
    public bool canFly;
    public int baseTurnDelay;
    public int basePriority;

    // adjacent tiles
    protected AdjacentTiles adjacentTiles;

    /// <summary>
    /// Run behaviour here
    /// </summary>
    /// <param name="turnNumber"></param>
    protected virtual void BeforeEvaluation(int turnNumber)
    {
    }

    public TurnAction EvaluateNextAction(int turnNumber)
    {
        adjacentTiles = new AdjacentTiles(GameGrid.instance, gridPosition);

        BeforeEvaluation(turnNumber);

        // you cannot walk into walls, set preset to none/idle
        if ((selectedEntityActionPreset == EntityActionPreset.MoveDown && GridTileTypeHelper.IsTileSolid(adjacentTiles.under)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveUp && GridTileTypeHelper.IsTileSolid(adjacentTiles.above)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveLeft && GridTileTypeHelper.IsTileSolid(adjacentTiles.left)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveRight && GridTileTypeHelper.IsTileSolid(adjacentTiles.right)))
        {
            selectedEntityActionPreset = EntityActionPreset.None;
        }

        // walk up stairs
        // this does not account for having a wall directly above a stair... surely that will never happen
        else if(selectedEntityActionPreset == EntityActionPreset.MoveLeft && adjacentTiles.left == GridTileGeometry.StairLeft)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUpLeft;
        }
        else if(selectedEntityActionPreset == EntityActionPreset.MoveRight && adjacentTiles.right == GridTileGeometry.StairRight)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUpRight;
        }
        else if (selectedEntityActionPreset == EntityActionPreset.MoveLeft && GridTileTypeHelper.IsTileEmpty(adjacentTiles.leftUnder))
        {
            selectedEntityActionPreset = EntityActionPreset.MoveDownLeft;
        }
        else if (selectedEntityActionPreset == EntityActionPreset.MoveRight && GridTileTypeHelper.IsTileEmpty(adjacentTiles.rightUnder))
        {
            selectedEntityActionPreset = EntityActionPreset.MoveDownRight;
        }

        if (!canFly)
        {
            // if no floor underneath entity and not on ladder, force it to fall down
            // this step has to happen last so it can overwrite other moves
            if (GridTileTypeHelper.IsTileEmpty(adjacentTiles.under) && !GridTileTypeHelper.IsTileClimbable(adjacentTiles.current))
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
    
    protected struct AdjacentTiles
    {
        public GridTileGeometry current, under, above, left, right, leftAbove, rightAbove, leftUnder, rightUnder;

        public AdjacentTiles(GameGrid gameGridInstance, Vector2Int gridPosition)
        {
            current = gameGridInstance.GetGeometryAtGridPosition(gridPosition);
            under = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.down);
            above = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.up);
            left = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.left);
            right = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.right);
            leftAbove = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.left + Vector2Int.up);
            rightAbove = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.right + Vector2Int.up);
            leftUnder = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.left + Vector2Int.down);
            rightUnder = gameGridInstance.GetGeometryAtGridPosition(gridPosition + Vector2Int.right + Vector2Int.down);
        }
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


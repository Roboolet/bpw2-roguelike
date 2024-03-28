using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridEntity : MonoBehaviour
{
    [HideInInspector] public EntityManager entityManager;

    public EntityActionPreset selectedEntityActionPreset;
    public Vector2Int gridPosition;
    [Header("Equipment")]
    public Weapon weapon;

    [Header("Stats")]
    public int health;

    [Header("Entity Properties")]
    public bool canFly;
    public int baseTurnDelay;
    public int basePriority;

    protected AdjacentTiles adjacentTiles;
    protected int cooldownTurns;

    /// <summary>
    /// Run behaviour here
    /// </summary>
    /// <param name="turnNumber"></param>
    protected virtual void BeforeEvaluation(int turnNumber)
    {
    }

    public TurnAction EvaluateNextAction(int turnNumber)
    {
        if(cooldownTurns > 0)
        {
            cooldownTurns--;
            return TurnAction.CreateIdleAction(this);
        }

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

        int waitUntilTurn = turnNumber + baseTurnDelay;
        // create the move action
        switch (selectedEntityActionPreset)
        {
            default: return EvaluateNonPresetAction(turnNumber);

            case EntityActionPreset.MoveUp:
                return TurnAction.CreateMoveAction(this, Vector2Int.up, waitUntilTurn, basePriority);
            case EntityActionPreset.MoveLeft:
                return TurnAction.CreateMoveAction(this, Vector2Int.left, waitUntilTurn, basePriority);
            case EntityActionPreset.MoveDown:
                return TurnAction.CreateMoveAction(this, Vector2Int.down, waitUntilTurn, basePriority);
            case EntityActionPreset.MoveRight:
                return TurnAction.CreateMoveAction(this, Vector2Int.right, waitUntilTurn, basePriority);

            // diagonal movement
            case EntityActionPreset.MoveUpLeft:
                return TurnAction.CreateMoveAction(this, Vector2Int.left + Vector2Int.up, waitUntilTurn, basePriority);
            case EntityActionPreset.MoveUpRight:
                return TurnAction.CreateMoveAction(this, Vector2Int.right + Vector2Int.up, waitUntilTurn, basePriority);
            case EntityActionPreset.MoveDownLeft:
                return TurnAction.CreateMoveAction(this, Vector2Int.left + Vector2Int.down, waitUntilTurn, basePriority);
            case EntityActionPreset.MoveDownRight:
                return TurnAction.CreateMoveAction(this, Vector2Int.right + Vector2Int.down, waitUntilTurn, basePriority);

            // attacks
            case EntityActionPreset.AttackRight:
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, waitUntilTurn, false, false, basePriority);
            case EntityActionPreset.AttackLeft:
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, waitUntilTurn, true, false, basePriority);
            case EntityActionPreset.AttackUp:
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, waitUntilTurn, false, true, basePriority);
            case EntityActionPreset.AttackDown:
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, waitUntilTurn, true, true, basePriority);

        }
    }

    protected virtual TurnAction EvaluateNonPresetAction(int turnNumber)
    {
        return TurnAction.CreateIdleAction(this);
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
    public int deletionTimestamp;   // the timestamp when the EntityManager can safely delete this turnAction
    public TurnActionMove move;
    public TurnActionAttack[] attacks;
    public bool enableMove, enableAttack;
    public GridEntity caster;

    public static TurnAction CreateMoveAction(GridEntity caster, Vector2Int positionOffset, int waitUntilTurn, int priority = 0)
    {
        TurnAction newAction = new TurnAction();
        newAction.priority = priority;
        newAction.caster = caster;
        newAction.move = new TurnActionMove(caster.gridPosition + positionOffset, waitUntilTurn);
        newAction.enableMove = true;

        return newAction;
    }

    public static TurnAction CreateIdleAction(GridEntity caster)
    {
        TurnAction newAction = new TurnAction();
        newAction.caster = caster;

        return newAction;
    }

    public static TurnAction CreateAttackAction(GridEntity caster, int waitUntilTurn, bool mirror, bool swizzle, int priority = 0)
    {
        TurnAction newAction = new TurnAction();
        newAction.caster = caster;
        newAction.priority = priority;

        List<TurnActionAttack> attackPoints = new List<TurnActionAttack>();
        for(int i = 0; i < caster.weapon.attackData.Length; i++)
        {
            Weapon.WeaponAttackData data = caster.weapon.attackData[i];
            Vector2Int pos = data.offsetFromUser;
            if (swizzle)
            {
                (pos.x, pos.y) = (pos.y, pos.x);
            }
            if (mirror) 
            { 
                pos.x *= -1; 
                pos.y *= -1; 
            }

            TurnActionAttack attack = new TurnActionAttack(caster.gridPosition + pos, waitUntilTurn + data.startupDelay, data.damage);
            attackPoints.Add(attack);
        }
        newAction.attacks = attackPoints.ToArray();
        newAction.enableAttack = true;

        return newAction;
    }

    public void CalculateDeletionTimestamp()
    {
        int highest = move.executionTurn;

        for(int i = 0; i < attacks.Length; i++)
        {
            int t = attacks[i].executionTurn;
            if (t > highest) highest = t;
        }

        deletionTimestamp = highest+1;
    }

    public struct TurnActionAttack
    {
        public Vector2Int gridPosition;
        public int executionTurn; // the turn when this action should be executed
        public int damage; // used for damage

        public TurnActionAttack(Vector2Int gridPosition, int executionTurn, int damage)
        {
            this.gridPosition = gridPosition;
            this.executionTurn = executionTurn;
            this.damage = damage;
        }
    }
    public struct TurnActionMove
    {
        public Vector2Int gridPosition;
        public int executionTurn; // the turn when this action should be executed

        public TurnActionMove(Vector2Int gridPosition, int executionTurn)
        {
            this.gridPosition = gridPosition;
            this.executionTurn = executionTurn;
        }
    }
}


public enum EntityActionPreset
{
    None = 0,
    MoveUp = 1, MoveLeft = 2, MoveDown = 3, MoveRight = 4,
    AttackUp = 5, AttackLeft = 6, AttackDown = 7, AttackRight = 8,

    // used with stairs and falling
    MoveUpLeft = 9, MoveUpRight = 10, MoveDownLeft = 11, MoveDownRight = 12
}


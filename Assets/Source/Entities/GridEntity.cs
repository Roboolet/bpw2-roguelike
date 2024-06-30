using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridEntity : MonoBehaviour
{
    [HideInInspector] public EntityManager entityManager;

    EntityActionPreset selectedEntityActionPreset;
    bool moveIsForced = false;
    public Vector2Int gridPosition;
    [Header("Equipment")]
    public Weapon weapon;    

    [Header("Entity Properties")]
    [SerializeField] protected int health;
    [SerializeField] protected int beforeMoveDelay;
    public bool canFly;
    public int basePriority;

    protected AdjacentTiles adjacentTiles;
    protected int cooldownTurns;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        if(TryGetComponent(out SpriteRenderer r))
        {
            spriteRenderer = r;
        }
    }

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

        BeforeEvaluation(turnNumber);  
        // this is to keep some enemy logic from breaking
        SetActionPreset(selectedEntityActionPreset);

        int executionTurn = turnNumber;
        if (!moveIsForced)
        {
            executionTurn += beforeMoveDelay;
        }
        // create the move action
        switch (selectedEntityActionPreset)
        {
            default: return EvaluateNonPresetAction(turnNumber);

            case EntityActionPreset.MoveUp:
                return TurnAction.CreateMoveAction(this, Vector2Int.up, executionTurn, basePriority);
            case EntityActionPreset.MoveLeft:
                spriteRenderer.flipX = true;
                return TurnAction.CreateMoveAction(this, Vector2Int.left, executionTurn, basePriority);
            case EntityActionPreset.MoveDown:
                return TurnAction.CreateMoveAction(this, Vector2Int.down, executionTurn, basePriority);
            case EntityActionPreset.MoveRight:
                spriteRenderer.flipX = false;
                return TurnAction.CreateMoveAction(this, Vector2Int.right, executionTurn, basePriority);

            // diagonal movement
            case EntityActionPreset.MoveUpLeft:
                spriteRenderer.flipX = true;
                return TurnAction.CreateMoveAction(this, Vector2Int.left + Vector2Int.up, executionTurn, basePriority);
            case EntityActionPreset.MoveUpRight:
                spriteRenderer.flipX = false;
                return TurnAction.CreateMoveAction(this, Vector2Int.right + Vector2Int.up, executionTurn, basePriority);
            case EntityActionPreset.MoveDownLeft:
                spriteRenderer.flipX = true;
                return TurnAction.CreateMoveAction(this, Vector2Int.left + Vector2Int.down, executionTurn, basePriority);
            case EntityActionPreset.MoveDownRight:
                spriteRenderer.flipX = false;
                return TurnAction.CreateMoveAction(this, Vector2Int.right + Vector2Int.down, executionTurn, basePriority);

            // attacks
            case EntityActionPreset.AttackRight:
                spriteRenderer.flipX = false;
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, turnNumber, false, false, basePriority);
            case EntityActionPreset.AttackLeft:
                spriteRenderer.flipX = true;
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, turnNumber, true, false, basePriority);
            case EntityActionPreset.AttackUp:
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, turnNumber, false, true, basePriority);
            case EntityActionPreset.AttackDown:
                cooldownTurns = weapon.onUseActionCooldown;
                return TurnAction.CreateAttackAction(this, turnNumber, true, true, basePriority);

        }
    }

    public EntityActionPreset SetActionPreset(EntityActionPreset actionPreset)
    {
        moveIsForced = false;
        adjacentTiles = new AdjacentTiles(GameGrid.instance, gridPosition);
        selectedEntityActionPreset = actionPreset;

        // walk up stairs
        // this does not account for having a wall directly above a stair... surely that will never happen
        if (selectedEntityActionPreset == EntityActionPreset.MoveLeft && adjacentTiles.left == GridTileGeometry.StairLeft)
        {
            selectedEntityActionPreset = EntityActionPreset.MoveUpLeft;
        }
        else if (selectedEntityActionPreset == EntityActionPreset.MoveRight && adjacentTiles.right == GridTileGeometry.StairRight)
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

        // you cannot walk into walls, set preset to none/idle
        if ((selectedEntityActionPreset == EntityActionPreset.MoveDown && GridTileTypeHelper.IsTileSolid(adjacentTiles.under)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveUp && GridTileTypeHelper.IsTileSolid(adjacentTiles.above)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveLeft && GridTileTypeHelper.IsTileSolid(adjacentTiles.left)) ||
            (selectedEntityActionPreset == EntityActionPreset.MoveRight && GridTileTypeHelper.IsTileSolid(adjacentTiles.right)))
        {
            selectedEntityActionPreset = EntityActionPreset.None;
        }

        if (!canFly)
        {
            // if no floor underneath entity and not on ladder, force it to fall down
            // this step has to happen last so it can overwrite other moves
            // you can stand on other entities
            if (GridTileTypeHelper.IsTileEmpty(adjacentTiles.under)
                && !GridTileTypeHelper.IsTileClimbable(adjacentTiles.current)
                && !entityManager.TryGetEntityAtGridPosition(gridPosition + Vector2Int.down, out GridEntity entity))
            {
                selectedEntityActionPreset = EntityActionPreset.MoveDown;
                moveIsForced = true;
            }

        }

        return selectedEntityActionPreset;
    }

    protected virtual TurnAction EvaluateNonPresetAction(int turnNumber)
    {
        return TurnAction.CreateIdleAction(this);
    }

    public virtual void OnDeath()
    {
        entityManager.KillEntity(this);
        // drop loot
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) OnDeath();
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

[System.Serializable]
public struct TurnAction
{
    public int priority;            // lower priority is executed first
    public int deletionTimestamp;   // the timestamp when the EntityManager can safely delete this turnAction
    public TurnActionMove move;
    public TurnActionAttack[] attacks;
    public bool enableMove, enableAttack;
    public GridEntity caster;

    public static TurnAction CreateMoveAction(GridEntity caster, Vector2Int positionOffset, int executionTurn, int priority = 0)
    {
        TurnAction newAction = new TurnAction();
        newAction.priority = priority;
        newAction.caster = caster;
        newAction.move = new TurnActionMove(caster.gridPosition + positionOffset, executionTurn);
        newAction.enableMove = true;
        newAction.CalculateDeletionTimestamp();

        return newAction;
    }

    public static TurnAction CreateIdleAction(GridEntity caster)
    {
        TurnAction newAction = new TurnAction();
        newAction.caster = caster;

        return newAction;
    }

    public static TurnAction CreateAttackAction(GridEntity caster, int executionTurn, bool mirror, bool swizzle, int priority = 0)
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

            TurnActionAttack attack = new TurnActionAttack(caster.gridPosition + pos, executionTurn + data.startupDelay, data.damage);
            attackPoints.Add(attack);
        }

        newAction.attacks = attackPoints.ToArray();
        newAction.enableAttack = true;
        newAction.CalculateDeletionTimestamp();

        return newAction;
    }

    public void CalculateDeletionTimestamp()
    {
        int highest = move.executionTurn;

        if (attacks != null)
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                int t = attacks[i].executionTurn;
                if (t > highest) highest = t;
            }
        }

        deletionTimestamp = highest;
    }

    [System.Serializable]
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

    [System.Serializable]
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


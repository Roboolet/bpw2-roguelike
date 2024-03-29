using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityManager : MonoBehaviour
{
    public PlayerEntity playerEntityReference;
    public List<GridEntity> entities = new List<GridEntity>();
    List<TurnAction> activeTurnActions = new List<TurnAction>();
    int currentTurn;

    public Action OnTimeAdvanced;
    GameGrid gameGrid;

    private void Start()
    {
        for(int i = 0; i < entities.Count; i++)
        {
            entities[i].entityManager = this;
        }

        gameGrid = GameGrid.instance;
        gameGrid.OnCameraUpdated += DrawEntities;
        OnTimeAdvanced += UpdateGameGridVisuals;
        OnTimeAdvanced?.Invoke();
    }

    void UpdateGameGridVisuals()
    {
        gameGrid.GridCameraPosition = playerEntityReference.gridPosition;
        gameGrid.DrawTiles();
    }

    public void EndTurn()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            GridEntity e = entities[i];

            // get all turn actions from all entities
            activeTurnActions.Add(e.EvaluateNextAction(currentTurn));

        }

        // sort using priority (lowest to highest)
        activeTurnActions.Sort(new TurnActionSorter());

        // execute actions for this turn in order, remove if cooldown is over
        for (int index = activeTurnActions.Count - 1; index >= 0; index--)
        {
            TurnAction action = activeTurnActions[index];

            // execute movement
            if (action.enableMove)
            {
                if (action.move.executionTurn <= currentTurn && !TryGetEntityAtGridPosition(action.move.gridPosition, out GridEntity target))
                {
                    action.caster.gridPosition = action.move.gridPosition;
                }
                // show intent
                else
                {

                }
            }

            // execute attacks
            if (action.enableAttack)
            {
                for (int i = 0; i < action.attacks.Length; i++)
                {
                    TurnAction.TurnActionAttack attack = action.attacks[i];
                    if (attack.executionTurn <= currentTurn && TryGetEntityAtGridPosition(attack.gridPosition, out GridEntity attackTarget))
                    {
                        attackTarget.TakeDamage(attack.damage);
                        Debug.Log($"{action.caster.name} attacked {attackTarget.name} for {attack.damage} damage");
                    }
                    // show intent
                    else
                    {

                    }
                }
            }
            
            // delete from active actions when used up
            if (action.deletionTimestamp <= currentTurn)
            {
                activeTurnActions.RemoveAt(index);
            }
        }        

        currentTurn++;
        OnTimeAdvanced?.Invoke();
        DrawEntities();
    }

    /// <summary>
    /// Draw all entities within view. Happens every game tick.
    /// </summary>
    public void DrawEntities()
    {
        for(int i = 0; i < entities.Count; i++)
        {
            GridEntity entity = entities[i];
            Vector2 screenPos = gameGrid.GetSpriteGridElementAtGridPosition(entity.gridPosition).transform.position;

            entity.transform.position = screenPos;
        }
    }

    public bool TryGetEntityAtGridPosition(Vector2Int pos, out GridEntity entity)
    {
        entity = null;
        for(int i = 0; i < entities.Count; i++)
        {
            GridEntity e = entities[i];
            if(e.gridPosition == pos)
            {
                entity = e;
                return true;
            }
        }

        return false;
    }

    public void KillEntity(GridEntity entity, bool triggerOnDeath = false)
    {
        if(triggerOnDeath) { entity.OnDeath(); }
        entities.Remove(entity);
        Destroy(entity.gameObject);
    }
}

public class TurnActionSorter : IComparer<TurnAction>
{
    public int Compare(TurnAction x, TurnAction y)
    {
        return x.priority.CompareTo(y.priority);
    }
}

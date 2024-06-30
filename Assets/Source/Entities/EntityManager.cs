using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityManager : MonoBehaviour
{
    [Header("Entities")]
    public PlayerEntity playerEntityReference;
    public List<GridEntity> entities = new List<GridEntity>();
    [SerializeField] EntitySpawnerDictionaryEntry[] spawnPrefabsInput;
    Dictionary<GridTileSpawns, GameObject> spawnPrefabs = new Dictionary<GridTileSpawns, GameObject>();

    [Header("Action Indicators")]
    public GameObject actionIndicatorTemplate;
    public int actionIndicatorPoolSize;
    public Transform actionIndicatorParent;
    EntityActionIndicator[] actionIndicators;
    int lastUsedActionIndicator;

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

        actionIndicators = new EntityActionIndicator[actionIndicatorPoolSize];
        for (int i = 0; i < actionIndicatorPoolSize; i++)
        {
            actionIndicators[i] = Instantiate(actionIndicatorTemplate, actionIndicatorParent).GetComponent<EntityActionIndicator>();
        }

        for (int i = 0; i < spawnPrefabsInput.Length; i++)
        {
            spawnPrefabs.Add(spawnPrefabsInput[i].type, spawnPrefabsInput[i].prefab);
        }

        gameGrid = GameGrid.instance;
        gameGrid.OnCameraUpdated += DrawEntities;

        // spawn enemies
        EntitySpawnData[] spawns = gameGrid.GetAllEntitySpawns();
        for(int i =0; i < spawns.Length; i++)
        {
            EntitySpawnData data = spawns[i];
            if (spawnPrefabs.TryGetValue(data.type, out GameObject value))
            {
                GridEntity entity = Instantiate(value).GetComponent<GridEntity>();
                entity.gridPosition = data.pos;
                entities.Add(entity);
            }
        }

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
        ResetAllActionIndicators();
        List<ActionIndicatorData> nextTurnIndicators = new List<ActionIndicatorData>();

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
                if (action.move.executionTurn == currentTurn && !TryGetEntityAtGridPosition(action.move.gridPosition, out GridEntity target))
                {
                    action.caster.gridPosition = action.move.gridPosition;
                }
                // show intent
                else if(action.move.executionTurn == currentTurn+1)
                {
                    nextTurnIndicators.Add(new ActionIndicatorData(action.move.gridPosition, EntityActionIndicator.ActionType.Move));
                }
            }

            // execute attacks
            if (action.enableAttack)
            {
                for (int i = 0; i < action.attacks.Length; i++)
                {
                    TurnAction.TurnActionAttack attack = action.attacks[i];
                    if (attack.executionTurn == currentTurn && TryGetEntityAtGridPosition(attack.gridPosition, out GridEntity attackTarget))
                    {
                        attackTarget.TakeDamage(attack.damage);
                    }
                    // show intent
                    else if(attack.executionTurn == currentTurn+1)
                    {
                        nextTurnIndicators.Add(new ActionIndicatorData(attack.gridPosition, EntityActionIndicator.ActionType.Attack));
                    }
                    else if(attack.executionTurn > currentTurn + 1)
                    {
                        nextTurnIndicators.Add(new ActionIndicatorData(attack.gridPosition, EntityActionIndicator.ActionType.AttackWarning));
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
        ActivateActionIndicators(nextTurnIndicators.ToArray());
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

    void ActivateActionIndicators(ActionIndicatorData[] indicators)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            ActionIndicatorData data = indicators[i];
            Vector2 screenpos = gameGrid.GetSpriteGridElementAtGridPosition(data.gridPosition).transform.position;
            actionIndicators[lastUsedActionIndicator].SetIndicator(screenpos, data.type);

            lastUsedActionIndicator = (lastUsedActionIndicator + 1) % actionIndicators.Length;
        }
    }

    void ResetAllActionIndicators()
    {
        for (int i = 0; i < actionIndicators.Length; i++)
        {
            actionIndicators[i].ResetIndicator();
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

    public void CancelActionsFromEntity(GridEntity entity)
    {

        for (int i = activeTurnActions.Count - 1; i >= 0; i--)
        {
            TurnAction action = activeTurnActions[i];
            if (action.caster == entity)
            {
                activeTurnActions.RemoveAt(i);
            }
        }
    }

    public void KillEntity(GridEntity entity, bool triggerOnDeath = false)
    {
        if(triggerOnDeath) { entity.OnDeath(); }
        entities.Remove(entity);
        Destroy(entity.gameObject);
    }
}

public struct EntitySpawnData
{
    public GridTileSpawns type;
    public Vector2Int pos;

    public EntitySpawnData(Vector2Int position, GridTileSpawns spawn)
    {
        this.type = spawn;
        this.pos = position;
    }
}

[Serializable]
public struct EntitySpawnerDictionaryEntry
{
    public GridTileSpawns type;
    public GameObject prefab;
}

public class TurnActionSorter : IComparer<TurnAction>
{
    public int Compare(TurnAction x, TurnAction y)
    {
        return x.priority.CompareTo(y.priority);
    }
}

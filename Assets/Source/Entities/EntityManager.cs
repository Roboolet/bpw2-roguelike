using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityManager : MonoBehaviour
{
    public GameGrid gameGrid;
    public PlayerEntity playerEntityReference;
    public List<GridEntity> entities = new List<GridEntity>();
    Dictionary<GridEntity, TurnAction> activeTurnActions = new Dictionary<GridEntity, TurnAction>();
    int currentTurn;

    public Action OnTimeAdvanced;

    private void Awake()
    {
        gameGrid.OnCameraUpdated += DrawEntities;
        OnTimeAdvanced += UpdateGameGridVisuals;
        // find the player among entities
    }

    private void Start()
    {
        OnTimeAdvanced?.Invoke();
    }

    void UpdateGameGridVisuals()
    {
        gameGrid.GridCameraPosition = playerEntityReference.gridPosition;
        gameGrid.DrawTiles();
    }

    public void EndTurn()
    {
        SortedList<int, TurnAction> actionsThisTurn = new SortedList<int, TurnAction>();

        // get all active actions
        for (int i = 0; i < entities.Count; i++)
        {
            GridEntity e = entities[i];

            if(activeTurnActions.TryGetValue(e, out TurnAction action))
            {
                actionsThisTurn.Add(action.priority, action);
            }
        }

        // execute actions for this turn in order, remove if cooldown is over
        foreach(KeyValuePair<int, TurnAction> pair in actionsThisTurn)
        {
            TurnAction action = pair.Value;

            switch (action.type)
            {

                default: // do nothing
                    break;

                case TurnActionType.Move:
                    // execute movement
                    if (action.executionTurn <= currentTurn)
                    {
                        action.caster.gridPosition = action.caster.gridPosition + action.values[0];
                    }
                    // show intent
                    else
                    {

                    }
                    break;

                case TurnActionType.Attack:
                    // execute attack
                    if (action.executionTurn <= currentTurn)
                    {

                    }
                    // show intent
                    else
                    {

                    }
                    break;
            }

            // delete from active actions when used up
            if(action.executionTurn + action.cooldown == currentTurn)
            {
                activeTurnActions.Remove(action.caster);
            }

            currentTurn++;
        }

        // generate new actions if possible
        for (int i = 0; i < entities.Count; i++)
        {
            GridEntity e = entities[i];

            // only add new actions when there is none for this entity
            // makes cooldowns work
            if (!activeTurnActions.ContainsKey(e))
            {
                activeTurnActions.Add(e, e.EvaluateNextAction(currentTurn));
            }
        }

        OnTimeAdvanced?.Invoke();
        ;
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
            Vector2 screenPos = gameGrid.GetSpriteGridElement(entity.gridPosition).transform.position;

            entity.transform.position = screenPos;
        }
    }

}

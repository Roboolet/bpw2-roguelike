using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public List<GridEntity> entities = new List<GridEntity>();
    Dictionary<GridEntity, TurnAction> activeTurnActions = new Dictionary<GridEntity, TurnAction>();
    int currentTurn;

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
        for(int i = 0; i < actionsThisTurn.Count; i++)
        {
            TurnAction action = actionsThisTurn[i];

            switch (action.type)
            {

                default: // do nothing
                    break;

                case TurnActionType.Move:
                    // execute movement
                    if (action.executionTurn == currentTurn)
                    {

                    }
                    // show intent
                    else
                    {

                    }
                    break;

                case TurnActionType.Attack:
                    // execute attack
                    if (action.executionTurn == currentTurn)
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
    }
}

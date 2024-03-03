using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [HideInInspector] public List<GridEntity> entities = new List<GridEntity>();
    int currentTurn;

    public void NextTurn()
    {
        currentTurn++;
        for(int i = 0; i < entities.Count; i++)
        {
            GridEntity e = entities[i];
            e.EvaluateTurn(currentTurn);
        }
    }
}

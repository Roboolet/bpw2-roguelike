using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityActionIndicator : MonoBehaviour
{
    public void SetIndicator(Vector2 position, ActionType type)
    {
        gameObject.SetActive(true);
    }

    public void ResetIndicator()
    {
        gameObject.SetActive(false);
    }

    public enum ActionType
    {
        Generic, Move, Attack
    }
}

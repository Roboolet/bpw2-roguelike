using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityActionIndicator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Vector2 offset;
    [SerializeField] Sprite attack, attackWarning, move;

    public void SetIndicator(Vector2 worldPosition, ActionType type, ActionDirection direction = ActionDirection.None)
    {
        gameObject.SetActive(true);
        transform.position = worldPosition + offset;

        switch (type)
        {
            case ActionType.Move:
                spriteRenderer.sprite = move;
                break;

            case ActionType.Attack: 
                spriteRenderer.sprite = attack; 
                break;

            case ActionType.AttackWarning:
                spriteRenderer.sprite = attackWarning;
                break;
        }
    }

    public void ResetIndicator()
    {
        gameObject.SetActive(false);
    }

    public enum ActionType
    {
        Generic, Move, Attack, AttackWarning
    }
    public enum ActionDirection
    {
        None, North, East, South, West, NorthEast, SouthEast, SouthWest, NorthWest
    }
}

public struct ActionIndicatorData
{
    public Vector2Int gridPosition;
    public EntityActionIndicator.ActionType type;
    public EntityActionIndicator.ActionDirection directionFromCaster;

    public ActionIndicatorData(Vector2Int gridPosition, EntityActionIndicator.ActionType type, EntityActionIndicator.ActionDirection direction = EntityActionIndicator.ActionDirection.None)
    {
        this.gridPosition = gridPosition;
        this.type = type;
        this.directionFromCaster = direction;
    }
}

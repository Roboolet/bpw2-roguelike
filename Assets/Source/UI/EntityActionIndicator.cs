using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityActionIndicator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Vector2 offset;
    [SerializeField] Sprite attack, move;

    public void SetIndicator(Vector2 worldPosition, ActionType type)
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
        }
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

public struct ActionIndicatorData
{
    public Vector2Int gridPosition;
    public EntityActionIndicator.ActionType type;

    public ActionIndicatorData(Vector2Int gridPosition, EntityActionIndicator.ActionType type)
    {
        this.gridPosition = gridPosition;
        this.type = type;
    }
}

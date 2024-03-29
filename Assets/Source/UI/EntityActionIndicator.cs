using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityActionIndicator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite attack, move;

    public void SetIndicator(Vector2 position, ActionType type)
    {
        gameObject.SetActive(true);
        transform.position = position;

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

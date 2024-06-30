using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerEntity : GridEntity
{
    Camera mainCamera;
    [Header("Player-specific settings")]
    [SerializeField] EventSystem eventSystem;
    [HideInInspector] public AimDirection pointerDirection;

    [Header("Move preview")]
    [SerializeField] SpriteRenderer preview;
    [SerializeField] Sprite prevAttackHorizontal, prevAttackVertical, prevMoveCardinal, prevMoveDiagonal;
    public bool attackMode { get; set; }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // used for the input
    private void Update()
    {
        // only execute when not hovering over buttons
        if (!eventSystem.IsPointerOverGameObject())
        {
            // calculate what direction you are aiming
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 normalizedDistance = mousePos - transform.position;

            // check which direction the mouse cursor is going in with relation to the player
            AimDirection highest = AimDirection.Right;
            float highestNum = Vector2.Dot(normalizedDistance, Vector2.right);
            float numLeft = Vector2.Dot(normalizedDistance, Vector2.left);
            float numUp = Vector2.Dot(normalizedDistance, Vector2.up);
            float numDown = Vector2.Dot(normalizedDistance, Vector2.down);

            if (numLeft > highestNum) { highest = AimDirection.Left; highestNum = numLeft; }
            if (numUp > highestNum) { highest = AimDirection.Up; highestNum = numUp; }
            if (numDown > highestNum) { highest = AimDirection.Down; highestNum = numDown; }
            pointerDirection = highest;

            UpdateActionPreset(pointerDirection);

            if (Input.GetButtonDown("Fire1"))
            {
                entityManager.EndTurn();
            }
        }
        else
        {
            pointerDirection = AimDirection.None;
        }
    }

    public void UpdateActionPreset(AimDirection direction)
    {
        EntityActionPreset validatedPreset = EntityActionPreset.None;

        // idle
        if (direction == AimDirection.None)
        {
            validatedPreset = SetActionPreset(EntityActionPreset.None);
            return;
        }

        // validate the selected action
        if (attackMode)
        {
            switch(direction)
            {
                case AimDirection.Right:
                    validatedPreset = SetActionPreset(EntityActionPreset.AttackRight);
                    break;
                case AimDirection.Left:
                    validatedPreset = SetActionPreset(EntityActionPreset.AttackLeft);
                    break;
                case AimDirection.Up:
                    validatedPreset = SetActionPreset(EntityActionPreset.AttackUp);
                    break;
                case AimDirection.Down:
                    validatedPreset = SetActionPreset(EntityActionPreset.AttackDown);
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case AimDirection.Right:
                    validatedPreset = SetActionPreset(EntityActionPreset.MoveRight);
                    break;
                case AimDirection.Left:
                    validatedPreset = SetActionPreset(EntityActionPreset.MoveLeft);
                    break;
                case AimDirection.Up:
                    validatedPreset = SetActionPreset(EntityActionPreset.MoveUp);
                    break;
                case AimDirection.Down:
                    validatedPreset = SetActionPreset(EntityActionPreset.MoveDown);
                    break;
            }
        }

        // update the preview
        const float dist = 0.7f;
        const float distDiag = 0.5f;
        switch (validatedPreset)
        {
            // moves
            default:
                preview.sprite = null;
                break;
            case EntityActionPreset.MoveUp:
                preview.sprite = prevMoveCardinal;
                preview.transform.localPosition = new Vector2(0, dist);
                preview.transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case EntityActionPreset.MoveRight:
                preview.sprite = prevMoveCardinal;
                preview.transform.localPosition = new Vector2(dist, 0);
                preview.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case EntityActionPreset.MoveDown:
                preview.sprite = prevMoveCardinal;
                preview.transform.localPosition = new Vector2(0, -dist);
                preview.transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case EntityActionPreset.MoveLeft:
                preview.sprite = prevMoveCardinal;
                preview.transform.localPosition = new Vector2(-dist, 0);
                preview.transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case EntityActionPreset.MoveDownLeft:
                preview.sprite = prevMoveDiagonal;
                preview.transform.localPosition = new Vector2(-distDiag, -distDiag);
                preview.transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case EntityActionPreset.MoveDownRight:
                preview.sprite = prevMoveDiagonal;
                preview.transform.localPosition = new Vector2(distDiag, -distDiag);
                preview.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case EntityActionPreset.MoveUpLeft:
                preview.sprite = prevMoveDiagonal;
                preview.transform.localPosition = new Vector2(-distDiag, distDiag);
                preview.transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case EntityActionPreset.MoveUpRight:
                preview.sprite = prevMoveDiagonal;
                preview.transform.localPosition = new Vector2(distDiag, distDiag);
                preview.transform.eulerAngles = new Vector3(0, 0, 90);
                break;

            // attacks
            case EntityActionPreset.AttackUp:
                preview.sprite = prevAttackVertical;
                preview.transform.localPosition = new Vector2(0, dist);
                preview.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case EntityActionPreset.AttackDown:
                preview.sprite = prevAttackVertical;
                preview.transform.localPosition = new Vector2(0, -dist);
                preview.transform.eulerAngles = new Vector3(180, 0, 0);
                break;
            case EntityActionPreset.AttackLeft:
                preview.sprite = prevAttackHorizontal;
                preview.transform.localPosition = new Vector2(-dist, 0);
                preview.transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            case EntityActionPreset.AttackRight:
                preview.sprite = prevAttackHorizontal;
                preview.transform.localPosition = new Vector2(dist, 0);
                preview.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    public override void OnDeath()
    {
        GameStateManager.instance.ChangeGameState(GameState.GameOver);
        entityManager.KillEntity(this);
    }

    public enum AimDirection
    {
        None, Right, Left, Up, Down
    }
}

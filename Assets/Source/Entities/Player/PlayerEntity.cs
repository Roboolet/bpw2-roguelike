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

            AimDirection highest = AimDirection.Right;
            float highestNum = Vector2.Dot(normalizedDistance, Vector2.right);
            float numLeft = Vector2.Dot(normalizedDistance, Vector2.left);
            float numUp = Vector2.Dot(normalizedDistance, Vector2.up);
            float numDown = Vector2.Dot(normalizedDistance, Vector2.down);

            if (numLeft > highestNum) { highest = AimDirection.Left; highestNum = numLeft; }
            if (numUp > highestNum) { highest = AimDirection.Up; highestNum = numUp; }
            if (numDown > highestNum) { highest = AimDirection.Down; highestNum = numDown; }
            pointerDirection = highest;

            if (Input.GetButtonDown("Fire1"))
            {
                Submit(pointerDirection);
            }
        }
        else pointerDirection = AimDirection.None;
    }

    public void Submit(AimDirection direction)
    {
        if (attackMode)
        {
            switch(direction)
            {
                case AimDirection.Right:
                    selectedEntityActionPreset = EntityActionPreset.AttackRight;
                    break;
                case AimDirection.Left:
                    selectedEntityActionPreset = EntityActionPreset.AttackLeft;
                    break;
                case AimDirection.Up:
                    selectedEntityActionPreset = EntityActionPreset.AttackUp;
                    break;
                case AimDirection.Down:
                    selectedEntityActionPreset = EntityActionPreset.AttackDown;
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case AimDirection.Right:
                    selectedEntityActionPreset = EntityActionPreset.MoveRight;
                    break;
                case AimDirection.Left:
                    selectedEntityActionPreset = EntityActionPreset.MoveLeft;
                    break;
                case AimDirection.Up:
                    selectedEntityActionPreset = EntityActionPreset.MoveUp;
                    break;
                case AimDirection.Down:
                    selectedEntityActionPreset = EntityActionPreset.MoveDown;
                    break;
            }
        }

        entityManager.EndTurn();
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

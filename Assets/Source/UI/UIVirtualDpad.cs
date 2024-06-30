using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVirtualDpad : MonoBehaviour
{
    PlayerEntity playerEntityReference;
    public EntityManager entityManager;

    private void Awake()
    {
        playerEntityReference = entityManager.playerEntityReference;
        entityManager.OnTimeAdvanced += UpdateButtons;
    }

    public void UpdateButtons()
    {

    }

    // see PlayerEntity.cs for what each actionID does
    public void SendPlayerAction(int actionID)
    {
        playerEntityReference.SetActionPreset((EntityActionPreset)actionID);
        entityManager.EndTurn();
    }
}

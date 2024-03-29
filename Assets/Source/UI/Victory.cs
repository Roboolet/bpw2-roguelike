using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public void MainMenuButton()
    {
        GameStateManager.instance.ChangeGameState(GameState.MainMenu);
    }
}

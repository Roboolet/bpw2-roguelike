using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject canvasGameOver, playerHud;

    private void Start()
    {
        GameStateManager.instance.OnGameStateChange += OnGameStateChange;
    }

    void OnGameStateChange(GameState state)
    {
        if(state == GameState.GameOver)
        {
            canvasGameOver.SetActive(true);
            playerHud.SetActive(false);
        }
        else
        {
            canvasGameOver.SetActive(false);
            playerHud.SetActive(true);
        }
    }

    public void MainMenuButton()
    {
        GameStateManager.instance.ChangeGameState(GameState.MainMenu);
    }
}

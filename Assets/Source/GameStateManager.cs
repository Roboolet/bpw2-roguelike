using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    [Tooltip("Put the buildIndex of the scene here")]
    public int sceneGame, sceneVictory, sceneMenu;
    [SerializeField] GameState startingGameState;

    public GameState CurrentGameState { get; private set; }
    public Action<GameState> OnGameStateChange;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        ChangeGameState(startingGameState);
    }

    public void ChangeGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                if(SceneManager.GetActiveScene().buildIndex != sceneGame)
                {
                    SceneManager.LoadSceneAsync(sceneGame);
                }
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver: 
                break;
            case GameState.Victory: 
                break;
            case GameState.MainMenu: 
                break;
        }

        OnGameStateChange?.Invoke(state);
        CurrentGameState = state;

        Debug.Log("Changed game state to " + state.ToString());
    }
}
public enum GameState
{
    Playing, Paused, GameOver, Victory, MainMenu
}

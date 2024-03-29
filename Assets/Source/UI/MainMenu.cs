using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Background effect")]
    [SerializeField] float spinSpeed, circleRadius;
    [SerializeField] Vector2Int center;
    [SerializeField] float tilePixelPerUnit;
    [SerializeField] GameGrid gameGrid;
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
        gameGrid.GridCameraPosition = center;
    }

    public void PlayButton()
    {
        GameStateManager.instance.ChangeGameState(GameState.Playing);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    private void LateUpdate()
    {
        cam.transform.position = new Vector3(center.x/tilePixelPerUnit + Mathf.Cos(Time.time * spinSpeed),
            center.y/tilePixelPerUnit + Mathf.Sin(Time.time * spinSpeed), -10) * circleRadius;
    }
}

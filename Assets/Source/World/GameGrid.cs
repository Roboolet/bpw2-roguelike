using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System;

public class GameGrid : MonoBehaviour
{
    public bool debugMode;

    // stores markers so you can't generate loot more than once from chests, and other one-time things
    public HashSet<(int, int)> singleUseMarker = new HashSet<(int, int)>();
    public LevelSettings levelSettings;

    public Vector2Int visibleTilesOnScreen;
    public Action OnCameraUpdated;

    Vector2Int _GridCameraPosition;
    public Vector2Int GridCameraPosition
    {
        get { return _GridCameraPosition; }
        set
        {
            _GridCameraPosition = value;
            OnCameraUpdated?.Invoke();
        }
    }

    SpriteRenderer[,] spriteGrid;
    Camera mainCamera;
    LevelGenerator levelGenerator;

    private void Awake()
    {
        levelGenerator = new LevelGenerator();
        levelGenerator.GenerateWorld(levelSettings);

        GenerateSpriteGrid();
        GridCameraPosition = Vector2Int.zero;

        OnCameraUpdated += DrawTiles;
    }

    private void Update()
    {
        if (!debugMode) return;

        if(Input.GetKeyDown(KeyCode.W)) { GridCameraPosition -= Vector2Int.up; }
        if (Input.GetKeyDown(KeyCode.A)) { GridCameraPosition -= Vector2Int.left; }
        if (Input.GetKeyDown(KeyCode.S)) { GridCameraPosition -= Vector2Int.down; }
        if (Input.GetKeyDown(KeyCode.D)) { GridCameraPosition -= Vector2Int.right; }
    }


    /// <summary>
    /// Generates the grid of sprites used to render the game world
    /// </summary>
    void GenerateSpriteGrid()
    {
        spriteGrid = new SpriteRenderer[visibleTilesOnScreen.x, visibleTilesOnScreen.y];
        int halfY = visibleTilesOnScreen.y / 2;
        int halfX = visibleTilesOnScreen.x / 2;

        for(int y = 0; y <  visibleTilesOnScreen.y; y++)
        {
            for(int x = 0; x < visibleTilesOnScreen.x; x++)
            {
                GameObject tileSprite = new GameObject($"TileSprite x{x} y{y}", typeof(SpriteRenderer));
                tileSprite.transform.parent = transform;
                tileSprite.transform.position = new Vector2(x - halfX, y - halfY);

                SpriteRenderer spr = tileSprite.GetComponent<SpriteRenderer>();
                spriteGrid[x, y] = spr;

                // set sprite for testing
                //spr.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), Vector2.zero);
                //tileSprite.transform.localScale = new Vector2(Screen.width / visibleTilesOnScreen.x / 1.5f, Screen.height / visibleTilesOnScreen.y / 1.5f);
            }
        }
    }

    /// <summary>
    /// Returns the SpriteRenderer corresponding to a tile in the game world
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public SpriteRenderer GetSpriteGridElement(Vector2Int gridPosition)
    {
        Vector2Int pos = levelSettings.WrapPos(gridPosition.x + GridCameraPosition.x, gridPosition.y + GridCameraPosition.y);
        return spriteGrid[pos.x, pos.y];
    }

    /// <summary>
    /// Redraw all tiles in view. Happens whenever the player position changes.
    /// </summary>
    public void DrawTiles()
    {
        // use random noise based on position to make tiles distinct from one another
        int halfX = visibleTilesOnScreen.x / 2;
        int halfY = visibleTilesOnScreen.y / 2;
        for (int x = 0; x < visibleTilesOnScreen.x; x++)
        {
            for (int y = 0; y < visibleTilesOnScreen.y; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x - GridCameraPosition.x - halfX, y - GridCameraPosition.y - halfY);
                Vector2Int screenPosition = new Vector2Int(x, y);

                GridTileType tileType = levelGenerator.Get(gridPosition);
                Sprite sprite = levelSettings.tileset.GetSpriteByTileType(tileType, gridPosition.x, gridPosition.y);
                spriteGrid[screenPosition.x, screenPosition.y].sprite = sprite;
            }
        }
    }

}

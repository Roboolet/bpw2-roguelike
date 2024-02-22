using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [HideInInspector] public GridEntity player;
    [HideInInspector] public List<GridEntity> nonPlayerEntities = new List<GridEntity>();
    LevelGenerator levelGenerator;

    // stores markers so you can't generate loot more than once from chests, and other one-time things
    public HashSet<(int, int)> singleUseMarker = new HashSet<(int, int)>();
    public LevelSettings levelSettings;
    public Vector2Int visibleTilesOnScreen;

    Camera mainCamera;

    public static GameGrid main;
    private void Awake()
    {
        main = this;

        mainCamera = Camera.main;
        GenerateSpriteGrid();

        levelGenerator = new LevelGenerator();
        levelGenerator.GenerateWorld(levelSettings);

    }

    /// <summary>
    /// Generates the grid of sprites used to render the game world
    /// </summary>
    void GenerateSpriteGrid()
    {
        for(int y = 0; y <  visibleTilesOnScreen.y; y++)
        {
            for(int x = 0; x < visibleTilesOnScreen.x; x++)
            {
                GameObject tileSprite = new GameObject($"TileSprite x{x} y{y}", typeof(SpriteRenderer));
                tileSprite.transform.parent = transform;
                Vector2 newPos = new Vector2((Screen.width / visibleTilesOnScreen.x) * x, (Screen.height / visibleTilesOnScreen.y) * y);
                tileSprite.transform.position = mainCamera.ScreenToWorldPoint(newPos);

                // set sprite for testing
                tileSprite.GetComponent<SpriteRenderer>().sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), Vector2.zero);

                tileSprite.transform.localScale = new Vector2(Screen.width / visibleTilesOnScreen.x / 1.5f, Screen.height / visibleTilesOnScreen.y / 1.5f);
            }
        }
    }

    /// <summary>
    /// Redraw all tiles in view. Happens whenever the player position changes. Width and height extend one tile further to make animations smoother
    /// </summary>
    public void DrawTiles()
    {
        // use random noise based on position to make tiles distinct from one another

    }

    /// <summary>
    /// Draw all entities within view. Happens every game tick.
    /// </summary>
    public void DrawEntities()
    {

    }
}

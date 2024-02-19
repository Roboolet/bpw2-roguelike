using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public GridEntity player;
    [HideInInspector] public List<GridEntity> nonPlayerEntities = new List<GridEntity>();

    // stores markers so you can't generate loot more than once from chests, and other one-time things
    public HashSet<(int, int)> singleUseMarker = new HashSet<(int, int)>();
    public LevelSettings levelSettings;

    LevelGenerator levelGenerator;

    public static GameGrid main;
    private void Awake()
    {
        main = this;

        GenerateSpriteGrid();

        levelGenerator = new LevelGenerator();
        levelGenerator.GenerateWorld(levelSettings);

    }

    /// <summary>
    /// Generates the grid of sprites used to render the game world
    /// </summary>
    void GenerateSpriteGrid()
    {

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

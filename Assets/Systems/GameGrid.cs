using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{    
    public List<GridEntity> entities = new List<GridEntity>();
    // This is info the generator needs to generate the level
    // At the top will be a save-point room (or the top fo the tower)
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

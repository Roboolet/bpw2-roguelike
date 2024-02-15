using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public GridTileType[][] grid;
    public List<GridEntity> entities = new List<GridEntity>();
    // This is info the generator needs to generate the level
    // At the top will be a save-point room (or the top fo the tower)
    public int levelWidth;
    public int levelHeight;


    public static GameGrid main;
    private void Awake()
    {
        main = this;

        GenerateSpriteGrid();
    }

    /// <summary>
    /// Generates the grid of sprites used to render the game world
    /// </summary>
    void GenerateSpriteGrid()
    {

    }

    /// <summary>
    /// Redraw all tiles in view. Happens whenever the player position changes. Width and height extend one tile further to make walking smoother
    /// </summary>
    public void DrawTiles()
    {

    }

    /// <summary>
    /// Draw all entities within view. Happens every game tick.
    /// </summary>
    public void DrawEntities()
    {

    }

    /// <summary>
    /// Places a specific tile at the position. Returns the tile that was originally there. Use ID 0 to remove.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public GridTileType Place(GridTileType id, int x, int y)
    {
        return 0;
    }
}

public enum GridTileType : byte
{
    Empty, Wall, Ladder, StairRight, StairLeft
}

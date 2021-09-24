using UnityEngine;

/// <summary>
/// The attachable script on the object to be placed on the grid to store the data of its grid size and position
/// </summary>
public class GridObject: MonoBehaviour
{
    //The grid size that the object takes on the grid
    public Vector2Int gridSize;
    //The current grid position on the grid
    public Vector2Int gridPosition;

    /// <summary>
    /// Setup the grid object data so it can be modified by the grid manager
    /// </summary>
    /// <param name="gridSize">The grid size that the object takes on the grid</param>
    /// <param name="gridPosition">The current grid position on the grid</param>
    public GridObject(Vector2Int gridSize, Vector2Int gridPosition)
    {
        //Setup the object's grid size
        this.gridSize = gridSize;
        //Setup the object's grid position
        this.gridPosition = gridPosition;
    }
}

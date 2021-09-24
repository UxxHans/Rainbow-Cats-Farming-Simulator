using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script that manage the area for the placeables using grid.
/// The script records the occupy space in the grid and place the game object in a tidy way.
/// After each operation on the editing of grid, the base grid cells disappears,
/// leaving the organized game objects with the grid position and size data on it.
/// </summary>
public class GridManager : MonoBehaviour
{
    #region Variable declarations
    //Set if the script is checking raycast of any buildable grid plane
    public bool raycastEnabled;

    //The gird base cell object to spawn
    public GameObject gridUnit;
    //The size of the grid object [In game size]
    public float gridUnitSize;
    //The size of the grid [How many grids on xy]
    public Vector2Int gridSize;
    //The left bottom position of the grid
    public Vector3 gridPosition;
    //The 2D array to store if the grid is occupied or not
    public bool[,] gridArray;

    //The layer for the gird base
    public LayerMask gridBaseLayer;
    //The layer for the objects built on the grid base
    public LayerMask gridObjectLayer;
    //The layer for the objects built on the grid base
    public LayerMask UILayer;
    //Build modes [Add building to the grid, remove building from the grid]
    public enum Mode { AddPlaceables, Demolish }
    public Mode gridMode;

    //The current grid that the player select
    private Transform c_Grid;
    //The current prefab grid object the player selected to build on the gird
    private GameObject c_GridPrefab;
    //The current in-game gird object the player selected to remove from the grid
    private GameObject c_GridObject;
    //The transform that stores all the grids
    private GameObject c_GridParent;
    //Current placeable data
    private Placeable c_Placeable;
    //The correct euler angle of the grid unit
    private static Vector3 gridUnitOffset = new Vector3(-90, 0, 0);
    //The correct euler angle of the grid prefab
    private static Vector3 gridPrefabOffset = new Vector3(0, -180, 0);

    //All interface color data
    private static Color chosenGridObjectColor = Color.blue;
    private static Color normalGridObjectColor = Color.white;
    private static Color vacantGridColor = Color.green;
    private static Color obstructGridColor = Color.red;
    private static Color normalGridColor = Color.white;
    private static Color occupiedGridColor = Color.grey;

    /// <summary>
    /// Set the raycast state, if inactive, the player won't be able to select grid to build
    /// </summary>
    /// <param name="active">Active state</param>
    public void SetActive(bool active) { raycastEnabled = active; c_GridParent.SetActive(active); }
    #endregion

    #region Initialization
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    public void Start()
    {
        InitializeGrid();
    }

    /// <summary>
    /// Create the grid and spawn the grid base cell objects
    /// </summary>
    public void InitializeGrid()
    {
        //Create a parent object for grid base and add it to the grid parent variable
        c_GridParent = new GameObject("Grid Units");
        //Initialize the grid array
        gridArray = new bool[gridSize.x, gridSize.y];
        //Spawn the grid object one by one
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                //Get the x postion
                float posX = gridPosition.x + x * gridUnitSize;
                //Get the y position
                float posY = gridPosition.z + y * gridUnitSize;
                //Spawn the grid unit
                Instantiate(gridUnit, new Vector3(posX, gridPosition.y, posY), Quaternion.Euler(gridUnitOffset), c_GridParent.transform);
                //Set the grid to not occupied
                gridArray[x, y] = false;
            }
        }
        //Set the color of the grid
        RefreshGrid();
        //Hide the grid and disable the build function
        SetActive(false);
    }
    #endregion

    #region Grid Fetch Information
    /// <summary>
    /// Get a list of transform of all the grid base cell objects within the rectangele, given the postion and size.
    /// Note: The pivot is at the bottom left
    /// </summary>
    /// <param name="position">Bottom left position of the area</param>
    /// <param name="size">The size of the area</param>
    /// <returns></returns>
    List<Transform> GetGridTransforms(Vector2Int position, Vector2Int size)
    {
        //Initialize the list
        List<Transform> output = new List<Transform>();
        //Get the grid base cells
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                //If the grid cell is not occupied or out of range
                if (CheckSpace(new Vector2Int(position.x + x, position.y + y), Vector2Int.one))
                    //Add the cell object to the list
                    output.Add(c_GridParent.transform.GetChild((position.y + y) * gridSize.x + (position.x + x)));
            }
        }
        //Return the list
        return output;
    }

    /// <summary>
    /// Get the postion in the grid according to the hierarchy index 
    /// </summary>
    /// <param name="siblingIndex">Hierarchy index</param>
    /// <returns></returns>
    Vector2Int GetPosition(int siblingIndex)
    {
        //As the grid is built from bottom left, we can calculate the postion using a little bit of math
        int x = siblingIndex % gridSize.x;
        int y = siblingIndex / gridSize.x;
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Check if the area has occupied cell of if it is out of range
    /// </summary>
    /// <param name="position">Bottom left of the area</param>
    /// <param name="size">Size of the area</param>
    /// <returns></returns>
    public bool CheckSpace(Vector2Int position, Vector2Int size)
    {
        //Check if its out of grid range
        if (position.x < 0 || position.y < 0) return false;
        if (position.x + size.x > gridArray.GetLength(0) || position.y + size.y > gridArray.GetLength(1)) return false;

        //Check any obstructions
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (gridArray[position.x + x, position.y + y]) return false;
            }
        }

        //All passed, return true
        return true;
    }
    #endregion

    #region Grid Visual
    /// <summary>
    /// Set the vacant and occupied color on th grid according to the curren grid array
    /// </summary>
    public void RefreshGrid()
    {
        //Set the not occupied color
        foreach (Transform child in c_GridParent.transform) 
            child.GetComponent<MeshRenderer>().material.color = normalGridColor;

        //Set the occupied color
        foreach (Transform child in c_GridParent.transform)
        {
            Vector2Int childPosition = GetPosition(child.GetSiblingIndex());
            Material material = child.GetComponent<MeshRenderer>().material;
            if (gridArray[childPosition.x, childPosition.y]) material.color = occupiedGridColor;
        }
    }

    /// <summary>
    /// Set the current selected area and show if the area is vacant by changing the grid color
    /// </summary>
    /// <param name="position">Selected position</param>
    /// <param name="size">Selected size</param>
    public void RefreshEditGrid(Vector2Int position, Vector2Int size)
    {
        //Set the grid color
        foreach (Transform child in GetGridTransforms(position, size)) 
            child.GetComponent<MeshRenderer>().material.color = CheckSpace(position, size) ? vacantGridColor : obstructGridColor;
    }
    #endregion

    #region Grid Operations
    bool SetGrid(Vector2Int position, Vector2Int size)
    {
        //Return if space is occupied
        if (!CheckSpace(position, size)) return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                gridArray[position.x + x, position.y + y] = true;
            }
        }

        return true;
    }

    void RemoveGrid(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                gridArray[position.x + x, position.y + y] = false;
            }
        }
    }
    #endregion

    #region Top Layer Functions
    /// <summary>
    /// Remove current selected object from grid
    /// </summary>
    void RemoveGridObject()
    {
        //Get the grid position from the current object selected
        Vector2Int position = c_GridObject.GetComponent<GridObject>().gridPosition;
        //Get the grid size from the current object selected
        Vector2Int size = c_GridObject.GetComponent<GridObject>().gridSize;
        //Remove the recorded occupied data from the grid array
        RemoveGrid(position, size);
        //Remove the object on the grid
        Destroy(c_GridObject);
    }
    
    /// <summary>
    /// Place the current selected object to the grid
    /// </summary>
    /// <param name="position">Position on the grid</param>
    void SetGridObject(Vector2Int position)
    {
        //If failed to occupy the grid array, which means the postion may be occupied or out of range, return
        if (!SetGrid(position, c_GridPrefab.GetComponent<GridObject>().gridSize)) return;
        //If successdfully occupied the grid array, instantiate the game object on the grid in game position
        GameObject instance = Instantiate(c_GridPrefab, c_Grid.position, Quaternion.Euler(gridPrefabOffset));
        //Get the position variable stored in the game object and store the grid position in the game object
        instance.GetComponent<GridObject>().gridPosition = position;
        //Find the placeable manager in scene
        PlaceableManager placeableManager = FindObjectOfType<PlaceableManager>();
        //Since the object is placed, we reduce the placeable in the inventory by one
        placeableManager.RemovePlaceable(c_Placeable.uniqueID, 1);
        //If there are no object of the same type in the inventory, we can not place any more objects.
        if (!placeableManager.PlaceableExist(c_Placeable.uniqueID))
            c_GridPrefab = null;
    }

    /// <summary>
    /// Change the mode of the grid editor
    /// </summary>
    /// <param name="mode">The enum index of the Mode</param>
    public void ChangeMode(int mode)
    {
        //Set the mode
        gridMode = (Mode)mode;
        //Refresh the grid visual [remove the grid selection visual]
        RefreshGrid();
    }

    /// <summary>
    /// Set the current placeable
    /// </summary>
    /// <param name="placeable">Placeable data</param>
    public void SetPlaceable(Placeable placeable) 
    {
        //Set current placeable data
        c_Placeable = placeable;
        //Set current placeable prefab
        c_GridPrefab = placeable.prefab;
    }

    /// <summary>
    /// Update is called once per frame to check the raycast on the surface for the placeables
    /// </summary>
    public void Update()
    {
        //If not enabled, return
        if (!raycastEnabled) return;
        //Get the main camera transform
        Transform camera = Camera.main.transform;
        //If hit UI return
        if (Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out RaycastHit UI, Mathf.Infinity, UILayer)) return;
        //If the mode is add placeables
        if (gridMode == Mode.AddPlaceables)
        {
            if (Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, gridBaseLayer))
            {
                if (!c_GridPrefab) return;
                if (c_Grid != hit.transform)
                {
                    c_Grid = hit.transform;
                    RefreshGrid();
                    RefreshEditGrid(GetPosition(c_Grid.GetSiblingIndex()), c_GridPrefab.GetComponent<GridObject>().gridSize);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    SetGridObject(GetPosition(c_Grid.GetSiblingIndex()));
                }
            }
        }

        //If the mode is demoish structure
        else if (gridMode == Mode.Demolish)
        {
            //Cast a ray from the camera center
            if (Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, gridObjectLayer))
            {
                //If the hit surface is not the surface remembered
                if (c_GridObject != hit.transform.gameObject)
                {
                    //Set the previous object color to the normal color
                    if (c_GridObject) foreach (MeshRenderer child in c_GridObject.GetComponentsInChildren<MeshRenderer>()) { child.material.color = normalGridObjectColor; }
                    //Set the current object color to the chosen color
                    foreach (MeshRenderer child in hit.transform.gameObject.GetComponentsInChildren<MeshRenderer>()) { child.material.color = chosenGridObjectColor; }
                    //Remember the current surface and forget the previous one
                    c_GridObject = hit.transform.gameObject;
                }
                //If the key is pressed
                if (Input.GetMouseButtonDown(0))
                {
                    //Remove the gird object
                    RemoveGridObject();
                    //Refresh the grid visual
                    RefreshGrid();
                }
            }

            //If the ray did not hit anything
            else
            {
                //Set the last hit surface to normal color
                if (c_GridObject) foreach (MeshRenderer child in c_GridObject.GetComponentsInChildren<MeshRenderer>()) { child.material.color = normalGridObjectColor; }
                //Forget the last hit surface
                c_GridObject = null;
            }
        }
    }
    #endregion
}

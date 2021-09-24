using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// This is a script that controls the seeds that the player owns
/// Main functions:
/// 1.Remove or add seed in the seed list
/// 2.Setup and manipulate the seed selection panel
/// 2.Modify the plantable surface
/// </summary>
public class SeedManager : MonoBehaviour
{
    #region Variable declaration
    /// <summary>
    /// Generic settings
    /// </summary>
    //To set the manager active or not [I used in-script set active to keep some of the function active]
    public bool raycastEnabled;

    //The seeds that the character have
    public List<Seed> seedList = new List<Seed>();
    //The object tag that can plant crops
    public string plantableSurfaceTag;
    //The object tag that can choose seeds
    public string seedSelectionPanelTag;

    /// <summary>
    /// The UI section for the seed selection panel
    /// </summary>
    [Header("Seed Panel UI")]
    //The main panel object
    public GameObject seedSelectionPanel;
    //The transform of the grid layout group that holds the options
    public Transform seedOptionsHolder;
    //The seed option in the panel to let the player choose
    public GameObject seedOption;
    //The text UI to show the seed name
    public Text seedName;
    //The text UI to show the grow time of the seed
    public Text seedGrowTime;
    //The text UI to show the description of the seed
    public Text seedDescription;
    //The text UI to show the maximum page
    public Text seedPageMax;
    //The text UI to show the index of the current page
    public Text seedPageCurrent;
    //The button UI to let the player remove the plant from the current surface
    public Button removeSeedButton;
    //The button UI to let the player plant chosen seed on the current surface
    public Button plantSeedButton;

    /// <summary>
    /// The private variables that records temporary values
    /// </summary>
    //Current seed chosen by the player from the panel
    private Seed c_Seed;
    //Current plantable surface the player chosen
    private GameObject c_PlantableSurface;
    //Current page index the player is on in the seed panel
    private int c_SeedPageValue = 1;
    //Total seeds on one page to show
    private const int seedsPerPage = 8;
    //The list to store the seed options for page separations
    //I did not use GetChild(index) because it will not recognize inactive game object
    private List<GameObject> seedUIList = new List<GameObject>();
    //The color to indicate the chosen plantable surface [I made the color darker when chosen]
    private static Color chosenColor = Color.grey;

    /// <summary>
    /// Setting the status of this seed manager
    /// If inactive, It will only manage the seed inventory,
    /// Also the raycast to check plantable surface will be turned off
    /// </summary>
    /// <param name="active">The variable is affecting the raycast check for the player to choose plantable surface.</param>
    public void SetActive(bool active)
    {
        //Set the function to the given active state
        raycastEnabled = active;

        //The panel should always be off no matter enabling or disabling the function
        SeedPanelSetActive(false);
    }
    #endregion

    #region Setup the UI according to the given data
    /// <summary>
    /// Set the UI content in the seed selection panel that changes with the seed selection
    /// </summary>
    /// <param name="seed">The seed data stored in the seed inventory of this manager</param>
    public void LoadSeedPanelUI(Seed seed)
    {
        //Set the name UI
        seedName.text = seed.seedName;
        //Set the grow time UI
        seedGrowTime.text = seed.growTime.ToString()+" Secs";
        //Set the description UI
        seedDescription.text = seed.description;
    }

    /// <summary>
    /// Set the UI content in the seed option of the seed selection panel
    /// </summary>
    /// <param name="seedUI">The attached script that locates the required UI elements</param>
    /// <param name="seed">The seed data stored in the seed inventory of this manager</param>
    public void LoadSeedOptionUI(SeedOptionUI seedUI, Seed seed)
    {
        //Set the icon UI
        seedUI.icon.sprite = seed.icon;
        //Set the amount text UI
        seedUI.amount.text = seed.amount.ToString();
    }

    /// <summary>
    /// Set the UI content that shows the current and total page number
    /// </summary>
    public void LoadSeedPageNumberUI()
    {
        //Refresh the max page number UI
        seedPageMax.text = GetSeedUIPageCount().ToString();
        //Refresh the current page number UI
        seedPageCurrent.text = c_SeedPageValue.ToString();
    }

    /// <summary>
    /// Update the UI in the seed selection panel
    /// </summary>
    public void UpdateUI()
    {
        //Clear all lists
        foreach (Transform child in seedOptionsHolder) { Destroy(child.gameObject); }

        //Clear all recorded display items in list
        seedUIList.Clear();

        //Resetup UI of all seeds in the panel
        //For each of the seed owned by player
        foreach (Seed child in seedList)
        {
            //Spawn the option for this seed in the circular menu
            GameObject option = Instantiate(seedOption, seedOptionsHolder);

            //Add listener to the option button
            //Listener: It will set the UI on the selection panel to the detail of this seed, set the current selected seed to this seed
            option.GetComponentInChildren<Button>().onClick.AddListener(delegate { LoadSeedPanelUI(child); c_Seed = child; });

            //Set the UI content of the option of this seed
            LoadSeedOptionUI(option.GetComponent<SeedOptionUI>(), child);

            //Add to the UI list for page filter
            seedUIList.Add(option);
        }

        //Display the current page in the panel
        ShowSeedUIPage(c_SeedPageValue);
    }
    #endregion

    #region Set the page views in the UI
    /// <summary>
    /// Get the maximum pages that can be separated
    /// </summary>
    /// <returns>Return the maximum pages that can be separated according to total object count</returns>
    public int GetSeedUIPageCount()
    {
        //Get remainder, which means the amount of elements on the unfulfilled last page
        int remainder = seedUIList.Count % seedsPerPage;

        //Get the amount of pages that fully displayed with elements
        int result = seedUIList.Count / seedsPerPage;

        //If there are no unfulfilled pages, use the page amount, if there is, add one page as unfulfilled
        return remainder > 0 ? result + 1 : result;
    }

    /// <summary>
    /// Display the next inventory page
    /// </summary>
    public void ShowSeedUINextPage()
    {
        //If there are no seed displayed in the inventory, displayed or hidden
        if (seedUIList.Count <= 0) return;

        print(GetSeedUIPageCount());

        //If the next page is out of range, warp it to the last page
        if (c_SeedPageValue + 1 > GetSeedUIPageCount())
            ShowSeedUIPage(1);

        //If the next page is in range, display it
        else
            ShowSeedUIPage(c_SeedPageValue + 1);
    }

    /// <summary>
    /// Display the previous inventory page
    /// </summary>
    public void ShowSeedUIPreviousPage()
    {
        //If there are no seed displayed in the inventory, displayed or hidden
        if (seedUIList.Count <= 0) return;

        //If the previous page is out of range, warp it to the last page
        if (c_SeedPageValue - 1 < 1)
            ShowSeedUIPage(GetSeedUIPageCount());

        //If the previous page is in range, display it
        else
            ShowSeedUIPage(c_SeedPageValue - 1);
    }

    /// <summary>
    /// Display the specific inventory page accoring to the given index by enabling only a certain set of UI objects in the list.
    /// </summary>
    /// <param name="page">The number of the page you want to display</param>
    public void ShowSeedUIPage(int page)
    {
        //If the page is lower than zero, return
        if (page <= 0) return;

        //Set current page numbers
        c_SeedPageValue = page;

        //Get and display the current page number and max page number
        LoadSeedPageNumberUI();

        //Hide all seeds in inventory first
        foreach (Transform child in seedOptionsHolder) { child.gameObject.SetActive(false); }

        //Show the seeds with in the page
        for (int i = seedsPerPage * (page - 1); i < seedsPerPage * page; i++)
        {
            //If within the range of elements
            if (i < seedUIList.Count)
                seedUIList[i].gameObject.SetActive(true);
        }

    }
    #endregion

    #region Add or remove seeds from the seed list
    /// <summary>
    /// Add seed to list according to the given data
    /// </summary>
    /// <param name="seed">The seed data stored in the game object</param>
    public void AddSeedToList(Seed seed)
    {
        //Find seed in the inventory according to the given ID
        Seed sameSeed = seedList.Find(x => x.uniqueID == seed.uniqueID);

        //If there is a same seed type that is already in the inventory
        if (sameSeed != null)
        {
            //Add the amount of the seed in the inventory
            sameSeed.amount += seed.amount;
        }
        //If this is a new type of seed that the player dont have
        else
        {
            //Add new seed data to the inventory
            seedList.Add(seed);
        }

        //Refresh UI
        UpdateUI();
    }

    /// <summary>
    /// Remove all the seeds of the same type from list according to the given unique ID
    /// </summary>
    /// <param name="uniqueID">The unique identifier that every seed have</param>
    public void RemoveSeedFromList(string uniqueID)
    {
        //Remove found item from the inventory
        seedList.RemoveAll(x => x.uniqueID == uniqueID);

        //Refresh UI
        UpdateUI();
    }

    /// <summary>
    /// Remove certain amount of seed from list according to the given amount and unique ID
    /// </summary>
    /// <param name="uniqueID">The unique identifier that every seed have</param>
    /// <param name="amount">The amount to delete if the seeds with the ID are found</param>
    public void RemoveSeedFromList(string uniqueID, int amount)
    {
        //Remove certain amount of item from the inventory
        Seed match = seedList.Find(x => x.uniqueID == uniqueID);

        //Reduce count
        match.amount -= amount;

        //If count <=0 remove the item from inventory
        if (match.amount <= 0)
            seedList.Remove(match);

        //Refresh UI
        UpdateUI();
    }
    #endregion

    #region Raycast and operations
    /// <summary>
    /// Plant seed on the current plantable surface
    /// </summary>
    /// <param name="seed">The seed data</param>
    public void PlantSeedOnSurface(Seed seed) => c_PlantableSurface.GetComponent<PlantableSurfaceManager>().Plant(seed.seedName, seed.icon, seed.growTime, seed.profit);

    /// <summary>
    /// Remove seed from the current plantable surface
    /// </summary>
    public void RemoveSeedFromSurface() => c_PlantableSurface.GetComponent<PlantableSurfaceManager>().Remove();

    /// <summary>
    /// Set the active status of the seed selection panel by toggling the bool parameter in the animator
    /// The animation is not included in the code
    /// </summary>
    /// <param name="active">The active state of the seed selection panel</param>
    private void SeedPanelSetActive(bool active) { seedSelectionPanel.GetComponent<Animator>().SetBool("Enabled", active); }

    /// <summary>
    /// Check if the selection panel is in the mid way of opening or closing animation
    /// By using this function, we can get smoother animation [The panel will not close until it is fully open]
    /// </summary>
    /// <returns>Return true if the animation is ongoing, false if the animation is not ongoing</returns>
    private bool SeedPanelGetPlayState() { return seedSelectionPanel.GetComponent<Animator>().IsInTransition(0); }

    /// <summary>
    /// Set the color of the material on the target game object if applicable
    /// </summary>
    /// <param name="target">Target game object</param>
    /// <param name="color">Target color</param>
    private void SetMaterialColor(GameObject target, Color color) { if (target && target.TryGetComponent(out MeshRenderer renderer)) renderer.material.color = color; }

    /// <summary>
    /// Update is called once per frame, check if it hits plantable surface or seed selection panel
    /// </summary>
    public void Update()
    {
        //Return if raycast triggering is not enabled
        if (!raycastEnabled) return;

        //Get the main camera transform
        Transform camera = Camera.main.transform;
        //Cast a ray from camera center
        Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity);

        //If nothing useful is hit [it did not hit seed selection panel or plantable surface]
        //Reset the surface color, forget the surface, close the panel
        if (hit.transform == null || (hit.transform.tag != seedSelectionPanelTag && hit.transform.tag != plantableSurfaceTag)) {
            //Set the color of the current plantable surface [The surface that we just looked away] to normal
            SetMaterialColor(c_PlantableSurface, Color.white);
            //Remove the surface from current
            c_PlantableSurface = null;
            //If the selection panel has totally open
            if(!SeedPanelGetPlayState())
                //Close the selection panel
                SeedPanelSetActive(false);
            //Exit
            return;
        }

        //If not hitting the seed selection panel
        //Close the panel
        if (hit.transform.tag != seedSelectionPanelTag && !SeedPanelGetPlayState()) 
        {
            SeedPanelSetActive(false); 
        }

        //If hit the plantable surface
        //Set surface color, set button listener if pressed on it
        if (hit.transform.tag == plantableSurfaceTag)
        {
            //If the hit surface is not the surface it remembered
            if (c_PlantableSurface != hit.transform.gameObject)
            {
                //Set the previous surface that it remembered to normal color
                SetMaterialColor(c_PlantableSurface, Color.white);
                //Set the selected surface the ray hit to chosen color
                SetMaterialColor(hit.transform.gameObject, chosenColor);
                //Override the previous surface with the new surface
                c_PlantableSurface = hit.transform.gameObject;
            }
            //If button press when looking at the surface
            if (Input.GetMouseButtonDown(0) && !SeedPanelGetPlayState())
            {
                //Set the distance of the seed selection panel to the player's eyes
                float viewDistance = 1f;
                //The vector that stores the direction and distance from the camera to the hit point
                Vector3 distanceVector = camera.position - hit.point;

                //Set panel position to the hit point
                seedSelectionPanel.transform.position = hit.point;
                //Bring panel to the camera using the vector and leave a certain distance
                seedSelectionPanel.transform.Translate(distanceVector - distanceVector.normalized * viewDistance, Space.World);
                //Face the panel to camera
                seedSelectionPanel.transform.forward = -distanceVector.normalized;
                
                //Remove all listeners on the add and remove button
                removeSeedButton.onClick.RemoveAllListeners();
                plantSeedButton.onClick.RemoveAllListeners();

                //Add listener to the remove button
                removeSeedButton.onClick.AddListener(delegate {
                    //Remove the plant on the current surface
                    RemoveSeedFromSurface(); 
                });

                //Add listener to the add button
                plantSeedButton.onClick.AddListener(delegate { 
                    //Plant current seed on the current surface
                    PlantSeedOnSurface(c_Seed);
                    //Remove one seed of the type that just planted [Cost one seed]
                    RemoveSeedFromList(c_Seed.uniqueID, 1);  
                    //Close the seed selection panel
                    SeedPanelSetActive(false); 
                });

                //Show the seed selection panel
                SeedPanelSetActive(true);
            }
        }
    }
    #endregion
}
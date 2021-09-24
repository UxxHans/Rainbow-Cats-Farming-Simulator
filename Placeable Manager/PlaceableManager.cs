using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// The script stores all the placeable buildings and lands of the player
/// Also it manage the interaction panel to display placeable options for player
/// </summary>
public class PlaceableManager : MonoBehaviour
{
    #region Variable Declaration
    //The list of all placeable that the player have
    public List<Placeable> placeableList = new List<Placeable>();

    [Header("Placeable Panel UI")]
    //The Whole UI of the panel
    public GameObject placeableComponent;
    //The placeable selection panel
    public GameObject placeableSelectionPanel;
    //The button that hide the placeable selection panel
    public GameObject placeablePanelShowButton;
    //The transform that holds all the options
    public Transform placeableOptionsHolder;
    //The placeable option to spawn in the transform
    public GameObject placeableOption;
    //The maximum pages text UI
    public Text placeablePageMax;
    //The current page text UI
    public Text placeablePageCurrent;

    //The current placeable page value
    private int c_PlaceablePageValue = 1;
    //The maximum options to show on each page
    private const int placeablesPerPage = 8;
    //The list to record all the placeable options for page toggle
    //As get child is not working for the inactive objects, so we have to remember all the options
    private List<GameObject> placeableUIList = new List<GameObject>();
    #endregion

    #region Placeable panel UI managing
    /// <summary>
    /// Hide or show the selection panel
    /// </summary>
    /// <param name="active"></param>
    public void SetPanelUIStatus(bool active)
    {
        placeableSelectionPanel.SetActive(active);
        placeablePanelShowButton.SetActive(!active);
    }


    /// <summary>
    /// Set the manager Interface active
    /// </summary>
    /// <param name="active">Status</param>
    public void SetActive(bool active)
    {
        placeableComponent.SetActive(active);
        placeableSelectionPanel.SetActive(active);
        placeablePanelShowButton.SetActive(false);
    }

    /// <summary>
    /// Update the options UI in the placeable panel
    /// </summary>
    public void UpdateUI()
    {
        //Clear all lists
        foreach (Transform child in placeableOptionsHolder) { Destroy(child.gameObject); }

        //Clear all recorded display items in list
        placeableUIList.Clear();

        foreach (Placeable child in placeableList)
        {
            //Spawn an option in the circular menu
            GameObject slot = Instantiate(placeableOption, placeableOptionsHolder);

            //Get grid manager
            GridManager gridManager = FindObjectOfType<GridManager>();

            //Get control manager
            ControlCenter controlCenter = FindObjectOfType<ControlCenter>();

            //Add listener to the option button
            slot.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                gridManager.SetPlaceable(child);
                SetPanelUIStatus(false);
            });

            //Show information on the option
            LoadPlaceableOptionUI(slot.GetComponent<PlaceableOptionUI>(), child);

            //Add to the UI list for page filter
            placeableUIList.Add(slot);
        }

        //Display the page
        ShowPlaceableUIPage(c_PlaceablePageValue);
    }

    /// <summary>
    /// Show the placeable option UI according to the given placeable data
    /// </summary>
    /// <param name="placeableUI">The UI component list</param>
    /// <param name="placeable">The placeable data</param>
    public void LoadPlaceableOptionUI(PlaceableOptionUI placeableUI, Placeable placeable)
    {
        placeableUI.icon.sprite = placeable.icon;
        placeableUI.amount.text = placeable.amount.ToString();
        placeableUI.placeableName.text = placeable.placeableName;
        placeableUI.uniqueID.text = placeable.uniqueID;
        placeableUI.description.text = placeable.description;
        placeableUI.brief.text = placeable.brief;
    }

    /// <summary>
    /// Return the maximum pages of current display according to the total options and options per page
    /// </summary>
    /// <returns>Returns the total amount of the pages</returns>
    public int GetPlaceableUIPageCount()
    {
        //Get remainder, which means the amount of elements on the unfulfilled last page
        int remainder = placeableUIList.Count % placeablesPerPage;

        //Get the amount of pages that fully displayed with elements
        int result = placeableUIList.Count / placeablesPerPage;

        //If there are no unfulfilled pages, use the page amount, if there is, add one page as unfulfilled
        return remainder > 0 ? result + 1 : result;
    }

    /// <summary>
    /// Display the next inventory page
    /// </summary>
    public void ShowPlaceableUINextPage()
    {
        //If there are no placeable displayed in the inventory, displayed or hidden
        if (placeableUIList.Count <= 0) return;

        print(GetPlaceableUIPageCount());

        //If the next page is out of range, warp it to the last page
        if (c_PlaceablePageValue + 1 > GetPlaceableUIPageCount())
            ShowPlaceableUIPage(1);

        //If the next page is in range, display it
        else
            ShowPlaceableUIPage(c_PlaceablePageValue + 1);
    }

    /// <summary>
    /// Display the previous inventory page
    /// </summary>
    public void ShowPlaceableUIPreviousPage()
    {
        //If there are no placeable displayed in the inventory, displayed or hidden
        if (placeableUIList.Count <= 0) return;

        //If the previous page is out of range, warp it to the last page
        if (c_PlaceablePageValue - 1 < 1)
            ShowPlaceableUIPage(GetPlaceableUIPageCount());

        //If the previous page is in range, display it
        else
            ShowPlaceableUIPage(c_PlaceablePageValue - 1);
    }

    /// <summary>
    /// Display the specific inventory page accoring to the given index
    /// </summary>
    /// <param name="page">Page index</param>
    public void ShowPlaceableUIPage(int page)
    {
        //If the page is lower than zero, return
        if (page <= 0) return;

        //Set current page numbers
        c_PlaceablePageValue = page;

        //Get and display the current page number and max page number
        LoadPlaceableUIPageNumber();

        //Hide all placeables in inventory first
        foreach (Transform child in placeableOptionsHolder) { child.gameObject.SetActive(false); }

        //Show the placeables with in the page
        for (int i = placeablesPerPage * (page - 1); i < placeablesPerPage * page; i++)
        {
            //If within the range of elements
            if (i < placeableUIList.Count)
                placeableUIList[i].gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// Refresh the page number UI
    /// </summary>
    public void LoadPlaceableUIPageNumber()
    {
        //Set the maximum page text UI
        placeablePageMax.text = GetPlaceableUIPageCount().ToString();
        //Set the current page text UI
        placeablePageCurrent.text = c_PlaceablePageValue.ToString();
    }
    #endregion

    #region Placeable list operations
    /// <summary>
    /// Add placeable to the list
    /// </summary>
    /// <param name="placeable">Placeable data</param>
    public void AddPlaceable(Placeable placeable)
    {
        //Find the same placeable in list
        Placeable samePlaceable = placeableList.Find(x => x.uniqueID == placeable.uniqueID);

        //If there are same placeables
        if (samePlaceable != null)
        {
            //Just add the amount of it
            samePlaceable.amount += placeable.amount;
        }
        //If there are no such placeable
        else
        {
            //Add the placeable to the list
            placeableList.Add(placeable);
        }

        //Refresh UI
        UpdateUI();
    }

    /// <summary>
    /// Remove all placeables with the given ID from list
    /// </summary>
    /// <param name="uniqueID">The ID of placeable</param>
    public void RemovePlaceable(string uniqueID)
    {
        //Remove item from the inventory
        placeableList.RemoveAll(x => x.uniqueID == uniqueID);

        //Refresh UI
        UpdateUI();
    }

    /// <summary>
    /// Remove placeable from list according to the given amount and id
    /// </summary>
    /// <param name="uniqueID">The ID of placeable</param>
    /// <param name="amount">The amount to remove</param>
    public void RemovePlaceable(string uniqueID, int amount)
    {
        //Remove certain amount of item from the inventory
        Placeable match = placeableList.Find(x => x.uniqueID == uniqueID);

        //Reduce count
        match.amount -= amount;

        //If count <=0 remove the item from inventory
        if (match.amount <= 0)
            placeableList.Remove(match);

        //Refresh UI
        UpdateUI();
    }

    /// <summary>
    /// Check if the place with the given unique ID exists
    /// </summary>
    /// <param name="uniqueID">The target unique ID</param>
    /// <returns>Return true if placeable exists, false if it doesnt</returns>
    public bool PlaceableExist(string uniqueID)
    {
        //If the placeable is found
        if (placeableList.Find(x => x.uniqueID == uniqueID) != null)
            return true;
        //If the placeable is not found
        else
            return false;
    }
    #endregion
}

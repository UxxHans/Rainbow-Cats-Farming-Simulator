using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// The script that manage the sale items in the shop
/// It restocks the shop with random its in certain amount of time
/// It also manage the interaction panel of the shop
/// </summary>
public class ShopManager : MonoBehaviour
{
    //The list of all items the shop have
    public List<GameObject> itemLibrary = new List<GameObject>();
    //Cargo in stock time
    public float inStockTime;
    //Max items in stock
    public int itemsPerStock;
    //Current items in stock
    private List<GameObject> c_Stock = new List<GameObject>();
    //Current restock remain time
    private float c_RefreshTime;

    [Header("UI Settings")]
    //The interaction panel of the shop
    public GameObject shopOptionUI;
    //The transform that stores all the options of the shop
    public Transform shopOptionUIHolder;
    //The countdown timer for next stock refresh
    public Text stockRefreshCountdown;

    [Header("Item Spawn Settings")]
    //The transfrom that spawn bought item
    public Transform spawnTransform;
    //The random range for the spawn
    public Vector2 spawnRandomRange;

    /// <summary>
    /// Initialization of the shop
    /// </summary>
    public void Start()
    {
        //Return if there is no item at all
        if (itemLibrary.Count == 0) return;
        //Check stock and UI
        InvokeRepeating(nameof(CheckRestock), 0, 0.2f);
        InvokeRepeating(nameof(RefreshRestockCountdownUI), 0, 0.2f);
    }

    /// <summary>
    /// Initialization and update of the shop panel
    /// </summary>
    public void RefreshShopPanelUI()
    {
        //Clear the UI
        foreach (Transform child in shopOptionUIHolder) Destroy(child.gameObject);
        //Setup the UI
        foreach(GameObject child in c_Stock)
        {
            //Spawn a option
            GameObject instance = Instantiate(shopOptionUI, shopOptionUIHolder);
            //Get required UI component
            ShopOptionsUI UI = instance.GetComponent<ShopOptionsUI>();

            //Set the content of the UI

            if(child.TryGetComponent(out SeedSettings seedSettings))
            {
                UI.itemIcon.sprite = seedSettings.seedProperties.icon;
                UI.itemName.text = seedSettings.seedProperties.seedName.ToString();
                UI.itemDescription.text = seedSettings.seedProperties.description.ToString();
                UI.itemBrief.text = "Grow Time: " + seedSettings.seedProperties.growTime.ToString() + "s";
                UI.cost.text = seedSettings.seedProperties.cost.ToString();
                //Add the button event
                instance.GetComponentInChildren<Button>().onClick.AddListener(delegate { BuyItem(child, seedSettings.seedProperties.cost); });
            }
            if (child.TryGetComponent(out PlaceableSettings placeableSettings))
            {
                UI.itemIcon.sprite = placeableSettings.placeableProperties.icon;
                UI.itemName.text = placeableSettings.placeableProperties.placeableName.ToString();
                UI.itemDescription.text = placeableSettings.placeableProperties.description.ToString();
                UI.itemBrief.text = placeableSettings.placeableProperties.brief.ToString();
                UI.cost.text = placeableSettings.placeableProperties.cost.ToString();
                //Add the button event
                instance.GetComponentInChildren<Button>().onClick.AddListener(delegate { BuyItem(child, placeableSettings.placeableProperties.cost); });
            }

        }
    }

    /// <summary>
    /// Load a random item batch from the library to current stock
    /// </summary>
    public void LoadRandomItemBatch()
    {
        //Clean the stock
        c_Stock = new List<GameObject>();

        List<int> indexList = new List<int>();
        for (int i = 0; i < itemsPerStock; i++)
        {
            //Get random index
            int randomIndex = Random.Range(0, itemLibrary.Count);
            indexList.Add(randomIndex);
        } 

        for(int a = 0; a < indexList.Count; a++)
        {
            //Load stock
            c_Stock.Add(itemLibrary[indexList[a]]);
        }
    }

    /// <summary>
    /// Refresh UI and current stock, set next restock
    /// </summary>
    public void CheckRestock()
    {
        //It the refresh time not passed, return
        if (c_RefreshTime > Time.time) return;
        //Refresh stock
        LoadRandomItemBatch();
        //Refresh UI
        RefreshShopPanelUI();
        //Set next refresh time
        c_RefreshTime = Time.time + inStockTime;
    }

    /// <summary>
    /// Refresh UI that shows the remain time of the next shop stock
    /// </summary>
    public void RefreshRestockCountdownUI()
    {
        float remainTime = c_RefreshTime - Time.time;
        stockRefreshCountdown.text = remainTime.ToString("0.00");
    }

    /// <summary>
    /// The function that will cost player money in exchange of some items
    /// </summary>
    /// <param name="item">Item data</param>
    public void BuyItem(GameObject item, int cost)
    {
        //Try to buy an item
        if (FindObjectOfType<FinanceManager>().TryBuy(cost))
        {
            //It we have enough money, get a random spawn position
            Vector3 pos = spawnTransform.position;
            float ranX = Random.Range(pos.x - spawnRandomRange.x, pos.x + spawnRandomRange.x);
            float ranZ = Random.Range(pos.z - spawnRandomRange.y, pos.z + spawnRandomRange.y);
            //Spawn the item at the position
            Instantiate(item, new Vector3(ranX, pos.y, ranZ), Quaternion.Euler(0, Random.Range(-90, 90), 0), spawnTransform);
        }
    }

    /// <summary>
    /// Show the random spawn range in the editor
    /// </summary>
    public void OnDrawGizmosSelected() => Gizmos.DrawWireCube(spawnTransform.position, new Vector3(spawnRandomRange.x * 2, 0, spawnRandomRange.y * 2));
}

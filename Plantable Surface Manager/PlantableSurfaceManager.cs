using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// This script is an attachable script that manage the plant on the surface
/// It can remove or add plant to the surface and monitor the grow time and harvest profit
/// </summary>
public class PlantableSurfaceManager : MonoBehaviour
{
    #region Variable declaration
    [Header("Buff Settings")]
    //The Times That it reduces
    public float reduceTimeConstant;
    public float reduceTimeMultiply;
    //The money that it earns
    public int addProfitConstant;
    public float addProfitMultiply;

    [Header("Plant Information Panel UI")]
    //The text UI of the plant name
    public Text plantNameUI;
    //The icon UI of the plant
    public Image plantIconUI;
    //The times remain to grow mature
    public Text remainTimeUI;
    //The image to show the progress of the growth [Progress bar]
    public Image progressUI;
    //The Button UI for the player to harvest the crop
    public Button harvestUI;

    //The transform to hold the plant model
    public Transform modelHolder;

    //The name of the current plant
    private string plantName;
    //The icon of the current plant
    private Sprite plantIcon;
    //The profit of the current plant
    private int plantProfit;
    //The total grow time of the current plant
    private float totalTime;
    //The remain time to grow mature for the current plant
    private float remainTime;

    //The status of current plant
    private int status;
    //Representation of the status as constant
    private const int EMPTY = 0;
    private const int GROWING = 1;
    private const int FINISHED = 2;
    private const float TICKRATE = 1f;
    #endregion

    #region Initialization and status check
    /// <summary>
    /// Initialization that start the countdown of the grow time
    /// </summary>
    void Start()
    {
        //Hide the UI
        SetActiveUI(false);
        //Hide the harvest Button
        SetHarvestUI(false);
        //If the current status is growing, start the time countdown
        if (status==GROWING) InvokeRepeating(nameof(CheckGrowProgress), 0, TICKRATE);
    }

    /// <summary>
    /// Check current grow progress and react if the time reached certain points
    /// </summary>
    private void CheckGrowProgress()
    {
        //Reduce time by the check rate
        remainTime -= TICKRATE;

        //Show the UI
        SetActiveUI(true);

        //Update the UI content
        UpdateUI();

        //Calculate the percentage of the growth
        float ratio = remainTime / totalTime;

        //If is mature
        if (ratio <= 0)
        {
            //Set button event on the harvest option
            harvestUI.onClick.AddListener(delegate { Harvest(); });
            //Show the harvest option
            SetHarvestUI(true);
            //Set the status to finished
            status = FINISHED;
            //Stop checking the plant
            CancelInvoke();
            //Return
            return;
        }
        else
        {
            //Set the grow progress model
            if (ratio < 1.0f && ratio > 0.6f) { RemoveModel(); SetModel((GameObject)Resources.Load("Prefab/Plant Progress/Prefabs/1", typeof(GameObject))); return; }
            if (ratio < 0.6f && ratio > 0.3f) { RemoveModel(); SetModel((GameObject)Resources.Load("Prefab/Plant Progress/Prefabs/2", typeof(GameObject))); return; }
            if (ratio < 0.3f && ratio > 0.0f) { RemoveModel(); SetModel((GameObject)Resources.Load("Prefab/Plant Progress/Prefabs/3", typeof(GameObject))); return; }
        }

    }
    #endregion

    #region Modify the UI
    /// <summary>
    /// Update the content on the UI according to the stored values
    /// </summary>
    private void UpdateUI()
    {
        progressUI.fillAmount = remainTime / totalTime;
        plantNameUI.text = plantName;
        plantIconUI.sprite = plantIcon;
        remainTimeUI.text = remainTime.ToString("0.00");
        remainTimeUI.gameObject.SetActive(remainTime <= 0 ? false : true);
    }

    /// <summary>
    /// Hide or show the plant UI, except the harvest button
    /// </summary>
    /// <param name="active"></param>
    private void SetActiveUI(bool active)
    {
        progressUI.gameObject.SetActive(active);
        plantNameUI.gameObject.SetActive(active);
        plantIconUI.gameObject.SetActive(active);
        remainTimeUI.gameObject.SetActive(active);
    }

    /// <summary>
    /// Hide or show the harvest button
    /// </summary>
    /// <param name="active">Active status</param>
    private void SetHarvestUI(bool active) => harvestUI.gameObject.SetActive(active);
    #endregion

    #region Main plant functions
    /// <summary>
    /// Add model to the plot transform
    /// </summary>
    /// <param name="prefab">The plant prefab</param>
    private void SetModel(GameObject prefab) { Instantiate(prefab, modelHolder); }

    /// <summary>
    /// Remove model from the plot transform
    /// </summary>
    private void RemoveModel() { foreach (Transform child in modelHolder) Destroy(child.gameObject); }

    /// <summary>
    /// Remove the plant from the surface
    /// </summary>
    public void Remove()
    {
        //Cancel checking the plant growth
        CancelInvoke();
        //Hide the UI
        SetActiveUI(false);

        //Set the status to empty so the surface become plantable
        status = EMPTY;
        //Empty the name
        plantName = null;
        //Empty the icon
        plantIcon = null;
        //Empty the total time
        totalTime = 0;
        //Empty the remain time
        remainTime = 0;

        //Remove the model from the surface
        RemoveModel();
    }

    /// <summary>
    /// Plant the seed on the surface
    /// </summary>
    /// <param name="name">The name of the plant</param>
    /// <param name="icon">The icon of the plant</param>
    /// <param name="seconds">The time it takes to grow</param>
    /// <param name="profit">The money it will earn on harvest</param>
    public void Plant(string name, Sprite icon, float seconds, int profit)
    {
        //If the surface has a plant occupied, return
        if (status==GROWING||status==FINISHED) return;
        //Store the name
        plantName = name;
        //Store the icon
        plantIcon = icon;
        //Store the profit
        plantProfit = Mathf.RoundToInt(profit * addProfitMultiply) + addProfitConstant;
        //Store the remain time as full or with buff
        remainTime = seconds/reduceTimeMultiply - reduceTimeConstant;
        //Store the full grow time
        totalTime = seconds;
        //Start to check the grow progress
        InvokeRepeating(nameof(CheckGrowProgress), 0, TICKRATE);
    }

    /// <summary>
    /// Harvest the current plant
    /// </summary>
    public void Harvest() {
        //Hide the harvest button
        SetHarvestUI(false);
        //Find the money manager and add money
        FindObjectOfType<FinanceManager>().Earn(plantProfit);
        //Remove the plant
        Remove();
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// The script that can manage the money of the player
/// It can try to reduce money and return if the money is suffecient or not
/// </summary>
public class FinanceManager : MonoBehaviour
{
    //The money that the player have currently
    public int money;
    //The money Text UI to show the amount
    public Text moneyUI;

    [Header("Sound Settings")]
    //The sound indicates success purchase
    public AudioClip purchaseSound;
    //The sound indicates insufficient funds
    public AudioClip insufficientSound;
    //The sound indicates insufficient funds
    public AudioClip earnProfitSound;

    //Refresh the money amount UI at the beginning
    public void Start() => RefreshUI();

    //Refresh the UI by setting the text
    public void RefreshUI() => moneyUI.text = money.ToString();

    /// <summary>
    /// Try to buy an item, if the cost is larger than the money we have, return false
    /// If the money is suffecient, we just reduce the money from the variable
    /// </summary>
    /// <param name="cost">The cost of the item</param>
    /// <returns>Return true is the money is suffecient and reduced, return false if the money is insuffecient</returns>
    public bool TryBuy(int cost)
    {
        //Return false if the cost is larger than money
        if (money < cost)
        {
            //Play the insufficient sound
            GetComponentInChildren<AudioSource>().PlayOneShot(insufficientSound);
            return false;
        }

        //If money is suffecient, reduce cost
        money -= cost;
        //Play the success sound
        GetComponentInChildren<AudioSource>().PlayOneShot(purchaseSound);
        //Refresh the money text UI
        RefreshUI();
        //Return true
        return true;
    }

    /// <summary>
    /// Add money to the player
    /// </summary>
    /// <param name="profit">Amount to add</param>
    public void Earn(int profit)
    {
        //Play earn sound
        GetComponentInChildren<AudioSource>().PlayOneShot(earnProfitSound);
        //Add money
        money += profit;
        //Refresh the money text UI
        RefreshUI();
    }
}

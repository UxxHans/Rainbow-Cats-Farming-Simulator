using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// This script is an attachable script on the pickable seed object
/// It can help to setup the seed quickly
/// It can manage the icon on the object and set the pickup event on the button
/// NOTICE: As the script setup the first image and button it founds,
/// place the components at the top of the child hierarchy of the object
/// </summary>
public class SeedSettings : MonoBehaviour
{
    //The seed property that is attached on the object
    public Seed seedProperties;

    // Start is called before the first frame update, It setups the pickable seed object
    public void Start()
    {
        //Find the seed manager in the scene
        SeedManager seedManager = FindObjectOfType<SeedManager>();

        //Find the icon in the lower hierarchy of the object
        Image icon = GetComponentInChildren<Image>();
        //Find the button in the lower hierarchy of the object
        Button button = GetComponentInChildren<Button>();

        //Set the sprite of the icon according to the settings
        icon.sprite = seedProperties.icon;
        //Set the pickup button event
        button.onClick.AddListener(delegate {
            seedManager.AddSeedToList(seedProperties);
            Destroy(gameObject);
        });
    }
}

/// <summary>
/// A class that includes all the elemental data of a seed
/// </summary>
/// Making it serializable enables it to be seen and modified in the unity inspector
[System.Serializable]
public class Seed
{
    //The icon of the seed
    public Sprite icon;
    //The name of the seed
    public string seedName;
    //The unique ID of the seed, it is used to identify replication of seeds
    public string uniqueID;
    //Description of the seed
    public string description;
    //The grow time of the seed [In seconds]
    [Range(1, 600)] public float growTime;
    //The amount of the seed
    [Range(1, 10)] public int amount;
    //The money player will receive when harvest
    public int profit;
    //Cost to buy seed
    public int cost;
}

using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// The attachable script on the pickable object to store data
/// </summary>
public class PlaceableSettings : MonoBehaviour
{
    //The placeable data
    public Placeable placeableProperties;

    // Start is called before the first frame update
    public void Start()
    {
        //Find the button in the lower hierarchy of the object
        Image image = GetComponentInChildren<Image>();
        //Find the icon in the lower hierarchy of the object
        Text name = GetComponentInChildren<Text>();
        //Find the button in the lower hierarchy of the object
        Button button = GetComponentInChildren<Button>();
        //Set the image in UI
        image.sprite = placeableProperties.icon;
        //Set the text in UI
        name.text = placeableProperties.placeableName;
        //Find the placeable manager
        PlaceableManager placeableManager = FindObjectOfType<PlaceableManager>();
        //Setup the pickup button event
        button.onClick.AddListener(delegate {
            //Add to the manager
            placeableManager.AddPlaceable(placeableProperties);
            //Destroy the object
            Destroy(gameObject);
        });
    }
}

/// <summary>
/// The placeable data class
/// </summary>
[System.Serializable]
public class Placeable
{
    public GameObject prefab;
    public Sprite icon;
    public string placeableName;
    public string uniqueID;
    public string brief;
    [TextArea]public string description;
    [Range(1, 10)] public int amount;
    public int cost;
}

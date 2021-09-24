using UnityEngine;

/// <summary>
/// This script controls the toggle between different functions
/// </summary>
public class ControlCenter : MonoBehaviour
{
    //Variables declaration
    public TeleportManager teleportManager;
    public SeedManager seedManager;
    public GridManager gridManager;
    public PlaceableManager placeableManager;

    public void SetFunction(int functionIndex)
    {
        //Set all to inactive
        placeableManager.SetActive(false);
        seedManager.SetActive(false);
        gridManager.SetActive(false);
        teleportManager.SetActive(false);

        //Set active selected function
        switch (functionIndex)
        {
            case -1:
                break;
            case 0:
                teleportManager.SetActive(true);
                break;
            case 1:
                seedManager.SetActive(true);
                break;
            case 2:
                gridManager.SetActive(true);
                placeableManager.SetActive(true);
                break;
        }
    }
}

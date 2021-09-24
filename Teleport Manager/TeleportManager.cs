using UnityEngine;

/// <summary>
/// This function enables the player to be teleported in the map by using raycast
/// </summary>
public class TeleportManager : MonoBehaviour
{
    //Is the teleport function active
    public bool raycastActive;

    //The object tag that is recognized as 'Teleportable surface'
    public string teleportableTag;
    //The indication of the teleport position
    public GameObject teleportIndicator;
    //The object to be teleported
    public GameObject playerHolder;

    /// <summary>
    /// Set the teleport raycast function state
    /// </summary>
    /// <param name="value">Active state</param>
    public void SetActive(bool value) 
    { 
        //Set the bool value
        raycastActive = value;
        //Always hide the indicator
        teleportIndicator.SetActive(false); 
    }

    /// <summary>
    /// Update is called once per frame to check the teleportable surface to teleport
    /// </summary>
    public void Update()
    {
        //If not enabled, return
        if (!raycastActive) return;

        //If we detect the hold of the button
        if (Input.GetMouseButton(0))
        {
            //Cast a ray from the main camera.
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
            {
                //If the hit object is the teleportable surface
                if (hit.transform.tag == teleportableTag)
                {
                    //Show the indicator so the player will see where to teleport
                    teleportIndicator.SetActive(true);

                    //Move the indicator to the teleport position
                    teleportIndicator.transform.position = hit.point;
                }

                //If its not a teleportable surface
                else
                {
                    //Hide the indicator
                    teleportIndicator.SetActive(false);
                }
            }
        }

        //Or if the mouse button is up
        else if (Input.GetMouseButtonUp(0))
        {
            //If the indicator is not hidden
            if (teleportIndicator.activeSelf)
            {
                //Hide the indicator
                teleportIndicator.SetActive(false);
                
                //Teleport the player to the position of the teleport indicator
                playerHolder.transform.position = teleportIndicator.transform.position;
            }
        }
    }
}

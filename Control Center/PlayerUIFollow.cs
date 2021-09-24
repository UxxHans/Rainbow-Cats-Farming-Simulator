using UnityEngine;

/// <summary>
/// Make the UI follow the player's look direction when y rotation exceeds 180 Deg
/// </summary>
public class PlayerUIFollow : MonoBehaviour
{
    //Rotate axis
    public enum Axis { x, y, z };
    public Axis rotateAxis;

    //Record original
    private Vector3 originalRotation;
    
    /// <summary>
    /// Start repeating check the look direction
    /// </summary>
    private void Start()
    {
        //Record original rotation
        originalRotation = transform.eulerAngles;
        // Use slow update to improve the performance
        InvokeRepeating(nameof(Check), 0, 0.25f);
    }

    /// <summary>
    /// Check the player's look direction and adjust the UI postion
    /// </summary>
    private void Check()
    {
        //Get player's y rotation
        float angle = Camera.main.transform.eulerAngles.y;
        //UI follow the player's angle
        switch (rotateAxis) {
            case Axis.x:
                if (angle < 180 && angle > 0)
                    gameObject.transform.eulerAngles = new Vector3(90, originalRotation.y, originalRotation.z);
                else
                    gameObject.transform.eulerAngles = new Vector3(270, originalRotation.y, originalRotation.z);
                break;
            case Axis.y:
                if (angle < 180 && angle > 0)
                    gameObject.transform.eulerAngles = new Vector3(originalRotation.x, 90, originalRotation.z);
                else
                    gameObject.transform.eulerAngles = new Vector3(originalRotation.x, 270, originalRotation.z);
                break;
            case Axis.z:
                if (angle < 180 && angle > 0)
                    gameObject.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, 90);
                else
                    gameObject.transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, 270);
                break;
        }
    }
}

using UnityEngine;

public class CircularLayoutGroup : MonoBehaviour
{
    //The total elements in the circular layout
    public int totalElements;

    //The child index in the element that will not rotate
    private int fixedSubelementIndex = 0;

    //If any element is added to the layout group, equally distribute the elements
    public void OnTransformChildrenChanged()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //Calculate the rotation angle of each element
            float rotateAngle = (360 / (float)totalElements) * i;

            //Get transforms
            Transform c_Element = transform.GetChild(i);
            Transform c_SubElement = c_Element.GetChild(fixedSubelementIndex);

            //Set rotation of the element and reset the rotation of the grand child
            c_Element.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -rotateAngle);
            c_SubElement.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, rotateAngle);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    [Header("GUI")]
    public Image iconPrefab;
    public string buttonName;
    public float wheelRadius;
    [Range(-180, 180)]
    public float rotationOffset;

    int slots;
    int selectedIndex;
    Image[] wheelIcons = new Image[1];
    bool wasActive;

    public void WheelHandler()
    {
        if (Input.GetButton(buttonName) && slots > 1) // If button is pressed and there is another slot to swap to
        {
            gameObject.SetActive(true);
            wasActive = true;

            #region Calculate index

            Vector3 relativeMousePosition = Input.mousePosition - transform.position;
            float selectAngle = -Vector3.SignedAngle(transform.up, relativeMousePosition, transform.forward);
            // Alternate functions for analog stick
            //Vector3 relativeControllerPosition = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //float selectAngle = -Vector3.SignedAngle(wheelBase.up, relativeControllerPosition, wheelBase.forward);

            selectAngle -= rotationOffset; // Angle is changed based on rotationOffset to account for the changed positions of the icons based on rotationOffset

            if (selectAngle < 0)
            {
                selectAngle += 360;
            }
            float segmentSize = 360 / slots;
            int index = Mathf.RoundToInt(selectAngle / segmentSize);
            if (index >= slots)
            {
                index = 0;
            }
            selectedIndex = index;
            #endregion
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void RefreshWheel(int slotCount, Sprite[] icons)
    {
        slots = slotCount;
        if (wheelIcons.Length != slots)
        {
            foreach (Image i in wheelIcons)
            {
                if (i != null)
                {
                    Destroy(i.gameObject);
                }
            }
            wheelIcons = new Image[slots];
            for (int i = 0; i < wheelIcons.Length; i++) // Put all icons from wheelIcons into newWheelIcons, then instantiate new icons until there is an icon for every item
            {
                Image icon = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity, transform);
                wheelIcons[i] = icon;

                float segmentSize = 360 / slots;
                float angle = (segmentSize * i) + rotationOffset;
                Vector3 iconPosition = Quaternion.Euler(0, 0, -angle) * new Vector3(0, wheelRadius, 0);
                wheelIcons[i].rectTransform.anchoredPosition = iconPosition;
            }
        }

        
        for (int i = 0; i < wheelIcons.Length; i++)
        {
            if (icons[i] != null)
            {
                wheelIcons[i].sprite = icons[i];
            }
        }
    }

    public int ReturnIndex()
    {
        return Mathf.Clamp(selectedIndex, 0, slots - 1);
    }

    public bool SelectionMade() // Returns true when the player exits the weapon wheel
    {
        if (gameObject.activeSelf == false && wasActive == true) // Check if was active last frame, if so return trues
        {
            wasActive = false;
            return true;
            
        }
        return false;
    }

    /*
    float InverseClamp(float input, float min, float max)
    {
        if (input > max)
        {
            input = min;
        }
        else if (input < min)
        {
            input = max;
        }
        return input;
    }

    int InverseClamp(int input, int min, int max)
    {
        if (input > max)
        {
            input = min;
        }
        else if (input < min)
        {
            input = max;
        }
        return input;
    }
    */
}

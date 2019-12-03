using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSelect : MonoBehaviour
{
    [Header("Inventory")]
    public int maxSlots = 16;
    public List<ItemStack> items;

    [Header("GUI")]
    public RadialMenu selector;
    public Image icon;
    public Text nameBox;
    public Text quantity;
    public Text cost;
    //public Canvas menu;
    //public RectTransform wheelBase;
    //public Image iconPrefab;
    //public float wheelRadius;
    //[Range(-180, 180)]
    //public float rotationOffset;

    //[Header("Variables")]

    //int selectedIndex;
    //Image[] wheelIcons = new Image[1];

    // Start is called before the first frame update
    void Start()
    {
        Add(new ItemStack { item = ItemData.GetItemByName("Magic Mushroom"), quantity = 69 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Magic Mushroom"), quantity = 40 });
        Add(new ItemStack { item = ItemData.GetItemByName("Jungle Beans"), quantity = 2 });
        Add(new ItemStack { item = ItemData.GetItemByName("Mana Elixir"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Stamina Booster"), quantity = 3 });

        Sprite[] icons = new Sprite[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item != null)
            {
                if (items[i].item.icon != null)
                {
                    icons[i] = items[i].item.icon;
                }
            }
        }

        selector.RefreshWheel(items.Count, icons);
    }

    // Update is called once per frame
    void Update()
    {
        //WheelHandler("RadialMenu");
        selector.WheelHandler();
        icon.sprite = items[selector.ReturnIndex()].item.icon;
        nameBox.text = items[selector.ReturnIndex()].item.name;
        quantity.text = "X " + items[selector.ReturnIndex()].quantity;
        cost.text = "$" + items[selector.ReturnIndex()].item.price;
        //print(items[selector.ReturnIndex()].item.name);

        if (selector.SelectionMade() == true)
        {
            print(items[selector.ReturnIndex()].item.name + " was selected");
        }
    }

    /*
    void WheelHandler(string buttonName)
    {
        if (Input.GetButton(buttonName))
        {
            if (Input.GetButtonDown(buttonName))
            {
                RefreshWheel();
                menu.gameObject.SetActive(true);
            }


            Vector3 relativeMousePosition = Input.mousePosition - wheelBase.position;
            float selectAngle = -Vector3.SignedAngle(wheelBase.up, relativeMousePosition, wheelBase.forward);

            //Vector3 relativeControllerPosition = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //float selectAngle = -Vector3.SignedAngle(wheelBase.up, relativeControllerPosition, wheelBase.forward);

            if (selectAngle < 0)
            {
                selectAngle += 360;
            }
            float segmentSize = 360 / items.Count;
            int index = Mathf.RoundToInt(selectAngle / segmentSize);
            if (index >= items.Count)
            {
                index = 0;
            }
            selectedIndex = index;
        }
        else if (Input.GetButtonUp(buttonName))
        {
            menu.gameObject.SetActive(false);
        }
    }

    void RefreshWheel()
    {
        if (wheelIcons.Length != items.Count)
        {
            foreach (Image i in wheelIcons)
            {
                if (i != null)
                {
                    Destroy(i.gameObject);
                }
            }
            wheelIcons = new Image[items.Count];
            for (int i = 0; i < wheelIcons.Length; i++) // Put all icons from wheelIcons into newWheelIcons, then instantiate new icons until there is an icon for every item
            {
                Image icon = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity, wheelBase);
                wheelIcons[i] = icon;
            }
        }

        float segmentSize = 360 / items.Count;
        for (int i = 0; i < wheelIcons.Length; i++)
        {
            float angle = (segmentSize * i) + rotationOffset;
            Vector3 iconPosition = Quaternion.Euler(0, 0, -angle) * new Vector3(0, wheelRadius, 0);
            wheelIcons[i].rectTransform.anchoredPosition = iconPosition;

            if (items[i].item != null)
            {
                if (items[i].item.icon != null)
                {
                    wheelIcons[i].sprite = items[i].item.icon;
                }
            }
        }
    }

    int ReturnIndex()
    {
        return selectedIndex;
    }
    */
    public void Add(ItemStack itemsObtained) // Needs a modifier to account for over-stacking an item
    {
        if (items.Count > 0) // If any non-empty inventory slots currently exist
        {
            foreach (ItemStack inventoryStack in items) // Checks existing slots for
            {
                print(inventoryStack.item + "/" + inventoryStack.quantity);

                if (inventoryStack.item == itemsObtained.item) // If a stack of the obtained item already exists but has not been filled yet
                {

                    // Adds new amount of item to existing stack
                    int spaceInStack = inventoryStack.item.maxStack - inventoryStack.quantity; // Checks how much free space is available in that slot

                    print(spaceInStack);

                    if (itemsObtained.quantity >= spaceInStack)
                    {
                        inventoryStack.quantity += spaceInStack; // Fills remaining space in inventory stack
                        // inventoryStack.quantity = inventoryStack.item.maxStack;
                        itemsObtained.quantity -= spaceInStack; // Removes amount from obtained stack
                    }
                    else
                    {
                        inventoryStack.quantity += itemsObtained.quantity; // Adds all items from obtained stack to inventory stack
                        itemsObtained.quantity -= itemsObtained.quantity; // Clears obtained inventory stack;
                        // itemsObtained.quantity = 0;
                    }
                }
            }
        }

        if (itemsObtained.quantity > 0 && items.Count < maxSlots) // If items remain after existing slots have been checked, but empty slots are present
        {
            items.Add(new ItemStack { item = itemsObtained.item, quantity = itemsObtained.quantity });
            itemsObtained.quantity = 0;
        }

    }

}

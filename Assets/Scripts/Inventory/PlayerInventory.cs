using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("GUI")]
    public Canvas inventoryScreen;
    public KeyCode toggleButton; // Replace with Input.GetButton

    [Header("Slot screen")]
    public RectTransform slotScreen;
    public Button slotButtonPrefab;
    public Text slotInfo;
    public Text sortInfo;
    public Button sortPrev;
    public Button sortNext;
    //int sortIndex;

    [Header("Inspect item")]
    public RectTransform inspectScreen;
    public Text itemName;
    public Image itemIcon;
    public Text itemCost;
    public Text[] itemBasicStats;
    public Text itemAdditionalStats;

    [Header("Item options")]
    public RectTransform optionsScreen;
    public Button equipRightHandButton;
    public Button equipLeftHandButton;
    public Button dropItemButton;

    [Header("Compare items")]
    public RectTransform compareScreen;
    public Image compareIcon;
    public Text compareName;
    public Text[] compareBasicStats;
    public Text compareAdditionalStats;

    [Header("Flavour text")]
    public RectTransform flavourTextScreen;
    public Text flavourText;

    [Header("Slots")]
    public int maxSlots = 30;
    public List<ItemStack> items;
    List<ItemStack> prevItems;

    [Header("Equip slots")]
    public EquipSlot rightHandSlot;
    public EquipSlot leftHandSlot;

    /*
    // Start is called before the first frame update
    void Start()
    {
        
        if (items.Count <= 0)
        {
            //items = ItemData.LoadInventory("Magic Mushroom,99,Necronomicon,1,Magic Mushroom,10,Necronomicon,1,Jungle Beans,2,Necronomicon,1,Mana Elixir,1,Necronomicon,1,Stamina Booster,3,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1,Necronomicon,1");
        }
        
        //inventoryScreen.enabled = false;

        //inventoryScreen.gameObject.SetActive(false);

        //sortIndex = System.Enum.GetValues(typeof(ItemType)).Length;
        //sortPrev.onClick.AddListener(() => CycleSort(-1));
        //sortNext.onClick.AddListener(() => CycleSort(1));

    }
    */
    // Update is called once per frame
    void Update()
    {
        if (prevItems != items) // Checks if items have changed since the last frame
        {
            // If true, check inventory for item anomalies and refresh inventory (refresh function only works the first time for some reason)
            print("Items updated");
            SanityCheck();
            //SortSlots(sortIndex);
            RefreshScreen(items);
            prevItems = items; // prevItems is updated to match items, so checks can continue to occur
        }
    }

    #region Item management
    void SanityCheck()
    {
        //items.RemoveRange(maxSlots, items.Count - maxSlots); // Removes item slots that exceed the max amount
        items.RemoveAll(s => s == null); // Remove null stacks
        items.RemoveAll(s => s.quantity <= 0); // Remove stacks with a quantity of zero

        foreach (ItemStack s in items)
        {
            s.quantity = Mathf.Clamp(s.quantity, 0, s.item.maxStack); // Ensures stacks do not exceed stack limit for each item
        }

        if (rightHandSlot.CurrentItem() != null) // If item is equipped
        {
            bool equippedItemExists = false; // Checks items in inventory against currently equipped item to ensure the player actually possesses it
            foreach (ItemStack i in items)
            {
                if (i.item == rightHandSlot.CurrentItem())
                {
                    equippedItemExists = true;
                }
            }

            if (equippedItemExists == false) // If false, unequip item
            {
                rightHandSlot.Unequip();
            }
        }
    }

    public void Add(ItemStack itemsObtained) // Needs a modifier to account for over-stacking an item
    {
        if (items.Count > 0) // If any non-empty inventory slots currently exist
        {
            foreach (ItemStack inventoryStack in items) // Checks existing slots for
            {
                //print(inventoryStack.item + "/" + inventoryStack.quantity);

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

    void EquipWeapon(Item equippable, bool offHandSlot)
    {
        if (offHandSlot == false) // If right hand
        {
            if (rightHandSlot.CurrentItem() != equippable) // If right hand slot does not already contain equippable, equip
            {
                if (leftHandSlot.CurrentItem() == equippable) // If left hand already contains equippable
                {
                    int amount = 0;
                    foreach (ItemStack i in items) // Check inventory to find out how many of the equippable item exists
                    {
                        if (i.item == equippable)
                        {
                            amount += 1;
                        }
                    }
                    if (amount < 2) // If there is only one of the item in the inventory
                    {
                        leftHandSlot.Unequip(); // Unequip weapon from left hand slot and transfer to right hand slot
                    }
                }

                rightHandSlot.Equip(equippable);
            }
            else
            {
                rightHandSlot.Unequip();
            }
        }
        else // If left hand
        {
            if (leftHandSlot.CurrentItem() != equippable) // If left hand slot does not already contain equippable, equip
            {
                if (rightHandSlot.CurrentItem() == equippable) // If right hand already contains equippable
                {
                    int amount = 0;
                    foreach (ItemStack i in items) // Check inventory to find out how many of the equippable item exists
                    {
                        if (i.item == equippable)
                        {
                            amount += 1;
                        }
                    }
                    if (amount < 2) // If there is only one of the item in the inventory
                    {
                        rightHandSlot.Unequip(); // Unequip weapon from left hand slot and transfer to right hand slot
                    }
                }

                leftHandSlot.Equip(equippable);
            }
            else
            {
                leftHandSlot.Unequip();
            }
        }
        
        /*
        if (rightHandSlot.CurrentItem() != equippable)
        {
            rightHandSlot.Equip(equippable);
        }
        else
        {
            rightHandSlot.Unequip();
        }
        */
    }
    #endregion

    #region GUI elements
    void RefreshScreen(List<ItemStack> itemsToDisplay)
    {
        // Hide item inspection window
        inspectScreen.gameObject.SetActive(false);
        compareScreen.gameObject.SetActive(false);
        flavourTextScreen.gameObject.SetActive(false);
        optionsScreen.gameObject.SetActive(false);

        // Destroy buttons currently existing in slot screen
        Transform[] stuffInSlotScreen = slotScreen.GetComponentsInChildren<Transform>();
        foreach (Transform t in stuffInSlotScreen)
        {
            if (t != slotScreen)
            {
                Destroy(t.gameObject);
            }
        }
        
        slotInfo.text = "SLOTS: " + items.Count + "/" + maxSlots; // Update inventory with amount of slots

        slotScreen.sizeDelta = new Vector2(slotScreen.rect.width, itemsToDisplay.Count * slotButtonPrefab.GetComponent<RectTransform>().rect.height); // Changes scroll area size to match amount of button slots

        for (int i = 0; i < itemsToDisplay.Count; i++) // Creates new buttons to inspect items
        {
            ItemStack items = itemsToDisplay[i]; // Obtains appropriate itemstack from list
            Button b = Instantiate(slotButtonPrefab, slotScreen); // Instantiates button prefab

            Text t = b.GetComponent<Text>(); // Obtains text component from button
            if (items.quantity != 1) // Names button, listing quantity if more than one of an item is available
            {
                t.text = items.item.name + ": " + items.quantity;
            }
            else
            {
                t.text = items.item.name;
            }
            
            b.onClick.AddListener(() => InspectItem(items.item)); // Add listener to button to inspect the appropriate item

            // Obtains RectTransform data and positions item at its appropriate place in the stack
            RectTransform br = b.GetComponent<RectTransform>();
            br.anchoredPosition = new Vector3(0, -i * br.rect.height, 0);
        }
    }
    /*
    public void CycleSort(int amount)
    {
        int i = sortIndex + amount;
        SortSlots(i);
    }

    public void SortSlots(int index)
    {
        index = Mathf.Clamp(index, 0, System.Enum.GetValues(typeof(ItemType)).Length);

        if (index < System.Enum.GetValues(typeof(ItemType)).Length)
        {
            List<ItemStack> itemsToShow = items;

            itemsToShow.RemoveAll(s => s.item.type != (ItemType)index);

            RefreshScreen(itemsToShow);
        }
        else
        {
            RefreshScreen(items);
        }

        sortIndex = index;
    }
    */
    public void InspectItem(Item i)
    {
        // Enables item inspection windows
        inspectScreen.gameObject.SetActive(true);
        //compareScreen.gameObject.SetActive(true);
        flavourTextScreen.gameObject.SetActive(true);
        optionsScreen.gameObject.SetActive(true);

        // Updates item information with the relative information for that item
        itemName.text = i.name;
        itemIcon.sprite = i.icon;
        itemCost.text = "$" + i.price;
        itemAdditionalStats.text = i.miscellaneousStats;
        flavourText.text = i.description;

        if (i.type == ItemType.Weapon) // If item is a weapon, enable buttons for equipping
        {
            // Enables equip buttons and resets them to add listeners to equip the new item being inspected
            equipRightHandButton.interactable = true;
            equipRightHandButton.onClick.RemoveAllListeners();
            equipRightHandButton.onClick.AddListener(() => EquipWeapon(i, false)); // Equips to right hand
            equipLeftHandButton.interactable = true;
            equipLeftHandButton.onClick.RemoveAllListeners();
            equipLeftHandButton.onClick.AddListener(() => EquipWeapon(i, true)); // Equips to left hand
        }
        else
        {
            // Disables equip buttons
            equipRightHandButton.interactable = false;
            equipLeftHandButton.interactable = false;
        }
    }
    #endregion
}

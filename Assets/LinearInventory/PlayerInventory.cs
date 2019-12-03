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
    public EquipSlot equipSlots;

    
    // Start is called before the first frame update
    void Start()
    {
        #region Add a bunch of items for testing
        Add(new ItemStack { item = ItemData.GetItemByName("Magic Mushroom"), quantity = 69 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Magic Mushroom"), quantity = 40 });
        Add(new ItemStack { item = ItemData.GetItemByName("Jungle Beans"), quantity = 2 });
        Add(new ItemStack { item = ItemData.GetItemByName("Mana Elixir"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Stamina Booster"), quantity = 3 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        #endregion

        //sortIndex = System.Enum.GetValues(typeof(ItemType)).Length;
        //sortPrev.onClick.AddListener(() => CycleSort(-1));
        //sortNext.onClick.AddListener(() => CycleSort(1));

    }

    // Update is called once per frame
    void Update()
    {
        if (prevItems != items)
        {
            SanityCheck();
            //SortSlots(sortIndex);
            RefreshScreen(items);
            prevItems = items;
        }

        if (Input.GetKeyDown(toggleButton))
        {
            inventoryScreen.enabled = !inventoryScreen.enabled;
        }

        /*
        if (inventoryScreen.enabled == true) // If inventory screen is showing
        {
            // Do inventory screen stuff
        }
        */
    }

    #region Item management
    void SanityCheck()
    {
        //items.RemoveRange(maxSlots, items.Count - maxSlots); // Removes item slots that exceed the max amount
        items.RemoveAll(s => s == null);
        items.RemoveAll(s => s.quantity <= 0);

        foreach (ItemStack s in items)
        {
            s.quantity = Mathf.Clamp(s.quantity, 0, s.item.maxStack);
        }
    }

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

    void Equip(Item equippable, EquipSlot slot)
    {
        slot.item = equippable;
        Instantiate(equippable.mesh, slot.location);
    }
    #endregion

    #region GUI elements
    void RefreshScreen(List<ItemStack> itemsToDisplay)
    {
        inspectScreen.gameObject.SetActive(false);
        compareScreen.gameObject.SetActive(false);
        flavourTextScreen.gameObject.SetActive(false);


        Transform[] stuffInSlotScreen = slotScreen.GetComponentsInChildren<Transform>();
        foreach (Transform t in stuffInSlotScreen)
        {
            if (t != slotScreen)
            {
                Destroy(t.gameObject);
            }
        }
        
        slotInfo.text = "SLOTS: " + items.Count + "/" + maxSlots;

        slotScreen.sizeDelta = new Vector2(slotScreen.rect.width, itemsToDisplay.Count * slotButtonPrefab.GetComponent<RectTransform>().rect.height);

        for (int i = 0; i < itemsToDisplay.Count; i++)
        {
            ItemStack items = itemsToDisplay[i];
            Button b = Instantiate(slotButtonPrefab, slotScreen);

            Text t = b.GetComponent<Text>();
            if (items.quantity != 1)
            {
                t.text = items.item.name + ": " + items.quantity;
            }
            else
            {
                t.text = items.item.name;
            }
            
            b.onClick.AddListener(() => InspectItem(items.item));

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
        inspectScreen.gameObject.SetActive(true);
        //compareScreen.gameObject.SetActive(true);
        flavourTextScreen.gameObject.SetActive(true);

        itemName.text = i.name;
        itemIcon.sprite = i.icon;
        itemCost.text = "$" + i.price;
        itemAdditionalStats.text = i.miscellaneousStats;
        flavourText.text = i.description;
    }
    #endregion


}

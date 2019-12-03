using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDragAndDropInventory : MonoBehaviour
{
    [Header("GUI")]
    public Canvas inventoryScreen;
    public RectTransform gridPanel;
    public Vector2 panelBorders;
    public KeyCode toggleButton; // Replace with Input.GetButton
    public RectTransform slotPrefab;
    public ItemInspector inspectPrefab;

    [Header("Slots")]
    public Vector2 slotArray = new Vector2(10, 3);
    public List<ItemStack> items;

    [Header("Equip slots")]
    public EquipSlot rightHand;
    public EquipSlot leftHand;
    public EquipSlot head;
    public EquipSlot torso;
    public EquipSlot lower;

    ItemInspector currentItemInspector;


    // Start is called before the first frame update
    void Start()
    {
        Add(new ItemStack { item = ItemData.GetItemByName("Magic Mushroom"), quantity = 69 });
        Add(new ItemStack { item = ItemData.GetItemByName("Necronomicon"), quantity = 1 });
        Add(new ItemStack { item = ItemData.GetItemByName("Magic Mushroom"), quantity = 40 });

        RefreshScreen(items);
    }

    // Update is called once per frame
    void Update()
    {
        SanityCheck();

        if (Input.GetKeyDown(toggleButton))
        {
            inventoryScreen.enabled = !inventoryScreen.enabled;
        }

        if (inventoryScreen.enabled == true) // If inventory screen is showing
        {
            // Do inventory screen stuff

            
        }
    }

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

        if (itemsObtained.quantity > 0 && items.Count < slotArray.x * slotArray.y) // If items remain after existing slots have been checked, but empty slots are present
        {
            items.Add(new ItemStack { item = itemsObtained.item, quantity = itemsObtained.quantity });
            itemsObtained.quantity = 0;
        }

    }

    #region GUI elements
    void RefreshScreen(List<ItemStack> itemsToDisplay)
    {
        //Vector2 gridWidth = new Vector2((slotPrefab.rect.width * slotArray.x) + (panelBorders.x * 2), (slotPrefab.rect.height * slotArray.y) + (panelBorders.y * 2));
        gridPanel.anchoredPosition = new Vector2(gridPanel.rect.x, gridPanel.rect.y);
        gridPanel.sizeDelta = new Vector2((slotPrefab.rect.width * slotArray.x) + (panelBorders.x * 2), (slotPrefab.rect.height * slotArray.y) + (panelBorders.y * 2));

        for (int i = 0; i < itemsToDisplay.Count;)
        {
            for (int y = 0; y < slotArray.y; y++)
            {
                for (int x = 0; x < slotArray.x; x++)
                {
                    
                    // Add item slot
                    i++;
                }
            }
        }
        
    }

    public void InspectItem(ItemStack i)
    {
        if (currentItemInspector == null)
        {
            GameObject itemWindow = Instantiate(inspectPrefab.gameObject, new Vector3(Input.mousePosition.x, Input.mousePosition.y), Quaternion.identity, inventoryScreen.transform);
            ItemInspector ii = itemWindow.GetComponent<ItemInspector>();
            currentItemInspector = ii;
            ii.nameBox.text = i.item.name;
            ii.icon.sprite = i.item.icon;
            ii.cost.text = "$" + i.item.price;
            ii.additionalStats.text = i.item.miscellaneousStats;
            ii.flavourText.text = i.item.description;
        }

        currentItemInspector.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
    }

    public void NewItemWindow(Item i)
    {
        
    }
    #endregion

    void Equip(Item equippable, EquipSlot slot)
    {
        slot.item = equippable;
        Instantiate(equippable.mesh, slot.location);
    }

}

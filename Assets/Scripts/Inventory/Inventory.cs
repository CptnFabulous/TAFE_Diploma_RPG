using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipSlot // Used to easily specify equipment slots
{
    Item item;
    GameObject itemObject;
    public Transform location;

    public void Equip(Item i)
    {
        if (itemObject != null) // If slot is not empty, empty slot and destroy prefab
        {
            Unequip();
        }
        item = i; // Specify item type
        // Specifies itemObject as new item mesh, instantiated in the appropriate position and rotation, then parents it to the equip location.
        itemObject = Object.Instantiate(item.mesh, location.position, location.rotation);
        itemObject.transform.SetParent(location);
    }

    public void Unequip() // Clears slot and destroys mesh
    {
        item = null;
        Object.Destroy(itemObject);
    }

    public Item CurrentItem() // Gets item currently equipped
    {
        return item;
    }
}

[System.Serializable]
public class ItemStack // A simple class used to easily group together items and quantities
{
    public Item item;
    public int quantity;
}

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

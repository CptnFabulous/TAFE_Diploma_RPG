using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipSlot
{
    Item item;
    GameObject itemObject;
    public Transform location;

    public void Equip(Item i)
    {
        if (itemObject != null)
        {
            Unequip();
        }
        item = i;
        itemObject = Object.Instantiate(item.mesh, location.position, location.rotation);
        itemObject.transform.SetParent(location);
    }

    public void Unequip()
    {
        item = null;
        Object.Destroy(itemObject);
    }

    public Item CurrentItem()
    {
        return item;
    }
}

[System.Serializable]
public class ItemStack
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

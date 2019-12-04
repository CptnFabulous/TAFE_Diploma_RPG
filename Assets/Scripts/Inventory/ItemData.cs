using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemData
{
    static string[] ItemIDs = new string[]
    {
        "Magic Mushroom"
    };

    public static Item GetItemByID(int id)
    {
        // Have code to check if ID is valid, give a placeholder item if invalid

        Item i = Resources.Load<Item>("Items/ItemObjects/" + ItemIDs[id]);
        if (i == null)
        {
            i = Resources.Load<Item>("Items/ItemObjects/BonusDuck");
        }
        return i;

        //return Resources.Load<Item>("Items/ItemObjects/" + ItemIDs[id]);
    }

    public static Item GetItemByName(string name)
    {
        // Have code to check if name is valid, give a placeholder item if invalid

        Item i = Resources.Load<Item>("Items/ItemObjects/" + name);
        if (i == null)
        {
            i = Resources.Load<Item>("Items/ItemObjects/BonusDuck");
        }
        return i;

        //return Resources.Load<Item>("Items/ItemObjects/" + name);
    }

    public static Item[] GetAllItems()
    {
        //return Resources.LoadAll<Item>("Items/ItemObjects/");
        return Resources.LoadAll<Item>("");
    }

    public static string SaveInventory(List<ItemStack> inventory)
    {
        string saveString = "";
        foreach(ItemStack i in inventory)
        {
            saveString += i.item.name + "," + i.quantity + ",";
        }
        saveString = saveString.Substring(0, saveString.Length - 1); // Removes last comma from save string
        return saveString;
    }

    public static List<ItemStack> LoadInventory(string saveString)
    {
        List<ItemStack> newInventory = new List<ItemStack>();
        string[] itemStrings = saveString.Split(',');
        for (int i = 0; i < itemStrings.Length; i += 2)
        {
            ItemStack newStack = new ItemStack { item = GetItemByName(itemStrings[i]), quantity = int.Parse(itemStrings[i + 1]) }; // Creates new item stack by obtaining the appropriate item
            newInventory.Add(newStack);
        }
        return newInventory;
    }
}

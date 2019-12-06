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
        Item i = Resources.Load<Item>("Items/ItemObjects/" + ItemIDs[id]); // Looks through ItemIDs list to obtain an appropriate string based on the ID, then looks in Resources for an Item scriptableObject with that name
        if (i == null)
        {
            i = Resources.Load<Item>("Items/ItemObjects/Bonus Duck"); // If null, give a Bonus Duck
        }
        return i;
    }

    public static Item GetItemByName(string name)
    {
        Item i = Resources.Load<Item>("Items/ItemObjects/" + name); // Looks in Resources for an Item scriptableObject with that name
        if (i == null)
        {
            i = Resources.Load<Item>("Items/ItemObjects/BonusDuck"); // If null, give a Bonus Duck
        }
        return i;
    }

    public static Item[] GetAllItems() // Returns a list of every item
    {
        return Resources.LoadAll<Item>("");
    }

    public static string SaveInventory(List<ItemStack> inventory)
    {
        string saveString = ""; // Generates an empty string
        foreach(ItemStack i in inventory)
        {
            saveString += i.item.name + "," + i.quantity + ","; // Adds item name and quantity, separated by commas, for each item in the inventory
        }
        saveString = saveString.Substring(0, saveString.Length - 1); // Removes last comma from save string so it can be recognised when loading
        return saveString; // Returns a string of inventory data that can be easily saved to a database
    }

    public static List<ItemStack> LoadInventory(string saveString)
    {
        List<ItemStack> newInventory = new List<ItemStack>(); // Creates an empty inventory to add items to
        string[] itemStrings = saveString.Split(','); // Splits inventory data string into individual strings, as separated by the comma. Assumes item string is in the format obtained when saving using SaveInventory()
        for (int i = 0; i < itemStrings.Length; i += 2) // Advances i by 2, as itemStrings' length is half how many stacks there are. Entries are divided into pairs alternating between item name and item quantity.
        {
            // Obtain item info based on [i], which always gives the name of an item, and [i + 1] which always gives a quantity integer
            ItemStack newStack = new ItemStack { item = GetItemByName(itemStrings[i]), quantity = int.Parse(itemStrings[i + 1]) };
            newInventory.Add(newStack); // Adds stack to inventory
        }
        return newInventory;
    }
}

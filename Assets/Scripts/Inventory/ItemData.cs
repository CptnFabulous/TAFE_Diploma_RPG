using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemData
{
    static string[] items = new string[]
    {
        "Magic Mushroom"
    };

    public static Item GetItemByID(int id)
    {
        // Have code to check if ID is valid, give a placeholder item if invalid
        return Resources.Load<Item>("Items/ItemObjects/" + items[id]);
    }

    public static Item GetItemByName(string name)
    {
        // Have code to check if name is valid, give a placeholder item if invalid
        return Resources.Load<Item>("Items/ItemObjects/" + name);
    }

    public static Item[] GetAllItems()
    {
        //return Resources.LoadAll<Item>("Items/ItemObjects/");
        return Resources.LoadAll<Item>("");
    }
}

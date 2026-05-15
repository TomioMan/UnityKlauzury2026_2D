using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Container")]
public class InventoryObject : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        items.Add(item);
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public bool HasItem(ItemData item, int amount)
    {
        int count = 0;
        foreach (var i in items)
        {
            if (i == item) count++;
        }
        return count >= amount;
    }

    public void RemoveMultipleItems(ItemData item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
            }
        }
    }
}
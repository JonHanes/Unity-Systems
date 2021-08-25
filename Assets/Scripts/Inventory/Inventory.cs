using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SystemExample.InventorySystem {
public class Inventory : MonoBehaviour
{
    const int INVENTORY_SIZE = 10; //Default is 10, any additions are on top of this

    public int Gold; //Public for debugging
    public List<InventoryItem> itemsInSlots = new List<InventoryItem>(); //Public for debugging

    public event Action inventoryUpdated;
    public void AddItem(InventoryItem item) {
        if (CanAddItem(item.itemPrefab.name)) {
            itemsInSlots.Add(item);
            inventoryUpdated();
        }
    }

    private bool CanAddItem(string itemName) {
        if (HasItem(itemName)) {
            return true; //By default allow stacking without bounds.
        }

         return INVENTORY_SIZE > itemsInSlots.Count;
    }

    public void AddGold(int setVal) {
        Gold += setVal;
    }

    public bool HasItem(string item) => itemsInSlots.Any(i => i.itemPrefab.name == item);
}

}

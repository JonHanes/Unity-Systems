using SystemExample.InventorySystem;

namespace SystemExample.Requirements {

    [System.Serializable]
    public class ItemRequirement {
         public string Name;
         public int Count;
         public bool IsRequired = true;

         public static Inventory Inventory;

         public bool Validate() => Inventory.HasItem(Name);
    }
}
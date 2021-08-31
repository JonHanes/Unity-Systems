using System;
using SystemExample.InventorySystem;
using UnityEngine;

namespace SystemExample.Conditions {

    [Serializable]
    public class InventoryCheck : Check {
        
        public static Inventory Inventory;

        public override CheckType GetCheckType() => CheckType.InventoryCheck;

        public override bool Validate() {
            return CheckIfContains();
        }

        public bool CheckIfContains() {
            if (Inventory == null) Inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
            bool hasItem = Inventory.HasItem(ItemName);

            return IsNeeded ? hasItem : !hasItem;
        }

        public string ItemName;

        public int Required;
        public bool IsNeeded;
    }
}
using SystemExample.InventorySystem;
using UnityEngine;

namespace SystemExample.Requirements {

    [System.Serializable]
    public class QuestRequirement {
         public ItemRequirement[] ItemRequirements;

         public bool Validate() {

             Inventory inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
             ItemRequirement.Inventory = inventory;

             foreach (var i in ItemRequirements) {
                 if (i.Validate()) return true;
             }
             return false;
         }
    }
}
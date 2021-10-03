using SystemExample.DialogueSystem;
using SystemExample.InventorySystem;
using SystemExample.Conditions;
using UnityEngine;
using SystemExample.States;

namespace SystemExample.Conditions {

    [System.Serializable]
    public class Condition {
        public AttributeCheck[] AttributeChecks;
        public InventoryCheck[] InventoryChecks;
        public QuestCheck[] QuestChecks;
        public RelationshipCheck[] RelationshipChecks;
        public StateCheck[] StateChecks;

        public bool ValidateAll() {
            foreach (var a in AttributeChecks) {
                if (!a.Validate()) return false;
            }

            if (StateChecks.Length > 0) {
                StateCheck.State = GameObject.FindObjectOfType<PlayerConversant>().GetAIConversant().gameObject.GetComponent<NPC>().GetState();

                foreach (var S in StateChecks)  {
                    if (!S.Validate()) return false;
                }
            }

            if (InventoryChecks.Length > 0) {
                InventoryCheck.Inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
                
                foreach (var i in InventoryChecks) {
                    if (!i.Validate()) return false;
                }
            }

            if (QuestChecks.Length > 0) {
                QuestCheck.quests = GameObject.FindObjectOfType<ProgressHandler>().GetAllQuests();

                foreach (var q in QuestChecks) {
                    if (!q.Validate()) return false;
                }
            }

            if (RelationshipChecks.Length > 0) {
                int relations = GameObject.FindObjectOfType<PlayerConversant>().GetAIConversant().GetRelations();

                foreach (var r in RelationshipChecks) {
                    if (!r.Validate()) return false;
                }
            }
            

            return true;
        }

        public bool ValidateNecessary() {
            if (QuestChecks.Length > 0) {
                QuestCheck.quests = GameObject.FindObjectOfType<ProgressHandler>().GetAllQuests();

                foreach (var q in QuestChecks) {
                    if (!q.Validate()) return false;
                }
            }

            if (RelationshipChecks.Length > 0) {
                int relations = GameObject.FindObjectOfType<PlayerConversant>().GetAIConversant().GetRelations();

                foreach (var r in RelationshipChecks) {
                    if (!r.Validate()) return false;
                }
            }
            

            return true;
        }
    }
}
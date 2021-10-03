using System.Collections;
using SystemExample.InventorySystem;
using SystemExample.States;
using System.Linq;
using UnityEngine;

namespace SystemExample.Interaction {

    [RequireComponent(typeof(BoxCollider))]
    public class Well : MonoBehaviour, IInteractable  {

        [SerializeField] string interactionName = "Use";
        [SerializeField] GameObject waterPrefab;
        bool inRange = false;
        UI_Handler uiHandler = null;
        ProgressHandler progHandler;

        private void Update() {
            if (!inRange) return;

             if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                Interact();
            } 
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {

                inRange = true;
                if (uiHandler == null)
                    uiHandler = FindObjectOfType<UI_Handler>();
                uiHandler.SetInteractionName(interactionName);
                uiHandler.ToggleMenu(true);
            }
        }

        public void Interact() {
            GetComponent<AudioSource>().Play();
            uiHandler.ToggleMenu(false);

            var player = GameObject.FindWithTag("Player");
            CheckProgressQuest();

            Inventory inv = player.GetComponent<Inventory>();
            inv.AddItem(new InventoryItem(waterPrefab, 1));
        }

        private void OnTriggerExit(Collider other) {
            if (other.tag == "Player") {
                inRange = false;
                uiHandler.ToggleMenu(false);
            }
            
        }

        void CheckProgressQuest() { //Very specific for this instance, in an actual project would need better implementation
            if (progHandler == null) { progHandler = FindObjectOfType<ProgressHandler>(); }

            //Attempt to find associated quest
            var assocQuest = progHandler.GetQuests().FirstOrDefault(q => q.GetQuestName() == "A Rude Wakeup Call");
            if (assocQuest != null && assocQuest.GetCurrentNode().GetText() == "Fetch water") {
                progHandler.AdvanceQuest(assocQuest.GetQuest());
            }
        }

    }

}

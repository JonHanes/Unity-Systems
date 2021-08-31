using System.Collections;
using SystemExample.InventorySystem;
using UnityEngine;

namespace SystemExample.Interaction {

    [RequireComponent(typeof(BoxCollider))]
    public class Well : MonoBehaviour, IInteractable  {

        [SerializeField] string interactionName = "Use";
        [SerializeField] GameObject waterPrefab;
        bool inRange = false;
        UI_Handler uiHandler = null;

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

            Inventory inv = GameObject.FindWithTag("Player").GetComponent<Inventory>();
            inv.AddItem(new InventoryItem(waterPrefab, 1));
        }

        private void OnTriggerExit(Collider other) {
            if (other.tag == "Player") {
                inRange = false;
                uiHandler.ToggleMenu(false);
            }
            
        }


    }

}

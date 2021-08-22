using SystemExample.Interaction;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class NPC : MonoBehaviour, IInteractable {
    [SerializeField] string interactionName = "Talk";
    [SerializeField] bool isLyingDown = false; //Not a good practice but I don't want to go too deep into this

    void Awake() {
        if (isLyingDown)
            GetComponent<Animator>().SetBool("isLyingDown", true);
    }

    UI_Handler uiHandler = null;
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            if (uiHandler == null)
                uiHandler = FindObjectOfType<UI_Handler>();
            uiHandler.SetInteractionName(interactionName);
            uiHandler.ToggleMenu(true);
        }
    }

    public void Interact() {
        uiHandler.ToggleMenu(false);
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            uiHandler.ToggleMenu(false);
        }
    }
}

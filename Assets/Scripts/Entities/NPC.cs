using SystemExample.DialogueSystem;
using SystemExample.Interaction;
using UnityEngine;

public enum NPC_State {
    Normal,
    Aggressive,
    Sleeping
}

[RequireComponent(typeof(BoxCollider))]
public class NPC : MonoBehaviour, IInteractable {
    [SerializeField] string interactionName = "Talk";
    [SerializeField] NPC_State state;
    bool isColliding = false;

    public NPC_State GetState() => state;
    public void SetState(NPC_State ns) {

        //Update animation
        if (ns != NPC_State.Sleeping)
            GetComponent<Animator>().SetBool("isSleeping", false);
        state = ns; 
    }

    void Awake() {
        if (state == NPC_State.Sleeping)
            GetComponent<Animator>().SetBool("isSleeping", true);
    }

    void Update() {
        if (!isColliding) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
            GetComponent<AIConversant>().StartConversation();
        }
    }

    UI_Handler uiHandler = null;
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            if (uiHandler == null)
                uiHandler = FindObjectOfType<UI_Handler>();

            isColliding = true;
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
            isColliding = false;
        }
    }
}

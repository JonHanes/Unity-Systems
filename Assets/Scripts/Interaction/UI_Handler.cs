using TMPro;
using UnityEngine;

public class UI_Handler : MonoBehaviour {
    [SerializeField] GameObject interactionMenu;

    TextMeshProUGUI interactionName;
    private void Awake() {
        interactionName = interactionMenu.GetComponentInChildren<TextMeshProUGUI>();
    }
    public void ToggleMenu(bool val) => interactionMenu.SetActive(val);

    public void SetInteractionName(string newText) => interactionName.text = newText;
}

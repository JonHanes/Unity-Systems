using SystemExample.Entities;
using UnityEngine;

public class TutorialUI : MonoBehaviour {

    [SerializeField] GameObject tutorialUIElement;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            tutorialUIElement.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            tutorialUIElement.SetActive(false);
        }
    }
}

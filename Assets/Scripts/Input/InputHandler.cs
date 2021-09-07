using System.Collections;
using SystemExample.Entities;
using SystemExample.Quests.UI;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    PlayerMovement playerMovement;
    CameraBehaviour cam;
    public bool InputDisabled { get; set; }
    private RaycastHit hit;

    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        cam = FindObjectOfType<CameraBehaviour>();
    }

    void Update()
    {
        CheckInteractions();
        CheckMovement();
        CheckRotation();
    }

    void CheckInteractions()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            GameObject.FindObjectOfType<QuestUIHandler>().ToggleDisplay();
        }
    }

    void CheckMovement() {

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) {
            movement = cam.transform.forward * -1 * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S)) {
            movement = cam.transform.forward * Time.deltaTime;
        } 
        
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) {
            StartCoroutine(FadeMovement());
        }

        if (movement == Vector3.zero) return;

        playerMovement.Move(movement);
    }

    void CheckRotation() {
        if (Input.GetKey(KeyCode.A)) {
            cam.RotateCameraRig(-1);
        } else if (Input.GetKey(KeyCode.D)) {
            cam.RotateCameraRig();
        }
    }

    IEnumerator FadeMovement() {
        while (playerMovement.GetSpeed() > 0) {
            playerMovement.SetSpeed(playerMovement.GetSpeed() - 0.05f);
            yield return new WaitForSeconds(0.05f);
        }
    }

}

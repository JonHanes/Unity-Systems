using SystemExample.Entities;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    [SerializeField] float defDistance = 10f;
    [SerializeField] float defHeight = 10f;
    [SerializeField] float rotationSpeed = 45f;
    [SerializeField] float rapidRotationSpeed = 110f;

    private Transform target;
    private Transform myCam;
    private void Start() {
        target = FindObjectOfType<PlayerMovement>().transform;
        myCam = Camera.main.transform;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + transform.forward * defDistance;

        float newY = target.position.y + defHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        myCam.LookAt(target);
    }

    public void RotateCameraRig(int val = 1) {

        float rotSpeed = rotationSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) { rotSpeed = rapidRotationSpeed; }

        Vector3 rotationVector = Vector3.up * rotSpeed * val * Time.deltaTime;
        transform.Rotate(rotationVector);
        target.transform.Rotate(rotationVector);
    }
}

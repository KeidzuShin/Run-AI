using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RigidTPSCam : MonoBehaviour
{
    public float movementSpeed = 5f;    // Player movement speed
    public Camera mainCamera;           // Reference to the main camera object

    private Rigidbody rb;               // Reference to the player's rigidbody component

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get the forward vector of the camera
        Vector3 cameraForward = mainCamera.transform.forward;

        // Ignore the y-axis rotation
        cameraForward.y = 0f;
        cameraForward = cameraForward.normalized;

        // Calculate the movement direction relative to the camera's forward direction
        Vector3 moveDir = Input.GetAxisRaw("Horizontal") * mainCamera.transform.right +
                                    Input.GetAxisRaw("Vertical") * cameraForward;

        // Normalize the movement direction to ensure consistent movement speed
        moveDir = moveDir.normalized;

        // Move the player in the calculated direction
        rb.MovePosition(transform.position + moveDir * movementSpeed * Time.fixedDeltaTime);

        // Rotate the player to face the movement direction
        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.15f);
        }
    }
}

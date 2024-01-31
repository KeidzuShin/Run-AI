using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSwingTest : MonoBehaviour
{
    public float swingForce = 10f;      // The force applied to the player when swinging
    public float maxSwingSpeed = 10f;   // The maximum speed the player can swing
    public float mouseSensitivity = 1f; // The sensitivity of mouse movement
    public Transform cameraTransform;   // The transform of the camera used for swinging
    public Transform hookTransform;     // The transform of the hook the player is swinging from

    private Rigidbody rb;               // The player's Rigidbody component
    private Vector3 swingDirection;     // The direction the player is currently swinging in
    private Vector3 mouseDirection;     // The direction the mouse is pointing in

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get the direction the mouse is pointing in
        mouseDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f).normalized * mouseSensitivity;
    }

    void FixedUpdate()
    {
        // If the player is swinging, apply a force in the swing direction
        if (swingDirection != Vector3.zero)
        {
            // Limit the player's speed while swinging
            if (rb.velocity.magnitude < maxSwingSpeed)
            {
                rb.AddForce(swingDirection * swingForce, ForceMode.Force);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // If the player is not already swinging and the "Use" button is pressed
        if (other.CompareTag("WebHook") && Input.GetButtonDown("Use") && swingDirection == Vector3.zero)
        {
            // Set the swing direction based on the player's position and the hook's position
            swingDirection = Vector3.Cross(transform.position - other.transform.position, cameraTransform.right).normalized;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // If the player is no longer touching the hook, stop swinging
        if (other.CompareTag("WebHook"))
        {
            swingDirection = Vector3.zero;
        }
    }

    void LateUpdate()
    {
        // Rotate the camera based on mouse input
        cameraTransform.RotateAround(transform.position, Vector3.up, mouseDirection.x);
        cameraTransform.RotateAround(transform.position, cameraTransform.right, -mouseDirection.y);

        // Set the player's rotation based on the swing direction
        if (swingDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(swingDirection, Vector3.up);
        }
    }
}

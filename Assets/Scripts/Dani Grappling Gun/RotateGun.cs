using UnityEngine;

public class RotateGun : MonoBehaviour {

    public GrapplingGun grappling;

    private Quaternion desiredRotation;
    private Quaternion originalRotation;
    private float rotationSpeed = 5f;
    private void Start()
    {
        originalRotation = transform.rotation;
    }
    void Update() {
        if (Input.GetMouseButtonDown(1))
            desiredRotation = Quaternion.LookRotation(grappling.GetGrapplePoint() - transform.position);           
        else
            desiredRotation = originalRotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

}

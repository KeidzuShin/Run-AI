using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform CameraOrientation;

    public float sensX = 30f; 
    public float sensY = -25f;
    public bool yes;

    float rotateX;
    float rotateY;

    private float rotatespeed;
    private Vector3 rotate;

    // Start is called before the first frame update
    void Start()
    { 
        //lock cursor on the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get mouse input. adjustable sensitivity
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        
        //how rotation in unity works (MAGIC) X and Y the other way around
        rotateY += mouseX;

        rotateX -= mouseY;
        rotateX = Mathf.Clamp(rotateX, -90f, 90f); // 90 degree MouseY limit

        //rotating the camera
        transform.rotation = Quaternion.Euler(rotateX, rotateY, 0);
        CameraOrientation.rotation = Quaternion.Euler(rotateX, rotateY, 0);

        //PLAYER LOOK FOR RIGIDBODY

    }

    public void RotateView(Vector3 AbsoluteEulerAngles)
    {
        
    }

}

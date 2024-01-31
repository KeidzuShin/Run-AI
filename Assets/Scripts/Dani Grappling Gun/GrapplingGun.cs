using UnityEngine;

public class GrapplingGun : MonoBehaviour 
{
    [Header("References")]
    public LayerMask whatIsGrappleable;
    public Transform GunTip, MainCamera, PlayerContainer;
    public GameSetting GameSetting;

    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    
    private SpringJoint joint;

    void Awake() {
        MainCamera = Camera.main.transform;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update() {
        //Wait for left mouse button
        if (Input.GetMouseButtonDown(1)) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1)) {
            StopGrapple();
        }
    }

    void LateUpdate() {        
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(MainCamera.position, MainCamera.forward, out hit, GameSetting.maxDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;
            joint = PlayerContainer.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(PlayerContainer.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = GameSetting.Spring;
            joint.damper = GameSetting.SpringDamper;
            joint.massScale = GameSetting.SpringMass;

            lineRenderer.positionCount = 2;
            currentGrapplePosition = GunTip.position;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lineRenderer.SetPosition(0, GunTip.position);
        lineRenderer.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}

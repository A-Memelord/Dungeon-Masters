using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement pm;

    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Air Movement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(swingKey))
        {
            StartSwing();
        }
        
        if (Input.GetKeyUp(swingKey))
        {
            StopSwing();
        }

        if (joint != null)
        {
            AirMovement();
        }

        CheckForSwingPoints();
    }

    private void LateUpdate()
    {
        if(pm.swinging)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void CheckForSwingPoints()
    {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward,
            out sphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward,
            out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;
        
        if(raycastHit.point != Vector3.zero)
        {
            realHitPoint = raycastHit.point;
        }

        else if (sphereCastHit.point != Vector3.zero)
        {
            realHitPoint = sphereCastHit.point;
        }

        else
        {
            realHitPoint = Vector3.zero;
        }

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }

        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    private void StartSwing()
    {

        if(predictionHit.point == Vector3.zero)
        {
            return;
        }

        if (GetComponent<Grappling>() != null)
        {
            GetComponent<Grappling>().StopGrapple();
        }

        pm.ResetRestrictions();

        pm.swinging = true;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        //customizable
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5F;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;


        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
        lr.enabled = true;
        
    }

    private Vector3 currentGrapplePosition;

    void StopSwing()
    {
        Debug.Log("pleb");
        pm.swinging = false;

        Debug.Log("3");
        lr.positionCount = 0;
        Destroy(joint);
        lr.enabled = false;
    }

    private void AirMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        }
    }
}

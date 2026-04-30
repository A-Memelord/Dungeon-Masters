using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PortalTeleport : MonoBehaviour
{
    [Header("Portal Settings")]
    public PortalTeleport linkedPortal;
    public Transform portalSurface;

    [Header("exit Settings")]
    public float exitOffset = 0.3f;
    public float exitYOffset = 0f;
    public bool preserveVelocity = false;

    [Header("Camera")]
    public float rotateCameraDuration = 0.3f;
    public bool pauseRotation = false;

    [Header("Events")]
    public UnityEvent onTeleport;
    public float onTeleportDelay = 0f;

    [Header("Look Away Event")]
    public UnityEvent onPlayerLookAway;
    public bool triggerEventOnEnter;
    [Tooltip("If true, fires onPlayerLookAway as soon as the portal is not visible - no need to look at it first.")]
    public bool immediateCheck;
    [Tooltip("Which portal to watch visibility of. if left empty, watches this portal.")]
    public PortalTeleport watchTarget;
    [Tooltip("How often (in seconds) to check if the player is looking at the portal")]
    public float lookCheckInterval = 0.1f;
    [Tooltip("Field of veiw angle threshold - portal is 'visible' if within this many degrees of screen center")]
    public float lookAwayAngleThreshold = 60f;

    private float teleportCooldown = 0f;
    private const float COOLDOWN_TIME = 0.1f;

    private MeshRenderer meshRenderer;
    private PlayerCam cachedCamScript;
    private Camera mainCam;

    private bool wasVisible = false;
    private Coroutine lookCheckCoroutine;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (Camera.main != null)
        {
            mainCam = Camera.main;
            cachedCamScript = mainCam.GetComponentInParent<PlayerCam>();
        }

        if (!triggerEventOnEnter)
            lookCheckCoroutine = StartCoroutine(LookAwayChecker());
    }

    private void Update()
    {
        if (teleportCooldown > 0f)
            teleportCooldown -= Time.deltaTime;
    }

    void OnDisable()
    {
        if (lookCheckCoroutine != null)
            StopCoroutine(lookCheckCoroutine);
    }

    void OnEnable()
    {
        if (gameObject.activeInHierarchy && !triggerEventOnEnter)
            lookCheckCoroutine = StartCoroutine(LookAwayChecker());
    }

    IEnumerator LookAwayChecker()
    {
        PortalTeleport target = watchTarget != null ? watchTarget : this;

        if (immediateCheck)
            wasVisible = true;

        while (true)
        {
            yield return new WaitForSeconds(lookCheckInterval);
            if (mainCam == null) continue;

            bool isVisible = IsPortalVisible(target);

            if (wasVisible && !isVisible)
                onPlayerLookAway?.Invoke();

            wasVisible = isVisible;
        }
    }

    bool IsPortalVisible(PortalTeleport target)
    {
        MeshRenderer targetRenderer = target.meshRenderer != null ? target.meshRenderer : target.GetComponent<MeshRenderer>();
        if (targetRenderer == null || !targetRenderer.enabled) return false;

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCam);
        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, targetRenderer.bounds))
            return false;

        Transform targetSurface = target.portalSurface != null ? target.portalSurface : target.transform;
        Vector3 toPortal = targetSurface.position - mainCam.transform.position;
        float angle = Vector3.Angle(mainCam.transform.forward, toPortal);
        return angle <= lookAwayAngleThreshold;
    }

    void OnTriggerEnter(Collider other)
    {
        if (teleportCooldown > 0f) return;
        if (!other.CompareTag("Player")) return;
        if (linkedPortal == null) return;
        if (linkedPortal.portalSurface == null) return;

        //CharacterController cc = other.GetComponent<CharacterController>();
        Rigidbody pm = other.GetComponent<Rigidbody>();

        //if (cc == null) return;

        teleportCooldown = COOLDOWN_TIME;
        linkedPortal.teleportCooldown = COOLDOWN_TIME;

        //cc.enabled = false;

        Transform exitSurface = linkedPortal.portalSurface;
        Vector3 exitNormal = exitSurface.forward;

        // --- Position ---
        Vector3 offset = other.transform.position - transform.position;

        // --- Rotation ---
        Quaternion entryToExit = exitSurface.rotation * Quaternion.Inverse(portalSurface.rotation);
        Quaternion yawOnly = Quaternion.Euler(0f, other.transform.eulerAngles.y, 0f);
        Quaternion exitRot = entryToExit * yawOnly * Quaternion.Euler(0f, exitYOffset, 0f);

        other.transform.SetPositionAndRotation(linkedPortal.transform.position + offset, Quaternion.Euler(0f, exitRot.eulerAngles.y, 0f));

        // --- Velocity ---
        if (pm != null)
        {
            if (preserveVelocity)
            {
                float speed = pm.linearVelocity.magnitude;
                pm.linearVelocity = Vector3.down * speed;
            }
        }
    }
}

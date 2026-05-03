using UnityEngine;

public class PlayerFirePoint : MonoBehaviour
{
    [Tooltip("Optional explicit fire point transform. If empty a child named 'FirePoint' will be used or created.")]
    public Transform firePoint;

    [Tooltip("Optional camera to follow. If empty Camera.main or a child Camera will be used.")]
    public Camera playerCamera;

    [Tooltip("If true the firePoint will copy the camera's rotation and be positioned slightly in front of the camera.")]
    public bool matchCameraRotation = true;

    [Tooltip("Distance in front of the camera to place the fire point when matching camera.")]
    public float cameraForwardOffset = 0.5f;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main ?? GetComponentInChildren<Camera>();

        if (firePoint == null)
        {
            var found = transform.Find("FirePoint");
            if (found != null)
            {
                firePoint = found;
            }
            else
            {
                // create a simple firepoint so users don't have to manually add one
                var go = new GameObject("FirePoint");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.up * 1.0f;
                go.transform.localRotation = Quaternion.identity;
                firePoint = go.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (matchCameraRotation && playerCamera != null && firePoint != null)
        {
            // copy full camera rotation to allow vertical aiming
            firePoint.rotation = playerCamera.transform.rotation;

            // keep firePoint positioned slightly in front of the camera (prevents spawning inside head)
            firePoint.position = playerCamera.transform.position + playerCamera.transform.forward * cameraForwardOffset;
        }
    }

    public Transform GetFirePoint()
    {
        return firePoint;
    }
}
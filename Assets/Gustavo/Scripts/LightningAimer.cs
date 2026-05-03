using System;
using UnityEngine;

// Responsible for showing a world-space indicator where the player is aiming.
// Call BeginAim(...) to start an aiming session. Will invoke onConfirm when player presses Fire1 / left-click.
public class LightningAimer : MonoBehaviour
{
    private Camera _cam;
    private GameObject _indicatorInstance;
    private GameObject _indicatorPrefab;
    private bool _isAiming;
    private float _maxDistance = 30f;
    private Action<Vector3, Vector3> _onConfirm;
    private GameObject _user; // used to compute spawn position (firepoint or camera)

    public void BeginAim(GameObject user, GameObject indicatorPrefab, float maxDistance, Action<Vector3, Vector3> onConfirm)
    {
        _user = user;
        _indicatorPrefab = indicatorPrefab;
        _maxDistance = maxDistance;
        _onConfirm = onConfirm;
        _isAiming = true;

        _cam = Camera.main ?? GetComponentInChildren<Camera>() ?? GetComponent<Camera>();
        if (_indicatorPrefab != null)
        {
            _indicatorInstance = Instantiate(_indicatorPrefab);
        }
        else
        {
            // Create a simple fallback indicator if none was provided
            _indicatorInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _indicatorInstance.transform.localScale = Vector3.one * 0.25f;
            var col = _indicatorInstance.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }
    }

    void Update()
    {
        if (!_isAiming) return;

        if (_cam == null)
            _cam = Camera.main ?? GetComponentInChildren<Camera>() ?? GetComponent<Camera>();

        if (_cam == null) return;

        // Raycast from camera forward to place the indicator
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        RaycastHit hit;
        Vector3 aimPoint;
        if (Physics.Raycast(ray, out hit, _maxDistance))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = _cam.transform.position + _cam.transform.forward * _maxDistance;
        }

        if (_indicatorInstance != null)
        {
            _indicatorInstance.transform.position = aimPoint;
            _indicatorInstance.transform.rotation = Quaternion.LookRotation(_cam.transform.up, Vector3.up);
        }

        // Confirm input: left mouse or Fire1 (works with keyboard/controller)
        if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
        {
            // Determine spawn origin: prefer PlayerFirePoint on user, else camera position
            Vector3 aimOrigin;
            Transform firePointTransform = null;
            if (_user != null)
            {
                var pfp = _user.GetComponent<PlayerFirePoint>();
                if (pfp != null) firePointTransform = pfp.GetFirePoint();
                if (firePointTransform == null)
                {
                    var found = _user.transform.Find("FirePoint");
                    if (found != null) firePointTransform = found;
                }
            }

            if (firePointTransform != null)
            {
                aimOrigin = firePointTransform.position;
            }
            else
            {
                aimOrigin = _cam.transform.position;
            }

            Vector3 aimDirection = (aimPoint - aimOrigin).normalized;

            _onConfirm?.Invoke(aimOrigin, aimDirection);
            EndAim();
        }

        // Cancel input: Escape / right mouse
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            EndAim();
        }
    }

    private void EndAim()
    {
        _isAiming = false;
        _onConfirm = null;
        if (_indicatorInstance != null)
        {
            Destroy(_indicatorInstance);
            _indicatorInstance = null;
        }
    }

    private void OnDisable()
    {
        EndAim();
    }

    private void OnDestroy()
    {
        EndAim();
    }
}
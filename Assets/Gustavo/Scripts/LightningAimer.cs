
using UnityEngine;

// Handles showing a ground indicator while the player aims and spawning the lightning strike on release.
// Use: created at runtime by LightningCard.Use(...)
public class LightningAimer : MonoBehaviour
{
    [HideInInspector] public GameObject IndicatorPrefab;
    [HideInInspector] public GameObject LightningPrefab;
    [HideInInspector] public GameObject Owner;
    [HideInInspector] public float MaxRange = 20f;
    [HideInInspector] public LayerMask GroundLayer = ~0;
    [HideInInspector] public float SpawnHeightOffset = 0.1f;

    // strike properties
    [HideInInspector] public float StrikeRadius = 3f;
    [HideInInspector] public float Damage = 40f;
    [HideInInspector] public StatusEffectData StatusEffect;

    private GameObject _indicatorInstance;
    private Camera _cam;
    private bool _isAiming;

    void Start()
    {
        _cam = Camera.main ?? Owner?.GetComponentInChildren<Camera>();
        if (IndicatorPrefab != null)
        {
            _indicatorInstance = Instantiate(IndicatorPrefab);
            _indicatorInstance.SetActive(false);
        }

        // start aiming immediately
        _isAiming = true;

        // optionally lock cursor or UI here if desired
    }

    void Update()
    {
        if (!_isAiming) return;

        // update indicator position
        Vector3 aimPoint;
        bool valid = ComputeAimPoint(out aimPoint);

        if (_indicatorInstance != null)
        {
            _indicatorInstance.SetActive(valid);
            if (valid)
            {
                _indicatorInstance.transform.position = aimPoint + Vector3.up * SpawnHeightOffset;
                // ensure indicator faces up
                _indicatorInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
        }

        // If user releases primary fire (left mouse button), confirm strike.
        // This uses the legacy Input system for simplicity. If you use the new Input System, replace with your action checks.
        if (Input.GetMouseButtonUp(0))
        {
            if (valid)
                SpawnStrikeAt(aimPoint);
            EndAiming();
        }

        // cancel on right click or Escape
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            EndAiming();
        }
    }

    private bool ComputeAimPoint(out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;

        // Prefer using the mouse position projected to world (great for mouse/keyboard).
        if (_cam != null)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, MaxRange, GroundLayer, QueryTriggerInteraction.Ignore))
            {
                worldPoint = hit.point;
                return true;
            }

            // If nothing hit, project a point forward at max range and try a downward ray to ground
            Vector3 forwardPoint = ray.origin + ray.direction.normalized * MaxRange;
            if (Physics.Raycast(forwardPoint + Vector3.up * 10f, Vector3.down, out hit, 50f, GroundLayer, QueryTriggerInteraction.Ignore))
            {
                worldPoint = hit.point;
                return true;
            }
        }

        // Fallback: use owner's forward if camera/mouse unavailable
        if (Owner != null)
        {
            Vector3 origin = Owner.transform.position + Vector3.up * 1f;
            if (Physics.Raycast(origin, Owner.transform.forward, out RaycastHit hit2, MaxRange, GroundLayer, QueryTriggerInteraction.Ignore))
            {
                worldPoint = hit2.point;
                return true;
            }
        }

        return false;
    }

    private void SpawnStrikeAt(Vector3 pos)
    {
        if (LightningPrefab == null) return;

        var strike = Instantiate(LightningPrefab, pos + Vector3.up * SpawnHeightOffset, Quaternion.identity);
        // configure strike (expects LightningProjectile-like behaviour)
        var lp = strike.GetComponent<LightningProjectile>();
        if (lp != null)
        {
            lp.Initialize(Damage, StrikeRadius, StatusEffect, Owner);
        }
        else
        {
            // try generic: attach LightningProjectile dynamically if prefab missing it
            var dyn = strike.AddComponent<LightningProjectile>();
            dyn.Initialize(Damage, StrikeRadius, StatusEffect, Owner);
        }
    }

    private void EndAiming()
    {
        if (_indicatorInstance != null) Destroy(_indicatorInstance);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_indicatorInstance != null) Destroy(_indicatorInstance);
    }
}
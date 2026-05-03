using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))
]
public class MeleeCombat : MonoBehaviour
{
  
    public float attackDistance = 3f;
    public int attackDamage = 30;
  
    public float attackCooldown = 0.5f;

   
    public int rayCount = 5;
    
    public float spreadAngle = 20f;
   
    public LayerMask enemyLayer;

    
    public GameObject[] slashPrefabs;

    [Header("Last Stand (alternate)")]
    public GameObject[] lastStandSlashPrefabs;
    public int lastStandDamage = 60;
    
    public float vfxDistance = 1.5f;
    
    public float vfxLifetime = 1.0f;

    [Header("Debug")]
    public bool drawDebugRays = true;
    public Color debugColor = Color.red;

    Camera cam;
    PlayerInput playerInput;
    Animator animator;
    bool canAttack = true;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main;
    }

    void Update()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            var action = playerInput.actions["Attack"];
            if (action != null && action.triggered)
                TriggerAttack();
        }
    }

    
    public void TriggerAttack()
    {
        if (!canAttack) return;
        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        if (animator != null)
            animator.SetTrigger("Attack");

 
        bool vfxSpawned = false;

        
        Vector3 origin = cam != null ? cam.transform.position + cam.transform.forward * 0.5f : transform.position;
        Vector3 forward = cam != null ? cam.transform.forward : transform.forward;

        
        int layerMask = (enemyLayer.value == 0) ? ~0 : enemyLayer.value;

        // collect processed colliders to avoid double-damage
        var damaged = new HashSet<Collider>();

        if (rayCount <= 1)
        {
            if (Physics.Raycast(origin, forward, out RaycastHit hit, attackDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (ProcessHit(hit, damaged))
                    vfxSpawned = true;
            }
            if (drawDebugRays) Debug.DrawRay(origin, forward * attackDistance, debugColor, 0.5f);
        }
        else
        {
            for (int i = 0; i < rayCount; i++)
            {
                float t = (rayCount == 1) ? 0.5f : (float)i / (rayCount - 1);
                float angle = Mathf.Lerp(-spreadAngle * 0.5f, spreadAngle * 0.5f, t);
                Vector3 dir = Quaternion.AngleAxis(angle, cam.transform.up) * forward;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, attackDistance, layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (ProcessHit(hit, damaged))
                        vfxSpawned = true;
                }

                if (drawDebugRays) Debug.DrawRay(origin, dir * attackDistance, debugColor, 0.5f);
            }
        }

        
        if (!vfxSpawned)
            SpawnVFXCentered();

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

  
    private bool ProcessHit(RaycastHit hit, HashSet<Collider> damaged)
    {
        if (hit.collider == null) return false;
        if (damaged.Contains(hit.collider)) return false;
        damaged.Add(hit.collider);

        int damage = GetCurrentAttackDamage();
        
        var enemy = hit.collider.GetComponentInParent<Health>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"Melee hit Enemy (-{damage}). Remaining health: {enemy.health}");
            SpawnVFXAtPoint(hit.point, hit.normal);
            return true;
        }

        
        var health = hit.collider.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"Melee hit Health (-{damage}).");
            SpawnVFXAtPoint(hit.point, hit.normal);
            return true;
        }

       
        SpawnVFXAtPoint(hit.point, hit.normal);
        return true;
    }

    private GameObject[] GetActiveSlashPrefabs()
    {
        bool lastStand = IsLastStandActive();
        if (lastStand && lastStandSlashPrefabs != null && lastStandSlashPrefabs.Length > 0)
            return lastStandSlashPrefabs;
        return slashPrefabs;
    }

    private int GetCurrentAttackDamage()
    {
        return IsLastStandActive() ? lastStandDamage : attackDamage;
    }

    // Updated: checks whether a UiCard instance for LastStand is present AND active,
    // instead of relying on the LastStandAdded flag.
    private bool IsLastStandActive()
    {
        var cards = FindFirstObjectByType<Cards>();
        if (cards == null || cards.LastStand == null) return false;

        UiCard[] uiCards = Object.FindObjectsByType<UiCard>(FindObjectsSortMode.None);
        foreach (var uiCard in uiCards)
        {
            if (uiCard.card == cards.LastStand && uiCard.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }

    private void SpawnVFXCentered()
    {
        var prefabs = GetActiveSlashPrefabs();
        if (prefabs == null || prefabs.Length == 0 || cam == null) return;
        var prefab = prefabs[Random.Range(0, prefabs.Length)];
        Vector3 pos = cam.transform.position + cam.transform.forward * vfxDistance;
        Quaternion rot = Quaternion.LookRotation(cam.transform.forward, Vector3.up);
        var go = Instantiate(prefab, pos, rot);
        Destroy(go, vfxLifetime);
    }

    private void SpawnVFXAtPoint(Vector3 point, Vector3 normal)
    {
        var prefabs = GetActiveSlashPrefabs();
        if (prefabs == null || prefabs.Length == 0) return;
        var prefab = prefabs[Random.Range(0, prefabs.Length)];
       
        var go = Instantiate(prefab, point + normal * 0.02f, Quaternion.LookRotation(normal));
        Destroy(go, vfxLifetime);
    }
}

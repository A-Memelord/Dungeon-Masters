using UnityEngine;

[CreateAssetMenu(fileName = "LightningCard", menuName = "Scriptable Objects/Cards/Lightning")]
public class LightningCard : Card
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 18f;
    public float projectileLifetime = 4f;
    public float damage = 20f;
    public float stunDuration = 2f;

    [Header("Aiming / Indicator")]
    public GameObject indicatorPrefab;
    public float aimMaxDistance = 50f;

    [Header("Fire Point")]
    public string firePointName = "FirePoint"; // fallback name

    public override void Use(GameObject user)
    {
        base.Use(user);

        if (projectilePrefab == null || user == null) return;

        // Get firepoint (same logic as FireballCard)
        Transform firePointTransform = null;
        var pfp = user.GetComponent<PlayerFirePoint>();
        if (pfp != null)
        {
            firePointTransform = pfp.GetFirePoint();
        }

        if (firePointTransform == null)
        {
            var found = user.transform.Find(firePointName);
            if (found != null) firePointTransform = found;
        }

        // Ensure camera reference (aimer will fall back to Camera.main)
        Camera cam = Camera.main ?? user.GetComponentInChildren<Camera>();

        // Ensure an aimer exists on the player (or attach to camera if available)
        LightningAimer aimer = null;
        if (cam != null)
        {
            aimer = cam.GetComponent<LightningAimer>();
            if (aimer == null)
                aimer = cam.gameObject.AddComponent<LightningAimer>();
        }
        else
        {
            aimer = user.GetComponentInChildren<LightningAimer>();
            if (aimer == null)
                aimer = user.AddComponent<LightningAimer>();
        }

        // Begin aim: the aimer will show indicator and call back when player confirms.
        aimer.BeginAim(user, indicatorPrefab, aimMaxDistance, (aimOrigin, aimDirection) =>
        {
            // Spawn projectile slightly forward of the aim origin to avoid hitting the user
            Vector3 spawnPos = aimOrigin + aimDirection.normalized * 0.6f;
            Quaternion rot = Quaternion.LookRotation(aimDirection.normalized, Vector3.up);

            var projObj = Instantiate(projectilePrefab, spawnPos, rot);
            projObj.SetActive(false);

            var proj = projObj.GetComponent<LightningProjectile>();
            if (proj != null)
            {
                proj.Initialize(damage, projectileSpeed, projectileLifetime, stunDuration, user);
            }
            else
            {
                // fallback: ensure forward assigned
                projObj.transform.forward = aimDirection.normalized;
            }

            projObj.SetActive(true);
        });
    }
}
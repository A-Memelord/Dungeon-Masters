using UnityEngine;

[CreateAssetMenu(fileName = "FireballCard", menuName = "Scriptable Objects/Cards/Fireball")]
public class FireballCard : Card
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float projectileLifetime = 5f;
    public float damage = 25f;

    [Header("Status Effect")]
    public StatusEffectData burnEffect;

    [Header("Fire Point")]
    public string firePointName = "FirePoint"; // fallback name

    public override void Use(GameObject user)
    {
        base.Use(user);

        if (projectilePrefab == null || user == null) return;

        // Try PlayerFirePoint component first
        Transform firePointTransform = null;
        var pfp = user.GetComponent<PlayerFirePoint>();
        if (pfp != null)
        {
            firePointTransform = pfp.GetFirePoint();
        }

        // If still null, try to locate by name on the player
        if (firePointTransform == null)
        {
            var found = user.transform.Find(firePointName);
            if (found != null) firePointTransform = found;
        }

        // If still null, use camera, else user forward
        Camera cam = Camera.main ?? user.GetComponentInChildren<Camera>();
        Vector3 aimOrigin;
        Vector3 aimForward;

        if (firePointTransform != null)
        {
            aimOrigin = firePointTransform.position;
            aimForward = firePointTransform.forward;
        }
        else if (cam != null)
        {
            aimOrigin = cam.transform.position;
            aimForward = cam.transform.forward;
        }
        else
        {
            aimOrigin = user.transform.position + Vector3.up * 1.0f;
            aimForward = user.transform.forward;
        }

        // spawn a little forward of the aim origin to avoid colliding with the player
        Vector3 spawnPos = aimOrigin + aimForward.normalized * 0.6f;
        Quaternion rot = Quaternion.LookRotation(aimForward.normalized, Vector3.up);

        // instantiate inactive to allow Initialize to call Physics.IgnoreCollision before any trigger events
        var projObj = Instantiate(projectilePrefab, spawnPos, rot);
        projObj.SetActive(false);

        var proj = projObj.GetComponent<FireballProjectile>();
        if (proj != null)
        {
            proj.Initialize(damage, projectileSpeed, projectileLifetime, burnEffect, user);
        }
        else
        {
            // fallback: ensure forward assigned
            projObj.transform.forward = aimForward.normalized;
        }

        // now activate projectile so its Start/Awake run after collisions are safely ignored
        projObj.SetActive(true);
    }
}

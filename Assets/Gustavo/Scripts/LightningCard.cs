using UnityEngine;


[CreateAssetMenu(fileName = "LightningCard", menuName = "Scriptable Objects/Cards/Lightning")]
public class LightningCard : Card
{
    [Header("Aiming")]
    public GameObject indicatorPrefab;     // ground circle prefab (set in inspector)
    public float maxRange = 20f;           // max distance from player to aim

    [Header("Strike")]
    public GameObject lightningPrefab;     // strike prefab (should have LightningProjectile component)
    public float strikeRadius = 3f;
    public float damage = 40f;
    public StatusEffectData statusEffect;

    [Header("Layer & offset")]
    public LayerMask groundLayer = ~0;     // layers to consider for ground placement (default: everything)
    public float spawnHeightOffset = 0.1f; // slight lift so particle effects don't clip the ground

    public override void Use(GameObject user)
    {
        base.Use(user);
        if (lightningPrefab == null || user == null)
            return;

        // prevent multiple aimers
        var existing = Object.FindFirstObjectByType<LightningAimer>();
        if (existing != null)
        {
            Object.Destroy(existing.gameObject);
        }

        // create aimer object at runtime and pass required data
        var go = new GameObject("LightningAimer");
        var aimer = go.AddComponent<LightningAimer>();
        aimer.Owner = user;
        aimer.IndicatorPrefab = indicatorPrefab;
        aimer.LightningPrefab = lightningPrefab;
        aimer.MaxRange = maxRange;
        aimer.GroundLayer = groundLayer;
        aimer.SpawnHeightOffset = spawnHeightOffset;

        // strike parameters
        aimer.StrikeRadius = strikeRadius;
        aimer.Damage = damage;
        aimer.StatusEffect = statusEffect;
    }
}

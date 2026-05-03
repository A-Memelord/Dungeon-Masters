using UnityEngine;

// Lightning strike behaviour: apply instant area damage and optional status effect, then spawn particles and self-destruct.
[RequireComponent(typeof(Collider))]
public class LightningProjectile : MonoBehaviour
{
    private float _damage = 40f;
    private float _radius = 3f;
    private StatusEffectData _statusEffect;
    private GameObject _owner;
    private float _lifeTime = 1f;
    private float _spawnTime;

    // Initialize called by LightningAimer after instantiation
    public void Initialize(float damage, float radius, StatusEffectData statusEffect, GameObject owner = null)
    {
        _damage = damage;
        _radius = radius;
        _statusEffect = statusEffect;
        _owner = owner;
        _spawnTime = Time.time;

        // run the strike immediately
        Strike();

        // optionally destroy after a short time so particles can play
        Destroy(gameObject, _lifeTime);
    }

    private void Strike()
    {
        // spawn effect particles if provided by statusEffect
        if (_statusEffect != null && _statusEffect.Effectparticles != null)
        {
            var p = Instantiate(_statusEffect.Effectparticles, transform.position, Quaternion.identity);
            // parent to the strike so destruction cleans it up if desired
            p.transform.SetParent(transform);
        }

        // overlap sphere to find targets
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        foreach (var c in hits)
        {
            if (_owner != null)
            {
                if (c.gameObject == _owner) continue;
                if (c.transform.IsChildOf(_owner.transform)) continue;
            }

            if (c.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(_damage);
                if (_statusEffect != null) health.ApplyEffect(_statusEffect);
            }
            else if (c.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.health -= _damage;
                if (_statusEffect != null) enemy.ApplyEffect(_statusEffect);
            }
        }
    }

    // optional: visualize radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class LightningProjectile : MonoBehaviour
{
    private float _speed = 10f;
    private float _damage = 10f;
    private float _lifetime = 5f;
    private StatusEffectData _statusEffect;
    private GameObject _owner;
    private float _spawnTime;
    private Collider _coll;
    private Rigidbody _rb;
    private Collider[] _ownerColliders;

    // piercing state
    private int _pierceRemaining = 3;
    private HashSet<int> _hitIds = new HashSet<int>();

    private void Awake()
    {
        _coll = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        // projectile should use physics for reliable trigger callbacks
        if (_rb != null)
        {
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (_coll != null) _coll.isTrigger = true;
    }

    // Added optional pierceCount (defaults to 3)
    public void Initialize(float damage, float speed, float lifetime, StatusEffectData statusEffect, GameObject owner = null, int pierceCount = 3)
    {
        _damage = damage;
        _speed = speed;
        _lifetime = lifetime;
        _statusEffect = statusEffect;
        _owner = owner;
        _spawnTime = Time.time;

        // initialize pierce state
        _pierceRemaining = Mathf.Max(0, pierceCount);
        _hitIds.Clear();

        // ensure components exist (Initialize may be called before Awake if prefab instantiated inactive)
        if (_coll == null) _coll = GetComponent<Collider>();
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_coll != null) _coll.isTrigger = true;
        if (_rb != null)
        {
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // Ignore collisions with the owner so the projectile won't hit the player
        if (_owner != null && _coll != null)
        {
            _ownerColliders = _owner.GetComponentsInChildren<Collider>();
            foreach (var oc in _ownerColliders)
            {
                if (oc != null)
                    Physics.IgnoreCollision(_coll, oc, true);
            }
        }

        // set initial velocity using Rigidbody (reliable for triggers)
        if (_rb != null)
        {
            _rb.linearVelocity = transform.forward * _speed;
        }
    }

    private void Update()
    {
        if (Time.time - _spawnTime >= _lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ignore owner and owner's children
        if (_owner != null)
        {
            if (other.gameObject == _owner) return;
            if (other.transform.IsChildOf(_owner.transform)) return;
        }

        // Try to find a damageable component on the collider or its parents.
        var health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            int id = health.gameObject.GetInstanceID();
            if (_hitIds.Contains(id)) return;

            health.TakeDamage(_damage);
            if (_statusEffect != null) health.ApplyEffect(_statusEffect);

            _hitIds.Add(id);
            _pierceRemaining--;

            if (_pierceRemaining <= 0)
            {
                Destroy(gameObject);
            }

            // pierced through this damageable target — keep going if pierce remaining > 0
            return;
        }

        var enemy = other.GetComponentInParent<Health>();
        if (enemy != null)
        {
            int id = enemy.gameObject.GetInstanceID();
            if (_hitIds.Contains(id)) return;

            enemy.TakeDamage(_damage);
            if (_statusEffect != null) enemy.ApplyEffect(_statusEffect);

            _hitIds.Add(id);
            _pierceRemaining--;

            if (_pierceRemaining <= 0)
            {
                Destroy(gameObject);
            }

            return;
        }

        // Any hit that is not a damageable target (walls, scenery) destroys the projectile immediately
        Destroy(gameObject);
    }
}
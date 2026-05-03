using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class LightningProjectile : MonoBehaviour
{
    private float _speed = 15f;
    private float _damage = 15f;
    private float _lifetime = 4f;
    private float _stunDuration = 1.5f;
    private GameObject _owner;
    private float _spawnTime;
    private Collider _coll;
    private Rigidbody _rb;
    private Collider[] _ownerColliders;

    // piercing state (can be adjusted if desired)
    private int _pierceRemaining = 1;
    private HashSet<int> _hitIds = new HashSet<int>();

    private void Awake()
    {
        _coll = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        if (_rb != null)
        {
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (_coll != null) _coll.isTrigger = true;
    }

    // Initialize with stunDuration added
    public void Initialize(float damage, float speed, float lifetime, float stunDuration, GameObject owner = null, int pierceCount = 1)
    {
        _damage = damage;
        _speed = speed;
        _lifetime = lifetime;
        _stunDuration = stunDuration;
        _owner = owner;
        _spawnTime = Time.time;

        _pierceRemaining = Mathf.Max(0, pierceCount);
        _hitIds.Clear();

        if (_coll == null) _coll = GetComponent<Collider>();
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_coll != null) _coll.isTrigger = true;
        if (_rb != null)
        {
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (_owner != null && _coll != null)
        {
            _ownerColliders = _owner.GetComponentsInChildren<Collider>();
            foreach (var oc in _ownerColliders)
            {
                if (oc != null)
                    Physics.IgnoreCollision(_coll, oc, true);
            }
        }

        if (_rb != null)
        {
            // keep the style consistent with existing projectile code in the project
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

            _hitIds.Add(id);
            _pierceRemaining--;

            if (_pierceRemaining <= 0)
            {
                Destroy(gameObject);
            }

            return;
        }

        //var enemy = other.GetComponentInParent<Enemy>();
        //if (enemy != null)
        //{
        //    int id = enemy.gameObject.GetInstanceID();
        //    if (_hitIds.Contains(id)) return;

        //    enemy.health -= _damage;

        //    // apply stun if available
        //    enemy.Stun(_stunDuration);

        //    _hitIds.Add(id);
        //    _pierceRemaining--;

        //    if (_pierceRemaining <= 0)
        //    {
        //        Destroy(gameObject);
        //    }

        //    return;
        //}

        // Any hit that is not a damageable target (walls, scenery) destroys the projectile immediately
        Destroy(gameObject);
    }
}
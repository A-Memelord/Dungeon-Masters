using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class WindProjectile : MonoBehaviour
{
    private float _speed = 12f;
    private float _damage = 0f;
    private float _lifetime = 5f;
    private float _pushForce = 6f;
    private float _stunDuration = 1.5f;
    private GameObject _owner;
    private float _spawnTime;
    private Collider _coll;
    private Rigidbody _rb;
    private Collider[] _ownerColliders;

    // The wall should affect many enemies while avoiding multiple hits on the same target
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

    // Initialize the wind wall
    public void Initialize(float damage, float speed, float lifetime, float pushForce, float stunDuration, GameObject owner = null)
    {
        _damage = damage;
        _speed = speed;
        _lifetime = lifetime;
        _pushForce = pushForce;
        _stunDuration = stunDuration;
        _owner = owner;
        _spawnTime = Time.time;

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

        // drive the wall forward
        if (_rb != null)
        {
            _rb.linearVelocity = transform.forward * _speed;
        }
    }

    private void Update()
    {
        if (Time.time - _spawnTime >= _lifetime)
            Destroy(gameObject);

        // if Rigidbody missing fallback to transform movement
        if (_rb == null)
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (_owner != null)
        {
            if (other.gameObject == _owner) return;
            if (other.transform.IsChildOf(_owner.transform)) return;
        }

        // Avoid processing the same entity multiple times
        var root = other.transform.root;
        if (root == null) root = other.transform;
        int id = root.gameObject.GetInstanceID();
        if (_hitIds.Contains(id)) return;

        // push and stun Enemy if present
        var enemy = other.GetComponentInParent<Health>();
        if (enemy != null)
        {
            // apply damage via Health if present
            var health = enemy.GetComponentInParent<Health>();
            if (health != null && _damage > 0f)
            {
                health.TakeDamage(_damage);
            }
            else if (_damage > 0f)
            {
                // fallback to modifying Enemy.health directly
                
            }

            // apply stun if the enemy supports it
            enemy.Stun(_stunDuration);

            // apply push force if the enemy has a Rigidbody
            Rigidbody enemyRb = enemy.GetComponentInParent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(transform.forward * _pushForce, ForceMode.Impulse);
            }
            else
            {
                // fallback: nudge position a little (best-effort)
                enemy.transform.position += transform.forward * (_pushForce * 0.02f);
            }

            _hitIds.Add(id);
            // Do not destroy the wall � it should pass through and affect multiple targets
            return;
        }

       
    }
}

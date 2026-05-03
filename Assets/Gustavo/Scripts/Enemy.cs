using UnityEngine;

public class Enemy : MonoBehaviour, IEffectable
{
    
    private StatusEffectData _data;

    // stun state
    private float _stunEndTime = 0f;
    public bool IsStunned => Time.time < _stunEndTime;

    public void ApplyEffect(StatusEffectData _data)
    {
        this._data = _data;
    }

    public void RemoveEffect()
    {
        _data = null;
    }

    void Start()
    {
        //health = maxHealth;
    }

    private void Update()
    {
        // Effects are not processed while stunned
        if (_data != null && !IsStunned) HandleEffect();

        //if (health <= 0)
        //{
        //    gameObject.SetActive(false);
        //    health = maxHealth;
        //    var tracker = FindFirstObjectByType<KillCountTracker>();
        //    if (tracker != null)
        //        tracker.IncrementKillCount();
        //}
    }

    private float _currentEffectTime = 0f;
    private float _nextTickTime = 0f;
    public void HandleEffect()
    {
        _currentEffectTime += Time.deltaTime;

        if (_currentEffectTime >= _data.Lifetime) RemoveEffect();

        if (_data.DOTAmount != 0 && _currentEffectTime > _nextTickTime)
        {
            _nextTickTime = _data.TickSpeed;
            
        }
    }

    // Public API to stun this enemy for a duration (seconds).
    public void Stun(float duration)
    {
        if (duration <= 0f) return;
        _stunEndTime = Time.time + duration;
    }
}





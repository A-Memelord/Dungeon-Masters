using UnityEngine;

public class Enemy : MonoBehaviour, IEffectable
{
    public float health;
    public float maxHealth;
    private StatusEffectData _data;
    
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
        health = maxHealth;
    }

    private void Update()
    {
        if (_data != null) HandleEffect();
        
        if (health <= 0)
        {
            gameObject.SetActive(false);
            health = maxHealth;
            var tracker = FindFirstObjectByType<KillCountTracker>();
            if (tracker != null)
                tracker.IncrementKillCount();

        }
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
            health -= _data.DOTAmount;
        }
    }
    
}





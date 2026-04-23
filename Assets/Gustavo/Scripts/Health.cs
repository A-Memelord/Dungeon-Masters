using UnityEngine;

public class Health : MonoBehaviour, IEffectable
{
    [SerializeField] private float currentHealth;
    private float maxHealth;

    private StatusEffectData _data;

    
    public float health => currentHealth;

    public void ApplyEffect(StatusEffectData _data)
    {
        RemoveEffect();
        this._data = _data;
        //_effectParticles = Instantiate(_data.Effectparticles, transform);
       
    }

    public void RemoveEffect()
    {
        _data = null;
        _currentEffectTime = 0;
        _nextTickTime = 0;
        //if (_effectParticles != null) Destroy(_effectParticles);
        
    }

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
    }

    // Added: heal the entity by a given amount (clamped to maxHealth)
    public void Heal(float amount)
    {
        if (amount <= 0) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private void Update()
    {
        if (_data != null) HandleEffect();

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            currentHealth = maxHealth;
            var tracker = FindFirstObjectByType<KillCountTracker>();
            if (tracker != null)
                tracker.IncrementKillCount();
        }

        if (currentHealth <= 0)
        {
            transform.parent.gameObject.SetActive(false);
            currentHealth = maxHealth;
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
            _nextTickTime += _data.TickSpeed;
            currentHealth -= _data.DOTAmount;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}

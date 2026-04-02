using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float maxHealth;
    void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (health <= 0)
        {
            gameObject.SetActive(false);
            health = maxHealth;
            var tracker = FindFirstObjectByType<KillCountTracker>();
            if (tracker != null)
                tracker.IncrementKillCount();

        }
    }

    
}





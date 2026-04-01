using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    public float maxHealth;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            transform.parent.gameObject.SetActive(false);
            health = maxHealth;

        }
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        
        
    }
}

using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Settings")]
    public string identity = "Unnamed Entity";
    public float health = 100f;
    public float maxHealth = 100f;
    public bool isImmortal = false;

    public virtual void TakeDamage(float damage)
    {
        if (isImmortal) return;

        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (health <= 0) Die();
    }

    public virtual void Heal(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
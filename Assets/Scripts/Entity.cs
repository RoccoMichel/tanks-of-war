using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Settings")]
    public string identity = "Unnamed Entity";
    public float health = 100f;
    public float maxHealth = 100f;
    public bool isImmortal = false;

    public virtual void Explode(float damage, float force, float radius)
    {
        //Instantiate(Resources.Load<GameObject>("Substances/Effects/Explosion.prefab"), transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider victim in colliders)
        {
            if (victim.transform.gameObject == gameObject) continue;

            Entity entity = victim.GetComponent<Entity>();
            Rigidbody rigidbody = victim.GetComponent<Rigidbody>();

            if (entity != null)
                entity.TakeDamage(damage);

            if (rigidbody != null)
                rigidbody.AddExplosionForce(force, transform.position, radius);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        // Play Effects & Sounds (load from resources)

        if (isImmortal) return;

        health -= damage;

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
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Settings")]
    public string identity = "Unnamed Entity";
    public float health = 100f;
    public float maxHealth = 100f;
    public bool isImmortal = false;
    public Substance substance;

    public enum Substance
    {
        /// <summary>
        /// Made of nothing or should display Nothing
        /// </summary>
        Void,

        // Biological entities
        Flesh,     // Raw organic flesh or biological tissue
        Alien,     // Alien creatures, with different biological properties
        Plant,     // Plant-based entities or creatures (earth-based or alien flora)

        // Materials
        Glass,     // Glass-like entities or objects
        Concrete,  // Concrete-like entities or objects
        Wood,      // Wood-like entities or objects
        Metal,     // Metal-like entities or objects
        Stone,     // Stone-like entities or objects

        // Synthetic & Special Materials
        Synthetic, // Man-made or artificially created materials or creatures
        Crystal,   // Crystal-like entities or materials
        Liquid,    // Liquid-based entities or materials
        Energy,    // Pure energy-based entities or forms (e.g., plasma, magical energy)
    }

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
using UnityEngine;

public class AntiTankMine : MonoBehaviour
{
    public float damage = 40;
    public GameObject explosionEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity victim))
        {
            if (victim == null) return;

            victim.TakeDamage(damage);
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            Destroy(explosion.gameObject, explosion.main.duration);
            Destroy(gameObject);
        }
    }
}

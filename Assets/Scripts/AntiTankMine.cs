using UnityEngine;

public class AntiTankMine : MonoBehaviour
{
    public float damage = 30;
    public GameObject explosionEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<BasePlayer>().TakeDamage(damage);
            ParticleSystem explosion = Instantiate(explosionEffect).GetComponent<ParticleSystem>();
            Destroy(explosion.gameObject, explosion.main.duration);
            Destroy(gameObject);
        }
    }
}

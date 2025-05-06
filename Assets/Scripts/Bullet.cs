using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 5;
    public float speed = 10;
    private new Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rigidbody.linearVelocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent(out Entity entity)) return;

        entity.TakeDamage(damage);
        Destroy(gameObject);
    }
}

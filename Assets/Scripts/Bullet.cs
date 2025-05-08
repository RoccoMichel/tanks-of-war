using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 5;
    public float speed = 10;
    public float range = 8;
    [SerializeField] protected GameObject landEffect;
    private new Rigidbody2D rigidbody;
    private Vector3 startPosition;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void Update()
    {
        // High tech Movement
        rigidbody.linearVelocity = transform.up * speed;

        // Bullet lands on the ground after its range
        if (Vector3.Distance(startPosition, transform.position) > range) Land();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // effect / sound

        if (!collision.transform.TryGetComponent(out Entity entity))
        {
            Destroy(gameObject);
            return;        
        }

        entity.TakeDamage(damage);
        Destroy(gameObject);
    }

    public virtual void Land()
    {
        Destroy(Instantiate(landEffect, transform.position, transform.rotation), 1);
        Destroy(gameObject);
    }
}

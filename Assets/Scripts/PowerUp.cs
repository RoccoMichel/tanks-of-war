using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.transform.CompareTag("Player")) return;

        TankMovement movement = collision.GetComponent<TankMovement>();
        Turret turret = collision.GetComponent<Turret>();

        Debug.Log(collision.GetComponent<Entity>().identity + " has been powered up!");

        Destroy(gameObject);
    }
}
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Tooltip("Ignore if not relevant")]
    public int specifier;
    public Powers powerUp;
    public enum Powers
    {
        refillFull,
        refillSpecific,
        healthFull,
        healthSpecific,

        // NOT IMPLEMENTED YET:

        speedUp,
        invincible
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.transform.CompareTag("Player")) return;

        Turret turret = collision.GetComponent<Turret>();
        BasePlayer player = collision.GetComponent<BasePlayer>();
        TankMovement movement = collision.GetComponent<TankMovement>();


        switch (powerUp)
        {
            case Powers.refillFull:
                if (!turret.CanReload()) return;
                turret.RefillFull();

                break;

            case Powers.refillSpecific:
                if (!turret.CanReload()) return;
                turret.RefillSpecific(specifier);

                break;

            case Powers.healthFull:
                if (player.health >= player.maxHealth) return;
                player.Heal(player.maxHealth);

                break;

            case Powers.healthSpecific:
                if (player.health >= player.maxHealth) return;
                player.Heal(specifier);

                break;

            case Powers.speedUp:
                movement.driveSpeed++;                      //////////

                break;

            case Powers.invincible:
                player.isImmortal = true;                   //////////

                break;
        }

        // Effect / Sound

        Destroy(gameObject);
    }
}
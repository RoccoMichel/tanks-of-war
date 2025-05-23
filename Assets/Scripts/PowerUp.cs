using Photon.Pun;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool limited;
    [Tooltip("Ignore if not relevant")]
    public float limitedTime = 5;
    public int specifier;
    private float timer;
    private ChatManager chat;
    public Powers powerUp;

    public enum Powers
    {
        None,
        RefillFull,
        RefillSpecific,
        HealthFull,
        HealthSpecific,
        Boost,
        Shield,
        Overdrive
    }

    private void Update()
    {
        if (chat == null) chat = FindAnyObjectByType<ChatManager>();

        // If object has limited time delete when it expires
        if (!limited) return;

        timer += Time.deltaTime;
        if (timer > limitedTime) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Power-Ups only work on players
        if (!collision.transform.CompareTag("Player")) return;

        Turret turret = collision.GetComponentInChildren<Turret>();
        BasePlayer player = collision.GetComponent<BasePlayer>();
        TankMovement movement = collision.GetComponent<TankMovement>();

        switch (powerUp)
        {
            case Powers.RefillFull:
                if (!turret.CanRefill()) return;
                turret.RefillFull();

                break;

            case Powers.RefillSpecific:
                if (!turret.CanRefill()) return;
                turret.RefillSpecific(specifier);

                break;

            case Powers.HealthFull:
                if (player.health >= player.maxHealth) return;
                player.Heal(player.maxHealth);

                break;

            case Powers.HealthSpecific:
                if (player.health >= player.maxHealth) return;
                player.Heal(specifier);

                break;

            case Powers.Boost:
                movement.Boost(specifier);
                try { if (PhotonNetwork.IsMasterClient) chat.SendChatMessage(player.identity, $"Has a {specifier} second Booster!"); }
                catch { }                

                break;

            case Powers.Shield:
                player.Shield(specifier);
                try { if (PhotonNetwork.IsMasterClient) chat.SendChatMessage(player.identity, $"Has a {specifier} second Shield!"); }
                catch { }                

                break;

            case Powers.Overdrive:
                turret.Overdrive(specifier);
                try { if (PhotonNetwork.IsMasterClient) chat.SendChatMessage(player.identity, $"Has a {specifier} second Overdrive!"); }
                catch { }                

                break;
        }

        Destroy(gameObject);
    }
}
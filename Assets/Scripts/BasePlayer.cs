using Photon.Pun;
using UnityEngine;

public class BasePlayer : Entity
{
    [Header("Player Settings")]
    public int score;
    [SerializeField] ParticleSystem damageEffect;
    private PhotonView view;

    

    private void Start()
    {
        view = GetComponent<PhotonView>();
        SetHealthBasedOnRoom();
    }

    private void Update()
    {
        var effect = damageEffect.emission;
        float rateOverTime = Mathf.Lerp(40, 0, Mathf.InverseLerp(0, maxHealth / 3, health));
        effect.rateOverTime = Mathf.Floor(Mathf.Pow(rateOverTime, 1.2f));
    }
    
    public virtual void RequestDamage(float amount)
    {
        if (!view.IsMine) return;

        view.RPC(nameof(TakeDamage), RpcTarget.All, amount);
    }

    [PunRPC]
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public virtual bool TryHeal(int amount)
    {
        if (health >= maxHealth) return false;

        Heal(amount);
        return true;
    }

    public override void Die()
    {
        Debug.Log("Player Died");
        gameObject.SetActive(false);
    }

    public void AddScore(int amount)
    {
        score += amount;
    }


    public void SetHealthBasedOnRoom()
    {
        view.RPC(nameof(RequestHealth), RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    protected void RequestHealth(int actorNumber)
    {
        view.RPC(nameof(ReceiveHealth), PhotonNetwork.CurrentRoom.GetPlayer(actorNumber), health);
    }

    [PunRPC]
    protected void ReceiveHealth(float newHealth)
    {
        health = newHealth;
    }
}
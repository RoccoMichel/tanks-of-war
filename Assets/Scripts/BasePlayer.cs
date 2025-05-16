using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BasePlayer : Entity
{
    [Header("Player Settings")]
    public int score;
    public int deaths;
    [SerializeField] ParticleSystem damageEffect;
    [SerializeField] GameObject shield;
    private GamemodeManager gamemode;
    private PhotonView view;
    private float shieldTimer;

    private void Start()
    {
        view = GetComponent<PhotonView>();

        try { gamemode = GameObject.FindGameObjectWithTag("GameController").GetComponent<GamemodeManager>(); }
        catch { Debug.LogWarning("No GameManager found in Scene!"); }

        StartCoroutine(StartRequests());
    }
    private IEnumerator StartRequests()
    {
        while (!PhotonNetwork.InRoom || view.ViewID != 0) yield return null;

        if (view.IsMine) view.RPC(nameof(SetIdentity), RpcTarget.All, PhotonNetwork.NickName);
        else view.RPC(nameof(UpdateIdentity), RpcTarget.Others);

        view.RPC(nameof(RequestInfo), RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void Update()
    {
        var effect = damageEffect.emission;
        float rateOverTime = Mathf.Lerp(40, 0, Mathf.InverseLerp(0, maxHealth / 3, health));
        effect.rateOverTime = Mathf.Floor(Mathf.Pow(rateOverTime, 1.2f));

        shieldTimer = Mathf.Clamp(shieldTimer -= Time.deltaTime, 0, int.MaxValue);
        isImmortal = shieldTimer > 0;
        shield.SetActive(isImmortal);
    }
    
    public void Shield(float timeSeconds)
    {
        shieldTimer += timeSeconds;
    }

    public virtual void RequestDamage(float amount, BasePlayer source)
    {
        if (!view.IsMine) return;

        view.RPC(nameof(TakeDamage), RpcTarget.All, amount, source.view.ViewID);
    }   

    [PunRPC]
    public void TakeDamage(float damage, int sourceViewID)
    {
        if (isImmortal) return;

        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (health <= 0)
        {
            gamemode.Kill(PhotonView.Find(sourceViewID).GetComponent<BasePlayer>(), this);
            Die();
        }
    }

    public virtual bool TryHeal(int amount)
    {
        if (health >= maxHealth) return false;

        Heal(amount);
        return true;
    }

    public override void Die()
    {
        shieldTimer = 0;
        view.RPC(nameof(AddDeath), RpcTarget.All);

        if (gamemode.gamemode == GamemodeManager.Gamemodes.Deathmatch) Invoke(nameof(Respawn), 5);
        if (gamemode.gamemode == GamemodeManager.Gamemodes.Explore) Invoke(nameof(Respawn), 0.5f);

        gameObject.SetActive(false);
    }

    public void DisablePlayer()
    {
        shieldTimer = 0;
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void AddDeath()
    {
        deaths++;
    }

    public virtual void Respawn()
    {
        gameObject.SetActive(true);
        gamemode.GetComponent<PlayerSpawner>().SetPosition(transform);
        gameObject.GetComponentInChildren<Turret>().RefillHalfAmmo();
        health = maxHealth;
    }    
    public virtual void Respawn(float waitTime)
    {
        Invoke(nameof(Respawn), waitTime);
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    [PunRPC]
    protected void RequestInfo(int actorNumber)
    {
        view.RPC(nameof(ReceiveInfo), PhotonNetwork.CurrentRoom.GetPlayer(actorNumber), health, score, deaths, GetComponentInChildren<Turret>().bullet);
    }

    [PunRPC]
    protected void ReceiveInfo(float newHealth)
    {
        health = newHealth;
    }    
    [PunRPC]
    protected void ReceiveInfo(float newHealth, int newScore)
    {
        health = newHealth;
        score = newScore;
    }    
    [PunRPC]
    protected void ReceiveInfo(float newHealth, int newScore, int newDeaths)
    {
        health = newHealth;
        score = newScore;
        deaths = newDeaths;
    }    
    [PunRPC]
    protected void ReceiveInfo(float newHealth, int newScore, int newDeaths, int currentBullet)
    {
        health = newHealth;
        score = newScore;
        deaths = newDeaths;
        GetComponentInChildren<Turret>().bullet = currentBullet;
    }

    [PunRPC]
    protected void SetIdentity(string newIdentity)
    {
        identity = newIdentity;
    }    
    
    [PunRPC]
    protected void UpdateIdentity()
    {
        if (view.IsMine) view.RPC(nameof(SetIdentity), RpcTarget.Others, PhotonNetwork.NickName);
    }
}
using UnityEngine;

public class BasePlayer : Entity
{
    [Header("Player Settings")]
    public int score;
    [SerializeField] ParticleSystem damageEffect;

    private void Update()
    {
        var effect = damageEffect.emission;
        float rateOverTime = Mathf.Lerp(40, 0, Mathf.InverseLerp(0, maxHealth / 3, health));
        effect.rateOverTime = Mathf.Floor(Mathf.Pow(rateOverTime, 1.2f));
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
        health += amount;
    }
}
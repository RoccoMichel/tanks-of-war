using UnityEngine;

public class BasePlayer : Entity
{
    public virtual bool TryHeal(int amount)
    {
        if (health >= maxHealth) return false;

        Heal(amount);
        return true;
    }

    public override void Die()
    {
        Debug.Log("Player Died");
    }
}
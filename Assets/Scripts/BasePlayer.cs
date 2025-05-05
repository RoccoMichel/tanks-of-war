using UnityEngine;

public class BasePlayer : Entity
{
    [Header("Player Settings")]
    public float armor = 0;
    public float maxArmor = 100;

    public override void TakeDamage(float damage)
    {
        if (isImmortal) return;

        // Calculate damage with Player Armor
        float healthDamage = damage;
        float armorDamage = 0;
        for (int i = 0; i < Mathf.FloorToInt(damage); i++)
        {
            if (armorDamage >= armor) break;
            // Damage if Player has Armor for that Health Point

            // Armor absorbs half the damage to seduce base health damage by 1/3
            armorDamage += 0.5f;
            healthDamage -= 0.666f;
        }

        // Apply Damage
        health -= Mathf.Floor(healthDamage);
        armor -= Mathf.Floor(armorDamage);

        // Sound and Visual Indication

        // Death
        if (health <= 0) Die();
    }

    public virtual void AddArmor(int amount)
    {
        armor = Mathf.Clamp(armor + amount, 0, maxArmor);
    }

    public virtual bool TryAddArmor(int amount)
    {
        if (armor >= maxArmor) return false;

        AddArmor(amount);
        return true;
    }

    public virtual bool TryHeal(int amount)
    {
        if (health >= maxHealth) return false;

        Heal(amount);
        return true;
    }

    public override void Die()
    {
        // Freeze player or something, anyway don't destroy it

        Debug.Log("Player Died");
    }
}
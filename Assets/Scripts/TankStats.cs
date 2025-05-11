using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankStats : MonoBehaviour
{
    public GameObject tankReference;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text stats;
    [SerializeField] private Image tankBody;
    [SerializeField] private Image tankBarrel;

    private void Start()
    {
        TankMovement movement = tankReference.GetComponent<TankMovement>();
        Turret turret = tankReference.GetComponentInChildren<Turret>();
        Bullet bullet = turret.bullets[turret.bullet].GetComponent<Bullet>();

        tankBody.sprite = tankReference.GetComponent<SpriteRenderer>().sprite;
        tankBarrel.sprite = turret.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;

        title.text = string.Empty;
        stats.text = string.Empty;

        title.text = tankReference.GetComponent<BasePlayer>().identity;
        stats.text = $"Tank Drive Speed: \t{movement.driveSpeed}";
        stats.text += $"\nRoad Speed Multiplier: \t{movement.roadMultiplier}";
        stats.text += $"\nTank Turn Speed: \t{movement.rotationSpeed}";
        stats.text += $"\nTurret Turn Speed: \t{turret.rotationSpeed}";
        stats.text += $"\nTurret Shot Delay: \t{turret.secondsBetweenFire}";
        stats.text += $"\nTurret Max Ammo: \t{turret.maxAmmo}";
        stats.text += $"\nBullet Damage: \t{bullet.damage}";
        stats.text += $"\nBullet Speed: \t{bullet.speed}";
        stats.text += $"\nBullet Range: \t{bullet.range}";
    }
}
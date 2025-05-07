using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [Header("Ammo")]
    public bool infiniteAmmo;
    public int ammo = 10;
    public int maxAmmo = 10;

    [Header("Shooting")]
    public float turnSpeed = 3;
    public float secondsBetweenFire = 0.5f;
    protected float timer;

    [Header("References")]
    public GameObject bullet;
    public GameObject muzzleFlash;
    public Transform muzzle;
    private InputAction attackAction;

    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        // Rotating
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime * 100f);

        // Shooting
        if (attackAction.WasPressedThisFrame() && timer == 0 && (ammo > 0 || infiniteAmmo)) Shoot(bullet);
        timer = Mathf.Clamp(timer - Time.deltaTime, 0, float.MaxValue);
    }

    public virtual void Shoot(GameObject bullet)
    {
        timer = secondsBetweenFire;
        if (!infiniteAmmo) ammo--;

        Instantiate(bullet, muzzle.position, transform.rotation);
        Destroy(Instantiate(muzzleFlash, muzzle.position, muzzle.rotation), 0.4f);
    }

    /// <summary>
    /// Sets ammo to maxAmmo no matter what
    /// </summary>
    public virtual void RefillFull() 
    {
        ammo = maxAmmo;
    }

    public virtual void RefillSpecific(int amount)
    {
        ammo = Mathf.Clamp(ammo + amount, 0, maxAmmo);
    }

    public bool CanReload()
    {
        if (ammo < maxAmmo) return true;
        else return false;
    }
}
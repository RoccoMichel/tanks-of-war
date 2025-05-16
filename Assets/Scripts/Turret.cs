using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [Header("Ammo")]
    public bool infiniteAmmo;
    public int ammo = 10;
    public int maxAmmo = 10;

    [Header("Shooting")]
    public float rotationSpeed = 3;
    public float secondsBetweenFire = 0.5f;
    protected float timer;
    private float overdriveTimer;

    [Header("References")]
    public int bullet;
    public GameObject[] bullets;
    public GameObject muzzleFlash;
    public GameObject overdriveEffect;
    public Transform muzzle;
    private InputAction attackAction;
    private PhotonView view;


    private void OnEnable()
    {
        // reset variables if it gameObject gets reenabled
        timer = 0;
        overdriveTimer = 0;
    }

    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        view = GetComponent<PhotonView>();
        ammo = maxAmmo / 2;
    }

    private void Update()
    {
        // Shooting Cooldown
        timer = Mathf.Clamp(timer - Time.deltaTime, 0, int.MaxValue);

        // Overdrive Power-Up
        overdriveTimer = Mathf.Clamp(overdriveTimer - Time.deltaTime, 0, int.MaxValue);
        overdriveEffect.SetActive(overdriveTimer > 0);

        if (!view.IsMine) return;

        // Rotating
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 100f);

        // Shooting
        if (attackAction.WasPressedThisFrame() && timer == 0 && (ammo > 0 || infiniteAmmo))
        {
            if (PhotonNetwork.InRoom) view.RPC("Shoot", RpcTarget.All, bullet);
            else Shoot(bullet);
        }
    }

    [PunRPC]
    public virtual void Shoot(int bullet)
    {
        // reset timer and reduce ammo (exception: in overdrive mode)
        timer = overdriveTimer > 0 ? 0.1f : secondsBetweenFire;
        if (infiniteAmmo || overdriveTimer > 0) overdriveTimer += 0.1f;
        else ammo--;

        // Create the bullet and effect
        Instantiate(bullets[bullet], muzzle.position, transform.rotation).GetComponent<Bullet>().origin = GetComponentInParent<BasePlayer>();
        Destroy(Instantiate(muzzleFlash, muzzle.position, muzzle.rotation), 0.4f);
    }

    /// <summary>
    /// Start or prolong turret overdrive
    /// </summary>
    /// <param name="timeSeconds">added time in seconds</param>
    public void Overdrive(float timeSeconds)
    {
        overdriveTimer += timeSeconds;
        timer = 0;
    }

    /// <summary>
    /// Sets ammo to maxAmmo no matter what
    /// </summary>
    public virtual void RefillFull() 
    {
        ammo = maxAmmo;
    }

    /// <summary>
    /// Add an specific amount of ammo to the already existing Turret ammo
    /// </summary>
    public virtual void RefillSpecific(int amount)
    {
        ammo = Mathf.Clamp(ammo + amount, 0, maxAmmo);
    }

    /// <summary>
    /// Sets Turret ammo to half that of maxAmmo
    /// </summary>
    public virtual void RefillHalfAmmo()
    {
        ammo = maxAmmo / 2;
    }

    /// <summary>
    /// Check if ammo is maxed out
    /// </summary>
    public bool CanRefill()
    {
        if (ammo < maxAmmo) return true;
        else return false;
    }
}
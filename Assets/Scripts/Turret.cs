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
        timer = 0;
        overdriveTimer = 0;
        ammo = Mathf.CeilToInt(maxAmmo / 2);
    }

    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        timer = Mathf.Clamp(timer - Time.deltaTime, 0, int.MaxValue);
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
            if (PhotonNetwork.IsConnected) view.RPC("Shoot", RpcTarget.All, bullet);
            else Shoot(bullet);
        }
    }

    [PunRPC]
    public virtual void Shoot(int bullet)
    {
        timer = overdriveTimer > 0 ? 0.1f : secondsBetweenFire;
        if (infiniteAmmo || overdriveTimer > 0) overdriveTimer += 0.1f;
        else ammo--;

        Instantiate(bullets[bullet], muzzle.position, transform.rotation).GetComponent<Bullet>().origin = GetComponentInParent<BasePlayer>();
        Destroy(Instantiate(muzzleFlash, muzzle.position, muzzle.rotation), 0.4f);
    }

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
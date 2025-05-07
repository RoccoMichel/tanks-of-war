using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text ammo;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // player get local photon client as object
    }

    private void Update()
    {
        health.text = player.GetComponent<BasePlayer>().health.ToString();
        ammo.text = player.GetComponentInChildren<Turret>().ammo.ToString();
    }
}

using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] TMP_Text code;
    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text ammo;
    private GameObject player;
    private InputAction quickLookAction;
    private InputAction menuAction;
    internal string[] RoomInfo;

    private void Start()
    {
        if (PhotonNetwork.IsConnected) RoomInfo = PhotonNetwork.CurrentRoom.ToString().Split('\'');

        quickLookAction = InputSystem.actions.FindAction("QuickLook");
        menuAction = InputSystem.actions.FindAction("Menu");
        code.text = $"Code: {RoomInfo[1]}";
        menu.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView view = player.GetComponent<PhotonView>();
                if (view != null && view.IsMine)
                {
                    this.player = player;
                    break;
                }
            }
        }
        else
        {
            health.text = "HEALTH: " + player.GetComponent<BasePlayer>().health.ToString();
            ammo.text = "AMMO: " + player.GetComponentInChildren<Turret>().ammo.ToString();
        }

        if (quickLookAction.WasPressedThisFrame()) Debug.Log("How quick menu!");

        if (menuAction.WasPressedThisFrame()) menu.SetActive(!menu.activeInHierarchy);
    }

    public void Exit()
    {
        PhotonNetwork.LeaveRoom(false);
        SceneManager.LoadScene(0);
    }
}
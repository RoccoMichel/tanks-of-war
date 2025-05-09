using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private TMP_Text code;
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text ammo;
    [SerializeField] private TMP_Text score;
    [SerializeField] private GamemodeManager gamemode;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private GameObject glanceboard;
    private GameObject player;
    private InputAction quickLookAction;
    private InputAction menuAction;
    internal string[] RoomInfo;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomInfo = PhotonNetwork.CurrentRoom.ToString().Split('\'');
            code.text = $"Code: {RoomInfo[1]}";
        }

        if (gamemode == null) gamemode = GameObject.FindGameObjectWithTag("GameController").GetComponent<GamemodeManager>();
        quickLookAction = InputSystem.actions.FindAction("QuickLook");
        menuAction = InputSystem.actions.FindAction("Menu");
        leaderboard.SetActive(false);
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
            score.text = "POINTS: " + player.GetComponent<BasePlayer>().score.ToString();
        }

        if (quickLookAction.WasPressedThisFrame())
        {
            leaderboard.SetActive(!leaderboard.activeInHierarchy);
            glanceboard.SetActive(!glanceboard.activeInHierarchy);
        }

        if (leaderboard.activeInHierarchy)
        {
            TMP_Text textMesh = leaderboard.GetComponentInChildren<TMP_Text>();
            textMesh.text = "[LEADERBOARD]\n";

            foreach (BasePlayer player in gamemode.playerList)
                textMesh.text += $"{player.identity}\t| {player.score} points ({player.deaths})\n";

        }
        if (glanceboard.activeInHierarchy)
        {
            string leaderInfo;

            try { leaderInfo = $"{gamemode.playerList[0].identity}\t| {gamemode.playerList[0].score} points"; }
            catch { leaderInfo = "[Leaderinfo]"; }

            glanceboard.GetComponent<TMP_Text>().text = leaderInfo;
        }

        if (menuAction.WasPressedThisFrame()) menu.SetActive(!menu.activeInHierarchy);
    }

    public void Exit()
    {
        FindAnyObjectByType<ChatManager>().SendMessage(PhotonNetwork.NickName, "has left.");
        Invoke(nameof(Disconnect), 0.5f);
    }

    void Disconnect()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
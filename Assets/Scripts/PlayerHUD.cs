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
    [SerializeField] private GamemodeManager gamemode;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private GameObject glanceboard;
    private GameObject player;
    private BasePlayer playerScript;
    private InputAction quickLookAction;
    private InputAction menuAction;
    internal string[] RoomInfo;

    private void Start()
    {
        if (leaderboard != null) leaderboard.SetActive(false);
        if (menu != null) menu.SetActive(false);

        if (PhotonNetwork.InRoom)
        {
            RoomInfo = PhotonNetwork.CurrentRoom.ToString().Split('\'');
            code.text = $"Code: {RoomInfo[1]}\n{(PhotonNetwork.CurrentRoom.IsOpen ? "public" : "private")}";
        }

        if (gamemode == null)
        {
            try { gamemode = GameObject.FindGameObjectWithTag("GameController").GetComponent<GamemodeManager>(); }
            catch { Debug.LogWarning("No GameManager found in Scene!"); }
        }

        quickLookAction = InputSystem.actions.FindAction("QuickLook");
        menuAction = InputSystem.actions.FindAction("Menu");
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
                    playerScript = player.GetComponent<BasePlayer>();
                    break;
                }
            }
        }
        else
        {
            health.text = "HP: " + playerScript.health.ToString();
            ammo.text = player.GetComponentInChildren<Turret>().ammo.ToString() + " Round(s) Left";
        }

        if (quickLookAction.WasPressedThisFrame())
        {
            leaderboard.SetActive(!leaderboard.activeInHierarchy);
            glanceboard.SetActive(!glanceboard.activeInHierarchy);
        }

        if (leaderboard != null && leaderboard.activeInHierarchy)
        {
            TMP_Text textMesh = leaderboard.GetComponentInChildren<TMP_Text>();
            textMesh.text = "[LEADERBOARD]\n";

            foreach (BasePlayer player in gamemode.playerList)
                textMesh.text += $"{player.identity}\t| {player.score} points ({player.deaths})\n";

        }

        if (glanceboard != null && glanceboard.activeInHierarchy)
        {
            string leaderInfo;
            int localRank = gamemode.GetPlayerRank(playerScript);

            try 
            {
                if (localRank == 0) leaderInfo = $"YOU\t| {gamemode.playerList[localRank].score} points";
                else
                {
                    leaderInfo = $"{gamemode.playerList[0].identity}\t| {gamemode.playerList[0].score} points";
                    leaderInfo += $"\nYOU\t| {gamemode.playerList[localRank].score} points";
                }
            }
            catch { leaderInfo = "[Leader info]"; }

            glanceboard.GetComponent<TMP_Text>().text = leaderInfo;
        }

        if (menuAction.WasPressedThisFrame()) menu.SetActive(!menu.activeInHierarchy);
    }

    public void Exit()
    {
        if (PhotonNetwork.IsConnected)
        {
            player = null;
            PhotonNetwork.Disconnect();
            FindAnyObjectByType<ChatManager>().SendChatMessage(PhotonNetwork.NickName, "has left.");
        }
        Invoke(nameof(Disconnect), 0.5f);
    }

    void Disconnect()
    {        
        SceneManager.LoadScene(0);
    }
}
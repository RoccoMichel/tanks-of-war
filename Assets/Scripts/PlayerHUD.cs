using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private void Start()
    {
        if (leaderboard != null) leaderboard.SetActive(false);
        if (menu != null) menu.SetActive(false);

        quickLookAction = InputSystem.actions.FindAction("QuickLook");
        menuAction = InputSystem.actions.FindAction("Menu");

        StartCoroutine(StartRequests());
    }

    private IEnumerator StartRequests()
    {
        yield return new WaitForSeconds(1);

        if (PhotonNetwork.InRoom)
        {
            print(PhotonNetwork.CurrentRoom.Name);
            code.text = $"CODE:\t{PhotonNetwork.CurrentRoom.Name}\n[{(PhotonNetwork.CurrentRoom.IsOpen ? "public" : "private")}]";
        }

        if (gamemode == null)
        {
            try { gamemode = GameObject.FindGameObjectWithTag("GameController").GetComponent<GamemodeManager>(); }
            catch { Debug.LogWarning("No GameManager found in Scene!"); }
        }
    }

    private void Update()
    {
        if (player == null) // Get a valid Player to reference from
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
        else // Display Player health and ammo
        {
            health.text = "HP: " + playerScript.health.ToString();
            ammo.text = player.GetComponentInChildren<Turret>().ammo.ToString() + " Round(s) Left";
        }

        // Toggle between leaderboard and glanceboard
        if (quickLookAction.WasPressedThisFrame())
        {
            leaderboard.SetActive(!leaderboard.activeInHierarchy);
            glanceboard.SetActive(!glanceboard.activeInHierarchy);
        }

        // Write to leaderboard
        if (leaderboard != null && leaderboard.activeInHierarchy)
        {
            TMP_Text textMesh = leaderboard.GetComponentInChildren<TMP_Text>();
            textMesh.text = "[LEADERBOARD]\n";

            foreach (BasePlayer player in gamemode.playerList)
                textMesh.text += $"{player.identity}\t| {player.score} points ({player.deaths})\n";

        }

        // Write to glanceboard
        if (glanceboard != null && glanceboard.activeInHierarchy)
        {
            string leaderInfo;
            int localRank = gamemode.GetPlayerRank(playerScript);

            try 
            {
                // should display the leading player and you own score
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

        // Toggle Exit Menu
        if (menuAction.WasPressedThisFrame()) menu.SetActive(!menu.activeInHierarchy);
    }

    // Leave the Room and return to Main Menu
    public void Exit()
    {
        if (PhotonNetwork.InRoom)
        {
            player = null;
            PhotonNetwork.LeaveRoom();
            FindAnyObjectByType<ChatManager>().SendChatMessage(PhotonNetwork.NickName, "has left.");
        }
        Invoke(nameof(Disconnect), 0.5f);
    }

    public void ShowLeaderboard(bool b)
    {
        leaderboard.SetActive(b);
        glanceboard.SetActive(!b);
    }

    void Disconnect()
    {        
        SceneManager.LoadScene(0);
    }
}
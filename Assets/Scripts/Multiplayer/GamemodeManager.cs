using TMPro;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GamemodeManager : MonoBehaviour
{
    public Gamemodes gamemode;
    public TMP_Text gamemodeInfo;
    public TMP_Text announceDisplay;
    public List<BasePlayer> playerList = new();
    private ChatManager chat;
    private PhotonView view;
    private const float DEATHMATCHLENGTH = 5 * 60;
    private bool inDuel;
    private bool cooldown;
    [SerializeField] private float timer;

    public enum Gamemodes
    {
        Deathmatch,
        LastStanding,
        Explore
    }

    private void Start()
    {
        if (chat == null) chat = FindAnyObjectByType<ChatManager>();
        view = GetComponent<PhotonView>();
        timer = DEATHMATCHLENGTH;
                
        StartCoroutine(StartRequests());
    }

    private IEnumerator StartRequests()
    {
        // Sync required data with the Network when connected
        yield return new WaitForSeconds(1);

        if (PhotonNetwork.IsMasterClient) gamemode = (Gamemodes)PlayerPrefs.GetInt("Preferred Gamemode", 2);
        else view.RPC(nameof(RequestInfo), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

        yield return new WaitForSeconds(0.5f);

        Announce(gamemode.ToString(), 3.5f);
    }

    /// <summary>
    /// Gamemode where players need to get as many kills with in a time duration
    /// announce a winner when time is up and start a new Round
    /// </summary>
    void Deathmatch()
    {
        timer = Mathf.Clamp(timer -= Time.deltaTime, 0, int.MaxValue);
        gamemodeInfo.text = $"[ {TimerLogic(timer)} ]";

        if (timer == 0) StartCoroutine(DeathmatchRoundOver(10));
    }

    IEnumerator DeathmatchRoundOver(float breakLength)
    {
        timer = DEATHMATCHLENGTH;

        RoundOver(true, breakLength, playerList[0]);

        yield return new WaitForSeconds(breakLength);

        CallPlayerListUpdate();
        FindAnyObjectByType<PlayerHUD>().ShowLeaderboard(false);
    }

    /// <summary>
    /// Last player alive gets a point then the round restarts
    /// </summary>
    void LastStanding()
    {
        // make sure there are enough players to duel
        if (playerList.Count <= 1)
        {
            cooldown = false;
            PlayerListUpdate();
            gamemodeInfo.text = "waiting for more players to join...";
            return;
        }
        else if (!inDuel && !cooldown)
        {
            Announce("Starting in 10 seconds!", 5);

            foreach (BasePlayer player in playerList) player.Shield(1);
            Invoke(nameof(SetInDuel), 10);
            cooldown = true;
            return;
        }
        else if (!inDuel && cooldown)
        {
            foreach (BasePlayer player in playerList) player.Shield(Time.deltaTime);
            gamemodeInfo.text = "starting soon...";
            return;
        }

        int alive = 0;
        foreach (BasePlayer player in playerList) if (player.health > 0) alive++;
        gamemodeInfo.text = $"{alive} players left!";

        if (alive < 2 && inDuel)
        {
            foreach (BasePlayer player in playerList)
            {
                if (!player.isActiveAndEnabled || player.health <= 0) continue;
                
                inDuel = false;
                RoundOver(false, 6, player);
                if (PhotonNetwork.IsMasterClient) player.score++;

                Invoke(nameof(SetInDuel), 8);
                break;
            }
        }
    }

    /// <summary>
    /// No gamemode constraints player are free to do as they want
    /// </summary>
    void Explore()
    {
        gamemodeInfo.text = "exploring...";
    }

    private void Update()
    {
        // Update loop depending on active gamemode
        switch (gamemode)
        {
            case Gamemodes.Deathmatch:
                Deathmatch(); break;

            case Gamemodes.LastStanding:
                LastStanding(); break;

            case Gamemodes.Explore:
                Explore(); break;
        }

        // Sort player list by user with the highest score
        if (playerList.Count > 1 && playerList != null)
            playerList.Sort((a, b) => b.score.CompareTo(a.score));
    }

    public void CallPlayerListUpdate()
    {
        view.RPC(nameof(PlayerListUpdate), RpcTarget.All);
    }

    /// <summary>
    /// Get the position in the leader board of a certain player
    /// </summary>
    /// <param name="player">comparing user</param>
    /// <returns>Element index (0 is highest)</returns>
    public int GetPlayerRank(BasePlayer player)
    {
        int rank;

        for (rank = 0;  rank < playerList.Count; rank++)
        {
            if (player == playerList[rank]) break;
        }

        return rank;
    }

    /// <summary>
    /// Necessary functions to end the Round and start the next
    /// </summary>
    /// <param name="resetStats">Should it reset player score and death count</param>
    /// <param name="nextRoundInSeconds">time until players get respawned</param>
    /// <param name="winner">congratulated user in the UI</param>
    private void RoundOver(bool resetStats, float nextRoundInSeconds, BasePlayer winner)
    {
        if (resetStats) Invoke(nameof(ResetStats), nextRoundInSeconds + 0.5f);
        foreach (BasePlayer player in playerList)
        {
            player.DisablePlayer();
            player.Respawn(nextRoundInSeconds);
        }

        Announce($"{winner.identity} has won", 5);
        FindAnyObjectByType<PlayerHUD>().ShowLeaderboard(true);

        if (!PhotonNetwork.IsMasterClient) return;
        FindAnyObjectByType<ChatManager>().SendChatMessage("SYSTEM", $"{winner.identity} has won {gamemode}!");
        FindAnyObjectByType<ChatManager>().SendChatMessage("SYSTEM", $"Starting next round in {nextRoundInSeconds} seconds.");
    }

    private void SetInDuel()
    {
        inDuel = true;
    }

    /// <summary>
    /// Show a message in big text on the screen
    /// </summary>
    /// <param name="message">display string</param>
    /// <param name="duration">time until it fades to </param>
    [PunRPC]
    public void Announce(string message, float duration)
    {
        announceDisplay.text = message;
        announceDisplay.color = Color.white;
        announceDisplay.CrossFadeAlpha(1, 0, true);
        announceDisplay.CrossFadeAlpha(0, duration, false);
    }

    /// <summary>
    /// Reset player score and death count
    /// </summary>
    private void ResetStats()
    {
        PlayerListUpdate();
        foreach (BasePlayer player in playerList)
        {
            player.score = 0;
            player.deaths = 0;
        }
    }

    [PunRPC]
    private void PlayerListUpdate()
    {
        playerList.Clear();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            playerList.Add(player.GetComponent<BasePlayer>());
    }

    [PunRPC]
    private void RequestInfo(int actorNumber)
    {
        view.RPC(nameof(ReceiveInfo), PhotonNetwork.CurrentRoom.GetPlayer(actorNumber), timer, (int)gamemode);
    }

    [PunRPC]
    private void ReceiveInfo(float newTime, int currentGamemode)
    {
        timer = newTime;
        gamemode = (Gamemodes)currentGamemode;
    }

    /// <summary>
    /// If Deathmatch give killer a point
    /// </summary>
    /// <param name="killer">Score Receiver</param>
    /// <param name="victim">Person who died</param>
    public void Kill(BasePlayer killer, BasePlayer victim)
    {
        try { if (PhotonNetwork.IsMasterClient) chat.SendChatMessage(killer.identity, $"killed {victim.identity}"); }
        catch { }

        if (gamemode == Gamemodes.Deathmatch) killer.AddScore(1);
    }

    // Translate float into minutes and seconds
    public string TimerLogic(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        string min;
        string sec;

        if (minutes < 10) min = ("0" + minutes);
        else min = minutes.ToString();
        if (seconds < 10) sec = ("0" + seconds);
        else sec = seconds.ToString();

        return ($"{min}:{sec}");
    }
}

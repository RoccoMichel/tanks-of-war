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
    private PhotonView view;
    private const float DEATHMATCHLENGTH = 5 * 60;
    private float timer;

    public enum Gamemodes
    {
        Deathmatch,
        LastStanding,
        Explore
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
        timer = DEATHMATCHLENGTH;

        StartCoroutine(StartRequests());
    }

    private IEnumerator StartRequests()
    {
        while (!PhotonNetwork.InRoom) yield return null;

        if (PhotonNetwork.IsMasterClient) gamemode = (Gamemodes)PlayerPrefs.GetInt("Preferred Gamemode", 2);
        else view.RPC(nameof(RequestInfo), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

        Anncounce(gamemode.ToString(), 3.5f);
    }

    void Deathmatch()
    {
        timer = Mathf.Clamp(timer -= Time.deltaTime, 0, int.MaxValue);
        gamemodeInfo.text = $"[ {TimerLogic(timer)} ]";

        if (timer == 0) StartCoroutine(DeathmatchRoundOver(10));
    }

    IEnumerator DeathmatchRoundOver(float breakLength)
    {
        RoundOver(true, breakLength, playerList[0]);

        yield return new WaitForSeconds(breakLength);

        CallPlayerListUpdate();
        timer = DEATHMATCHLENGTH;
    }

    void LastStanding()
    {
        print(PhotonNetwork.CountOfPlayersInRooms);
        if (PhotonNetwork.CountOfPlayersInRooms <= 1)
        {
            gamemodeInfo.text = "waiting for more players to join...";
            return;
        }

        int alive = 0;
        foreach (BasePlayer player in playerList) if (player.health > 0) alive++;
        gamemodeInfo.text = $"{alive} players left!";

        if (alive < 2)
        {
            foreach (BasePlayer player in playerList)
            {
                if (player.health > 0)
                {
                    player.score++;
                    RoundOver(false, 10, player);

                    break;
                }
            }

            CallPlayerListUpdate();
        }
    }

    void Explore()
    {
        gamemodeInfo.text = "exploring...";
    }

    private void Update()
    {
        switch (gamemode)
        {
            case Gamemodes.Deathmatch:
                Deathmatch(); break;

            case Gamemodes.LastStanding:
                LastStanding(); break;

            case Gamemodes.Explore:
                Explore(); break;
        }

        if (playerList.Count > 0 && playerList != null)
            playerList.Sort((a, b) => b.score.CompareTo(a.score));
    }

    public void CallPlayerListUpdate()
    {
        view.RPC(nameof(PlayerListUpdate), RpcTarget.All);
    }

    public int GetPlayerRank(BasePlayer player)
    {
        int rank;

        for (rank = 0;  rank < playerList.Count; rank++)
        {
            if (player == playerList[rank]) break;
        }

        return rank;
    }


    private void RoundOver(bool resetStats, float nextRoundInSeconds, BasePlayer winner)
    {
        if (resetStats) Invoke(nameof(ResetStats), nextRoundInSeconds);
        foreach (BasePlayer player in playerList)
        {
            player.DisablePlayer();
            player.Respawn(nextRoundInSeconds);
        }

        Anncounce($"{winner.identity} has won", 5);
        FindAnyObjectByType<ChatManager>().SendChatMessage("SYSTEM", $"{winner.identity} has won {gamemode}!");
        FindAnyObjectByType<ChatManager>().SendChatMessage("SYSTEM", $"Starting next round in {nextRoundInSeconds} seconds.");
    }

    [PunRPC]
    public void Anncounce(string message, float duration)
    {
        announceDisplay.text = message;
        announceDisplay.color = Color.white;
        announceDisplay.CrossFadeAlpha(0, duration, false);
    }

    private void ResetStats()
    {
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

    public void Kill( BasePlayer killer)
    {
        if (gamemode == Gamemodes.Deathmatch) killer.AddScore(1);
    }

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

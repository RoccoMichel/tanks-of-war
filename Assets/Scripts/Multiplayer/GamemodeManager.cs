using TMPro;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;

public class GamemodeManager : MonoBehaviour
{
    public Gamemodes gamemode;
    public TMP_Text gamemodeInfo;
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
    }

    void Deathmatch()
    {
        timer = Mathf.Clamp(timer -= Time.deltaTime, 0, int.MaxValue);
        gamemodeInfo.text = $"[ {TimerLogic(timer)} ]";

        if (timer == 0)
        {
            // show winner'

            foreach (BasePlayer player in playerList)
            {
                player.Respawn();
                player.score = 0;
            }

            CallPlayerListUpdate();
            timer = DEATHMATCHLENGTH;
        }
    }

    void LastStanding()
    {
        int alive = 0;
        foreach (BasePlayer player in playerList) if (player.health > 0) alive++;
        gamemodeInfo.text = $"{alive} players left!";

        if (alive < 2)
        {
            // show winner

            foreach (BasePlayer player in playerList)
            {
                if (player.health > 0) player.score++;
                player.Respawn();
            }

            CallPlayerListUpdate();
        }
    }

    void Explore()
    {
        gamemodeInfo.text = "exploring alone...";
    }

    private void Update()
    {
        switch(gamemode)
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

    [PunRPC]
    void PlayerListUpdate()
    {
        playerList.Clear();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            playerList.Add(player.GetComponent<BasePlayer>());
    }

    public void Kill(BasePlayer killed, BasePlayer killer)
    {
        killed.deaths++;
        if (gamemode == Gamemodes.Deathmatch) killer.score++;
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

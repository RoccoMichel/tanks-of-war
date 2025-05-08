using TMPro;
using UnityEngine;

public class GamemodeManager : MonoBehaviour
{
    public Gamemodes gamemode;
    public TMP_Text gamemodeInfo;
    private float timer;
    private const float DEATHMATCHLENGTH = 5*60;

    public enum Gamemodes
    {
        Deathmatch,
        LastStanding,
        Explore
    }

    private void Start()
    {
        timer = DEATHMATCHLENGTH;
    }

    void Deathmatch()
    {
        timer -= Time.deltaTime;
        gamemodeInfo.text = $"[ {TimerLogic(timer)} ]";
    }

    void LastStanding()
    {
        gamemodeInfo.text = "x player(s) left!";
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

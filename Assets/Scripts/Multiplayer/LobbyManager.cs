using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public bool debug;
    public TMP_Text code;
    [SerializeField] Animator error;
    public int codeLength;

    public void Play()
    {
        if (IsValidateJoinCode()) JoinGame();
        else RandomGame();
    }

    public void RandomGame()
    {
        PhotonNetwork.JoinRandomRoom();
        if (debug) print("trying to join room...");
    }

    public void CreateGame()
    {
        if (debug) print("creating room...");

        float num = Random.Range(10000, 99999);
        RoomOptions options = new()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 5,
        };

        PhotonNetwork.CreateRoom(num.ToString(), options);
        if (debug) print($"room {num} has been created!");
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(code.text.Trim());                // NOT WORKING???
        if (debug) print($"trying to join room '{code.text}'...");
    }

    public bool IsValidateJoinCode()
    {
        //check for server connection
        if (!PhotonNetwork.IsConnected)
        {
            return false;
        }

        //check for correct Length
        char[] roomCode = code.text.ToCharArray();
        if (roomCode.Length != codeLength + 1)
        {
            return false;
        }

        //check if code is digits only
        if (!ValidateCode(roomCode))
        {
            return false;
        }

        return true;
    }

    public bool ValidateCode(char[] c)
    {
        for (int i = 0; i < c.Length - 1; i++)
        {
            if (c[i] == '0' || c[i] == '1' || c[i] == '2' || c[i] == '3' || c[i] == '4' || c[i] == '5' || c[i] == '6' || c[i] == '7' || c[i] == '8' || c[i] == '9')
                continue;
            else return false;
        }

        return true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(returnCode);
        if (debug) print($"couldn't join a room! {message}");
        error.Play("Display");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (debug) print($"couldn't join a room! {message}");
        if (debug) print("creating own Instead...");
        CreateGame();
    }

    // for tutorial button
    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }
}
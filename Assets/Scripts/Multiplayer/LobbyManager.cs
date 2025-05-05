using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject errorMessage;
    [SerializeField] TMP_Text code;
    public int codeLength;

    public void RandomGame()
    {
        PhotonNetwork.JoinRandomRoom();
        print("trying to join room...");
    }

    public void CreateGame()
    {
        print("creating room...");

        float num = Random.Range(10000, 99999);
        RoomOptions options = new()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2,
        };

        PhotonNetwork.CreateRoom($"Lobby#{num}", options);
        print($"room {num} has been created!");
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(code.text, null);
        print($"trying to join room {code.text}...");
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
        print($"couldn't join a room! {message}");
        DisplayJoinError();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print($"couldn't join a room! {message}");
        DisplayJoinError();
    }

    public void DisplayJoinError()
    {
        Destroy(Instantiate(errorMessage, Vector3.zero, Quaternion.identity, FindFirstObjectByType<Canvas>().transform), 2f);
    }
}
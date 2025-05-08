using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class ServerManager : MonoBehaviourPunCallbacks
{
    public bool debug;
    [SerializeField] Button serverButton;
    [SerializeField] TMP_InputField nameField;
    public bool validName;
    LobbyManager lobbyManager;

    void Start()
    {
        try { lobbyManager = GetComponent<LobbyManager>(); } 
        catch { Debug.LogWarning("missing lobbyManager Reference"); }
        
        PhotonNetwork.ConnectUsingSettings();
        if (debug) print("Connecting...");
    }

    public override void OnConnectedToMaster()
    {
        if (debug) print($"Connected! Server:{PhotonNetwork.CloudRegion}");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        serverButton.interactable = PhotonNetwork.IsConnected;

        //serverButtons[1].interactable = validName && lobbyManager.IsValidateJoinCode();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeName()
    {
        validName = false;
        if (string.IsNullOrWhiteSpace(nameField.text)) return;

        validName = true;
        PhotonNetwork.NickName = nameField.text;
        if (debug) print($"Changed Name to: {nameField.text}");
    }
}

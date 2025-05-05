using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class ServerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Button[] serverButtons;
    [SerializeField] TMP_InputField nameField;
    public bool validName;
    LobbyManager lobbyManager;

    void Start()
    {
        try { lobbyManager = GetComponent<LobbyManager>(); } 
        catch { Debug.LogWarning("missing lobbyManager Reference"); }
        
        PhotonNetwork.ConnectUsingSettings();
        print("Connecting...");
    }

    public override void OnConnectedToMaster()
    {
        print($"Connected! Server:{PhotonNetwork.CloudRegion}");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        serverButtons[0].interactable = validName && PhotonNetwork.IsConnected;
        serverButtons[1].interactable = validName && lobbyManager.IsValidateJoinCode();
        serverButtons[2].interactable = validName && PhotonNetwork.IsConnected;
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
        print($"Changed Name to: {nameField.text}");
    }
}

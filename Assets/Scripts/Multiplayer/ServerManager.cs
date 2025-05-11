using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class ServerManager : MonoBehaviourPunCallbacks
{
    public bool debug;
    [SerializeField] Button joinButton;
    [SerializeField] Button createButton;
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_Dropdown gamemodeSelector;
    public bool validName;
    LobbyManager lobbyManager;

    void Start()
    {
        if (joinButton != null) joinButton.interactable = false;
        if (createButton != null) createButton.interactable = false;
        try { lobbyManager = GetComponent<LobbyManager>(); } 
        catch { Debug.LogWarning("missing lobbyManager Reference"); }

        if (gamemodeSelector != null) gamemodeSelector.value = PlayerPrefs.GetInt("Preferred Gamemode", 2);
        nameField.text = PlayerPrefs.GetString("Name");

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
        if (lobbyManager.code.text.Length <= 1) joinButton.interactable = PhotonNetwork.IsConnectedAndReady;
        else if (lobbyManager.IsValidateJoinCode()) joinButton.interactable = true;
        else joinButton.interactable = false;

        createButton.interactable = PhotonNetwork.IsConnectedAndReady;
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
        PlayerPrefs.SetString("Name", nameField.text);
        PhotonNetwork.NickName = nameField.text;
        if (debug) print($"Changed Name to: {nameField.text}");
    }

    public void ChangedPreferredGamemode()
    {
        if (gamemodeSelector != null) PlayerPrefs.SetInt("Preferred Gamemode", gamemodeSelector.value);
    }
}

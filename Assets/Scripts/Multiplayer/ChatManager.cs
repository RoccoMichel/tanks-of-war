using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [Header("Attributes")]
    public bool inChat;
    public int fontSize;
    public float chatLifeSeconds;

    [Header("References")]
    [SerializeField] protected TMP_Text recent;
    [SerializeField] protected TMP_Text full;
    [SerializeField] protected TMP_InputField chatInput;
    [SerializeField] protected Component[] fullChatFeatures;

    [HideInInspector] public List<string> messages = new();
    public List<float> timers = new();

    internal PhotonView view;
    internal InputAction enterChatAction;
    internal InputAction cancelAction;

    private void Start()
    {
        enterChatAction = InputSystem.actions.FindAction("EnterChat");
        cancelAction = InputSystem.actions.FindAction("Cancel");
        view = GetComponent<PhotonView>();

        // make sure chat starts in recent view mode
        inChat = false;
        TogglChatText(false);
    }

    private void Update()
    {
        // Reduce timers for all, remove element if it expired
        for (int i = 0;  i < timers.Count; i++)
        {
            timers[i] -= Time.deltaTime;

            if (timers[i] > 0) continue;

            timers.RemoveAt(i);
            messages.RemoveAt(i);
            RefreshRecentChat();
        }

        // Toggle which type of Chat is displayed
        if (enterChatAction.WasPressedThisFrame() || (inChat && cancelAction.WasPressedThisFrame()))
            ToggleChat();
    }

    [PunRPC]
    private void ReceiveMessage(string username, string message)
    {
        if (string.IsNullOrWhiteSpace(username)) username = "ANONYMOUS";

        messages.Add($"[{username}] {message}");
        timers.Add(chatLifeSeconds);
        RefreshRecentChat();

        full.text = $"{full.text}[{username}] {message}\n\n";
    }

    /// <summary>
    /// Send a message to all clients
    /// </summary>
    /// <param name="sender">Username</param>
    public void SendChatMessage(string sender, string message)
    {
        if (!PhotonNetwork.InRoom) return;
        view.RPC(nameof(ReceiveMessage), RpcTarget.All, sender, message);
    }

    /// <summary>
    /// Rewrite every stored message to the recent chat window
    /// </summary>
    public virtual void RefreshRecentChat()
    {
        recent.text = string.Empty;

        for (int i = 0; i < messages.Count; i++)
        {
            recent.text = $"{recent.text}{messages[i]}\n\n";
        }
    }

    /// <summary>
    /// Toggle the visibly of the chat on the canvas
    /// </summary>
    public void ToggleChat()
    {
        inChat = !inChat;
        TogglChatText(inChat);

        if (inChat) chatInput.ActivateInputField();
    }

    /// <summary>
    /// Change which Chat is displayed to the Player by a boolean
    /// </summary>
    /// <param name="b">True indicates being in the full chat and False being in recent mode</param>
    public virtual void TogglChatText(bool b)
    {
        full.fontSize = b ? fontSize : 0;
        recent.fontSize = b ? 0 : fontSize;

        chatInput.interactable = b;
        foreach (Component component in fullChatFeatures)
        {
            if (component is Behaviour behaviour) behaviour.enabled = b;
        }
    }
}
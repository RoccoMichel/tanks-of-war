using TMPro;
using Photon.Pun;
using UnityEngine;

public class ChatSender : MonoBehaviour
{
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private TMP_InputField message;

    // For Unity UI elements
    public void SendMessage()
    {
        // Don't send an empty message
        if (string.IsNullOrWhiteSpace(message.text)) return;

        // Send Message to Chat Manager & reset input field
        if (chatManager.enterChatAction.WasPressedThisFrame())
        {
            chatManager.SendChatMessage(PhotonNetwork.NickName, message.text);
            message.text = string.Empty;
        }
    }
}
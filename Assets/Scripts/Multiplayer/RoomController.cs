using Photon.Pun;
using UnityEngine;


public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] string[] sceneName;
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(sceneName[Random.Range(0, sceneName.Length)]);
        }
    }
}

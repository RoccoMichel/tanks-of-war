using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public Transform[] spawnLocations;
    int chosenLocation;

    public void Start()
    {
        chosenLocation = Random.Range(0, spawnLocations.Length);
        Invoke(nameof(SpawnPlayer), 0.5f);
    }

    public void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnected) return;

        FindAnyObjectByType<ChatManager>().SendChatMessage(PhotonNetwork.NickName, "has joined.");
        PhotonNetwork.Instantiate(PlayerPrefs.GetString("selectedTank"), spawnLocations[chosenLocation].position, spawnLocations[chosenLocation].rotation);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GamemodeManager>().CallPlayerListUpdate();
    }

    public void SetPosition(Transform player)
    {
        chosenLocation = Random.Range(0, spawnLocations.Length);
        player.SetPositionAndRotation(spawnLocations[chosenLocation].position, spawnLocations[chosenLocation].rotation);
    }
}
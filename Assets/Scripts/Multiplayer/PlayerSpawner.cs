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

        if (PhotonNetwork.IsMasterClient) FindAnyObjectByType<GamemodeManager>().gamemode = (GamemodeManager.Gamemodes)PlayerPrefs.GetInt("Preferred Gamemode", 2);
    }

    public void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnected) return;

        PhotonNetwork.Instantiate(PlayerPrefs.GetString("selectedTank"), spawnLocations[chosenLocation].position, spawnLocations[chosenLocation].rotation);
    }
}
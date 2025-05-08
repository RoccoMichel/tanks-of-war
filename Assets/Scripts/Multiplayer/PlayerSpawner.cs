using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnLocations;
    int choosenLocation;

    public void Start()
    {
        choosenLocation = Random.Range(0, spawnLocations.Length);
        Invoke(nameof(SpawnPlayer), 0.5f);

        if (PhotonNetwork.IsMasterClient) FindAnyObjectByType<GamemodeManager>().gamemode = (GamemodeManager.Gamemodes)PlayerPrefs.GetInt("Prefered Gamemode", 2);
    }

    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(PlayerPrefs.GetString("selectedTank"), spawnLocations[choosenLocation].position, spawnLocations[choosenLocation].rotation);
    }
}
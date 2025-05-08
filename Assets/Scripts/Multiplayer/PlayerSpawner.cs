using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnLocations;

    public void Start()
    {
        int choosenLocation = Random.Range(0, spawnLocations.Length);
        PhotonNetwork.Instantiate(PlayerPrefs.GetString("selectedTank"), spawnLocations[choosenLocation].position, spawnLocations[choosenLocation].rotation);
    }
}
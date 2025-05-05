using UnityEngine;
using Photon.Pun;

public class PlayerCreator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    protected DeathmatchManager manager;
    public string[] charactersNames;
    public GameObject characterSelector;

    public void CreatePlayer(int index)
    {
        Camera.main.gameObject.SetActive(false);
        characterSelector.SetActive(false);
        PhotonNetwork.Instantiate(charactersNames[index], manager.worldSpawnPoints[Random.Range(0, manager.worldSpawnPoints.Length)], Quaternion.identity);
    }
}
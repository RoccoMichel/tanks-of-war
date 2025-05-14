using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class ItemSpawning : MonoBehaviour
{
    public bool photonInstantiate;
    public float frequencySeconds = 5;
    public int attempts = 4;
    public Transform[] locations;
    public GameObject[] items;
    private GameObject[] itemCheck;
    private PhotonView view;
    private float timer;

    private void Start()
    {
        if (locations.Length != 0 && locations != null) itemCheck = new GameObject[locations.Length];
        view = GetComponent<PhotonView>();

        if (!PhotonNetwork.IsMasterClient) view.RPC(nameof(RequestInfo), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        timer += Time.deltaTime;
        if (timer >= frequencySeconds) SpawnItem();
    }

    public void SpawnItem()
    {
        timer = 0;
        int index = 0;

        for (int i = 0; i < attempts; i++)
        {
            index = Random.Range(0, locations.Length);

            if (itemCheck[index] == null) break;

            if (i == attempts - 1) return;
        }

        int type = Random.Range(0, items.Length);

        if (photonInstantiate) itemCheck[index] = PhotonNetwork.Instantiate(items[type].name, locations[index].position, Quaternion.Euler(new(0, 0, Random.Range(0, 360))));
        else view.RPC(nameof(InstantiateNewItem), RpcTarget.All, index, type, index);
    }

    [PunRPC]
    private void InstantiateNewItem(int slotIndex, int typeIndex, int locationIndex)
    {
        Vector3 rotation = new(0, 0, Random.Range(0, 360));
        itemCheck[slotIndex] = Instantiate(items[typeIndex], locations[locationIndex].position, Quaternion.Euler(rotation));
    }

    protected void RequestInfo(int actorNumber)
    {
        List<int> filledLocations = new();
        List<int> itemOnLocation = new();

        for (int i = 0; i < itemCheck.Length; i++)
        {
            if (itemCheck[i] == null) continue;

            filledLocations.Add(i);

            for (int j = 0; j < items.Length; j++)
            {
                if (items[i] == itemCheck[i]) itemOnLocation.Add(j);
            }
        }

        view.RPC(nameof(ReceiveInfo), PhotonNetwork.CurrentRoom.GetPlayer(actorNumber), filledLocations.ToArray(), itemOnLocation.ToArray());
    }

    [PunRPC]
    protected void ReceiveInfo(int[] location, int[] item)
    {
        for (int i = 0; i < location.Length; i++) 
        {
            InstantiateNewItem(location[i], item[i], location[i]);
        }
    }
}
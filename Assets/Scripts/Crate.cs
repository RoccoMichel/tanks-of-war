using Photon.Pun;
using UnityEngine;

public class Crate : Destructible
{
    [Header("Crate Attributes")]
    public Vector2 randomAmount = Vector2.one;
    public float spawnRadius = 0.5f;
    public bool oneType = true;
    public GameObject[] contents;
    private PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public override void Die()
    {
        if (PhotonNetwork.IsMasterClient && view != null)
        {
            int amount = Random.Range((int)randomAmount.x, (int)randomAmount.y + 1);
            int content = Random.Range(0, contents.Length);

            for (int i = 0; i < amount; i++)
            {
                Vector2 position = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), transform.position.z);
                float rotation = Random.Range(0, 360);

                if (!oneType) content = Random.Range(0, contents.Length);

                view.RPC(nameof(SpawnContents), RpcTarget.All, content, position.x, position.y, rotation);
            }
        }

        PlayAllEffect();
        Destroy(gameObject);
    }

    [PunRPC]
    public void SpawnContents(int content, float PosX, float PosY, float RotZ)
    {
        Instantiate(contents[content], new (PosX, PosY), Quaternion.Euler(new (0, 0, RotZ)));
    }
}
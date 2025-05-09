using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Vector2 offset;
    public Transform target;

    private void Update()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
            {
                target = player.transform;
                break;
            }
        }

        gameObject.SetActive(!target.IsUnityNull());
        if (target != null) transform.position = new Vector2(target.position.x + offset.x, target.position.y + offset.y);
    }
}

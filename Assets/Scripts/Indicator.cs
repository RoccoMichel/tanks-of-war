using Photon.Pun;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Vector2 offset;
    public Transform target;

    private void Update()
    {
        if (target == null)
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
        }
        else transform.position = new Vector2(target.position.x + offset.x, target.position.y + offset.y);
    }
}

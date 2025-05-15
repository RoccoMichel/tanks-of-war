using Photon.Pun;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Vector2 offset;
    public Transform target;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

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

        if (target == null) return;

        sprite.enabled = target.gameObject.activeInHierarchy;
        transform.position = new Vector2(target.position.x + offset.x, target.position.y + offset.y);
    }
}

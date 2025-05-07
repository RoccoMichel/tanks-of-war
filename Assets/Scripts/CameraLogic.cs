using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    // try to keep all tanks in frame but focus on local player

    public float maxDistance = 15f;
    public float minDistance = 5f;
    Vector2 targetPosition;

    private void Update()
    {

        foreach (GameObject playerPosition in GameObject.FindGameObjectsWithTag("Player"))
        {
            //playerPosition.transform.position = targetPosition;
        }
    }
}
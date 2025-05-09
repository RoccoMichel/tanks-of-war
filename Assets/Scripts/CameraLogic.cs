using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraLogic : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 minBounds;
    public Vector2 maxBounds;
    public bool clamp = true;
    private float camHalfHeight;
    private float camHalfWidth;

    [Header("Zooming")]
    public float zoomSpeed = 4f;
    public float minZoom = 3f;
    public float maxZoom = 50f;
    public float zoomLimiter = 130f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // Assigning
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return;

        // Positioning
        Vector3 centerPoint = GetCenterPoint(players);
        Vector3 newPosition = new(centerPoint.x, centerPoint.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed * Time.deltaTime);

        // Zooming
        float targetDistance = GetGreatestDistance(players);
        float newZoom = Mathf.Lerp(minZoom, maxZoom, targetDistance / zoomLimiter);
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime * zoomSpeed);

        // Clamping
        if (!clamp) return;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.orthographicSize * cam.aspect;

        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

        transform.position = clampedPos;
    }

    Vector3 GetCenterPoint(GameObject[] objects)
    {
        if (objects.Length == 1) return objects[0].transform.position;

        Bounds bounds = new(objects[0].transform.position, Vector3.zero);
        foreach (GameObject obj in objects)
            bounds.Encapsulate(obj.transform.position);

        return bounds.center;
    }
    float GetGreatestDistance(GameObject[] objects)
    {
        Bounds bounds = new(objects[0].transform.position, Vector3.zero);
        foreach (GameObject obj in objects)
            bounds.Encapsulate(obj.transform.position);

        return Mathf.Max(bounds.size.x, bounds.size.y);
    }
}
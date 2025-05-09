using UnityEngine;

public class ItemSpawning : MonoBehaviour
{
    public GameObject[] items;
    public float frequencySeconds;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frequencySeconds) SpawnItem();
    }

    public void SpawnItem()
    {
        timer = 0;

        Vector2 spawnLocation = new(Random.Range(-10, 10), Random.Range(-10, 10));
        Vector3 rotation = new(0, 0, Random.Range(0, 360));

        Instantiate(items[Random.Range(0, items.Length)], spawnLocation, Quaternion.Euler(rotation));
    }
}

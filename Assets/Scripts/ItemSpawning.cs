using UnityEngine;

public class ItemSpawning : MonoBehaviour
{
    public float frequencySeconds = 5;
    public int attempts = 4;
    public Transform[] locations;
    public GameObject[] items;
    public GameObject[] itemCheck;
    private float timer;

    private void Start()
    {
        if (locations.Length != 0 && locations != null) itemCheck = new GameObject[locations.Length];
    }

    private void Update()
    {
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

        Vector3 rotation = new(0, 0, Random.Range(0, 360));

        itemCheck[index] = Instantiate(items[Random.Range(0, items.Length)], locations[index].position, Quaternion.Euler(rotation));
    }
}

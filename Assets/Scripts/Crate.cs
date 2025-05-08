using UnityEngine;

public class Crate : Destructable
{
    [Header("Crate Attributes")]
    public Vector2 randomAmount = Vector2.one;
    public float spawnRadius = 0.5f;
    public bool oneType = true;
    public GameObject[] contents;

    public override void Die()
    {
        int amount = Mathf.CeilToInt(Random.Range(Mathf.Abs(randomAmount.x), Mathf.Abs(randomAmount.y)));
        GameObject content = oneType ? contents[Random.Range(0, contents.Length)] : null;

        for (int i = 0; i < amount; i++)
        {
            Vector2 position = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), transform.position.z);
            Vector3 rotaion = new Vector3(0, 0, Random.Range(0, 360));

            if (!oneType) content = contents[Random.Range(0, contents.Length)];

            Instantiate(content, position, Quaternion.Euler(rotaion));
        }

        base.Die();
    }
}

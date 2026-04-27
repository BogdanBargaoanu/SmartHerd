using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject sheepPrefab;
    public GameObject wolfPrefab;

    public int startingSheep = 50;
    public int startingWolves = 2;
    public float spawnRadius = 20f;

    void Start()
    {
        for (int i = 0; i < startingSheep; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-spawnRadius, spawnRadius), 1f, Random.Range(-spawnRadius, spawnRadius));
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Instantiate(sheepPrefab, randomPos, randomRot);
        }

        for (int i = 0; i < startingWolves; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-spawnRadius, spawnRadius), 1f, Random.Range(-spawnRadius, spawnRadius));
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Instantiate(wolfPrefab, randomPos, randomRot);
        }
    }
}
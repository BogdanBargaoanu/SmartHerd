using Assets.Scripts.AI.FSM;
using Assets.Scripts.AI.Utility;
using Assets.Scripts.Wolf;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject sheepPrefab;
    public GameObject wolfPrefab;

    [Header("Settings")]
    public int startingSheep = 50;
    public int startingWolves = 2;
    public float spawnRadius = 20f;

    private List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        SpawnAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSimulation();
        }
    }

    void ResetSimulation()
    {
        foreach (var obj in spawned)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawned.Clear();
        SpawnAll();
    }

    void SpawnAll()
    {
        for (int i = 0; i < startingSheep; i++)
        {
            Spawn(sheepPrefab);
        }

        for (int i = 0; i < startingWolves; i++)
        {
            GameObject wolfObj = Spawn(wolfPrefab);

            // 🔥 IMPORTANT: reset wolf AI state
            ResetWolf(wolfObj);
        }
    }

    GameObject Spawn(GameObject prefab)
    {
        Vector3 pos = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            1f,
            Random.Range(-spawnRadius, spawnRadius)
        );

        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        GameObject obj = Instantiate(prefab, pos, rot);
        spawned.Add(obj);

        return obj;
    }

    void ResetWolf(GameObject wolfObj)
    {
        Wolf wolf = wolfObj.GetComponent<Wolf>();

        if (wolf == null) return;

        // reset movement memory
        var velocityField = typeof(Wolf)
            .GetField("velocity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var wanderField = typeof(Wolf)
            .GetField("wanderDirection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var timerField = typeof(Wolf)
            .GetField("wanderTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (velocityField != null) velocityField.SetValue(wolf, Vector3.zero);
        if (wanderField != null) wanderField.SetValue(wolf, Vector3.zero);
        if (timerField != null) timerField.SetValue(wolf, 0f);

        // reset AI state
        wolf.currentState = WolfState.Idle;

        // reset utility AI values
        var ai = wolf.GetComponent<WolfUtilityAI>();
        if (ai != null)
        {
            ai.hunger = 0f;
            ai.stamina = 100f;
        }
    }
}
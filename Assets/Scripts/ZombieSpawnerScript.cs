using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnerScript : MonoBehaviour
{
    [SerializeField] GameObject zombiePrefab = null;

    [Header("Settings")]
    [SerializeField] bool START = false;
    [SerializeField] float spawnRate = 1f, nextTimeSpawn = 0f;

    private void Awake()
    {
        nextTimeSpawn = 1f / spawnRate;
    }

    private void Update()
    {
        if (!START)
            return;
        if(nextTimeSpawn <= 0)
        {
            SpawnZombie();
            nextTimeSpawn = 1f / spawnRate;
        }
        else
            nextTimeSpawn -= Time.deltaTime;
        
    }

    private void SpawnZombie()
    {
        if (zombiePrefab)
        { 
            GameObject newZm = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
            newZm.transform.SetParent(GameObject.Find("Environment").transform);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
   public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota;
        public float spawnInterval;
        public int spawnedCount;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnedCount;
        public GameObject enemyPrefab;
    }
    public List<Wave> waves;
    public int currentWaveCount;

    [Header("Spawner Attributes")]
    float spawnTimer;
    public int enemiesAlive;
    public int enemiesKilled;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached = false;
    public float waveInterval;
    bool isWaveActive = false;

    [Header("Spawn Points")]
    public List<Transform> relativeSpawnPoints;

    Transform player;
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalcWaveQuota();
    }

    private void Update()
    {

        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnedCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }
        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn the next enemy
        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            SpawnEnemies();
            spawnTimer = 0f;
        }

    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        yield return new WaitForSeconds(waveInterval);

        if (currentWaveCount < waves.Count -1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalcWaveQuota();
        }
    }

    void CalcWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
    }

    void SpawnEnemies()
    {
        // Check if enemies spawned is less than waveQuota
        if (waves[currentWaveCount].spawnedCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            // Spawn enemy groups (and iterate through groups)
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {

                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnedCount++;
                    waves[currentWaveCount].spawnedCount++;
                    enemiesAlive++;

                //Check if enemy of current type has spawned enough
                if (enemyGroup.spawnedCount < enemyGroup.enemyCount)
                {

                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                }
            }
        }


    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}

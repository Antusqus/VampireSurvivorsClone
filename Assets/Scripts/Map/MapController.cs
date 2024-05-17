using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public PlayerStats player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;

    Vector3 playerLastPosition;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist; // Must be greater than the length and width of the tilemap
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDuration;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerLastPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChunkSpawnChecker();
        ChunkOptimizer();
    }

    void ChunkSpawnChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        int x = moveDir.x == 0 ? 0 : moveDir.x > 0 ? 20 : -20;
        int y = moveDir.y == 0 ? 0 : moveDir.y > 0 ? 20 : -20;

        if (x != 0 && y != 0)
        {
            SpawnDiagonals(x, y);
        }
        else
        {
                SpawnChunk(currentChunk.transform.position + new Vector3(x, y, 0));
        }


    }

    //bool ChunkExists(Vector3 spawnPosition)
    //{
    //    return (Physics2D.OverlapCircle(spawnPosition, checkerRadius, terrainMask));
    //}

    void SpawnChunk(Vector3 spawnPosition)
    {
        if (!Physics2D.OverlapCircle(spawnPosition, checkerRadius, terrainMask))
        {
            int rand = Random.Range(0, terrainChunks.Count);
            latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
            spawnedChunks.Add(latestChunk);
        }


    }

    void SpawnDiagonals(int x, int y)
    {
        SpawnChunk(currentChunk.transform.position + new Vector3(x, 0, 0));
        SpawnChunk(currentChunk.transform.position + new Vector3(0, y, 0));
        SpawnChunk(currentChunk.transform.position + new Vector3(x, y, 0));
    }



    void ChunkOptimizer()
    {

        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDuration;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Biome
{
    public bool smoothened;
    public bool scanned;
    public BiomeManager manager;
    public int maxChunkCount;
    public GameObject biomeObject;
    public Dictionary<Vector2, TerrainChunk> terrainChunksInBiomeDict = new Dictionary<Vector2, TerrainChunk>();

    public BiomePreset preset;
    System.Random prng = new System.Random();


    public float HeatAvg;
    public float HeightAvg;

    public float MoistAvg;

    public bool scannerCoroutineRunning = false;


    public Biome(BiomeManager _manager, int _chunks = 0, string name = null)
    {
        smoothened = false;
        scanned = false;
        this.manager = _manager;
        List<BiomePreset> presets = _manager.presets;
        System.Random rnd = new System.Random();
        preset = presets[rnd.Next(presets.Count)];
        biomeObject = new GameObject((name == null || name == "") ? preset.biomeType.ToString() : name);

        if (_chunks == 0)
        {
            maxChunkCount = prng.Next(((int)TerrainGenerator.viewerMoveThresholdForChunkUpdate - 8), (int)TerrainGenerator.viewerMoveThresholdForChunkUpdate + 8);
        }
        else
        {
            maxChunkCount = _chunks;
        }
    }
    public BiomeType GetBiome(float temp, float humid)
    {
        BiomeType t = TerrainGenerator.BiomeTable[(int)temp, (int)humid];

        return t;

    }

    





}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Biome
{
    public int maxChunkCount;
    public GameObject biomeObject;
    public Dictionary<Vector2, TerrainChunk> terrainChunksInBiomeDict = new Dictionary<Vector2, TerrainChunk>();

    public BiomePreset preset;
    System.Random prng = new System.Random();

    public Biome(BiomeManager manager, int _chunks = 0, string name = null)
    {
        biomeObject = new GameObject((name == null || name == "") ? this.GetType().Name : name);
        List<BiomePreset> presets = manager.presets;
        System.Random rnd = new System.Random();
        preset = presets[rnd.Next(presets.Count)];

        if (_chunks == 0)
        {
            maxChunkCount = prng.Next(((int)TerrainGenerator.viewerMoveThresholdForChunkUpdate - 8), (int)TerrainGenerator.viewerMoveThresholdForChunkUpdate + 8);
        }
        else
        {
            maxChunkCount = _chunks;
        }
    }
    public static int GetBiome(float temp, float humid)
    {
        int temperature = (int)(temp);
        int humidity = (int)(humid);
        //Debug.Log(temp);
        int biome = (int)TerrainGenerator.BiomeTable[temperature,humidity];

        return biome;
    }




}


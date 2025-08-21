using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    [SerializeField]
    public List<BiomePreset> presets = new List<BiomePreset>();

    public List<Biome> biomes = new List<Biome>();
    public List<string> biomeNames = new List<string>();

    public List<Vector2> borderChunks = new List<Vector2>();
    public TerrainGenerator terrainGen;
    public List<TerrainChunk[,]> toBlendChunks = new List<TerrainChunk[,]>();

    private void Awake()
    {
        terrainGen = FindObjectOfType<TerrainGenerator>();
    }
    public void AddSpawnedBiome(Biome biome)
    {
        biomeNames.Add(biome.biomeObject.name);
        Dictionary<string, int> biomeCounter = CountOccurrences(biomeNames);
        biome.biomeObject.name = biome.biomeObject.ToString() + biomeCounter[biome.biomeObject.name];

        biomes.Add(biome);


    }

    static Dictionary<T, int> CountOccurrences<T>(List<T> list)
    {
        var counts = new Dictionary<T, int>();

        foreach (var item in list)
        {
            if (counts.ContainsKey(item))
            {
                counts[item]++;
            }
            else
            {
                counts[item] = 1;
            }
        }

        return counts;
    }



    public IEnumerator BlendBiomes()
    {
        yield return new WaitForSeconds(1f);
        SmoothChunks();
        //yield return BlendChunks();

    }
    public void SmoothChunks()
    {
        foreach (Biome biome in biomes)
        {
            if (!biome.smoothened)
            {
                foreach (TerrainChunk tc in biome.terrainChunksInBiomeDict.Values)
                {
                    if (!tc.heightMapReceived) return;
                    biome.HeatAvg += tc.mapGen.heatAvg;
                    biome.HeightAvg += tc.mapGen.heightAvg;
                    biome.MoistAvg += tc.mapGen.moistAvg;


                }

                biome.HeatAvg = biome.HeatAvg / biome.terrainChunksInBiomeDict.Count;
                biome.HeightAvg = biome.HeightAvg / biome.terrainChunksInBiomeDict.Count;
                biome.MoistAvg = biome.MoistAvg / biome.terrainChunksInBiomeDict.Count;

                //foreach (TerrainChunk tc in biome.terrainChunksInBiomeDict.Values)
                //{
                //    foreach (Tile t in tc.noiseMaps.tiles)
                //    {
                //        t.HeatValue = Mathf.Lerp(tc.mapGen.heatTotal, t.HeatValue, biome.HeatAvg);
                //        t.MoistureValue = Mathf.Lerp(tc.mapGen.moistTotal, t.MoistureValue, biome.MoistAvg);
                //        t.HeightValue = Mathf.Lerp(tc.mapGen.heightTotal, t.HeightValue, biome.HeightAvg);


                //    }
                //}

                biome.smoothened = true;
                //Debug.Log("Smoothened Biome:" + biome.biomeObject.name);
            }
        }

    }


    public void LoadTiles(TerrainChunk chunk)
    {

        float heatTotal = 0;
        float moistTotal = 0;
        float heightTotal = 0;
        for (int row = 0; row < 2; row++)
        {
            heatTotal = 0;
            moistTotal = 0;
            heightTotal = 0;

            for (int col = 0; col < 2; col++)
            {
                heatTotal += chunk.mapGen.heatMap.values[row, col];
                moistTotal += chunk.mapGen.moistureMap.values[row, col];
                heightTotal += chunk.mapGen.heightMap.values[row, col];
            }

        }

        float heatAvg = heatTotal / 2;
        if (heatAvg > 5)
            heatAvg = 5;

        float moistAvg = moistTotal / 2;
        if (moistAvg > 5)
            moistAvg = 5;

        float heightAvg = heightTotal / 2;
        if (heightAvg > 5)
            heightAvg = 5;

        foreach (Tile t in chunk.mapGen.tiles)
        {
            t.HeatValue = Mathf.Lerp(t.HeatValue, heatTotal, t.distFromEdge);
            t.MoistureValue = Mathf.Lerp(t.MoistureValue, moistTotal, t.distFromEdge);
            t.HeightValue = Mathf.Lerp(t.HeightValue, heightTotal, t.distFromEdge);
        }
    }

    float GetBlendDirection(Vector2 offset, TerrainChunk chunk, Tile t)
    {
        float dir = 1000;

        if (Vector2.up == offset)
        {
            dir = chunk.meshSettings.numVertsPerLine - 1 - t.Y;

        }

        if (Vector2.down == offset)
        {
            dir = t.Y;

        }
        if (Vector2.left == offset)
        {
            dir = t.X;

        }

        if (Vector2.right == offset)
        {
            dir = chunk.meshSettings.numVertsPerLine - 1 - t.X;

        }

        return dir;
    }


    Tile GetTileGlobal(int globalX, int globalY)
    {
        int chunkSize = (int)terrainGen.meshSettings.meshWorldSize;

        int chunkX = Mathf.FloorToInt((float)globalX / chunkSize);
        int chunkY = Mathf.FloorToInt((float)globalY / chunkSize);

        int localX = globalX - (chunkX * chunkSize);
        int localY = globalY - (chunkY * chunkSize);

        Vector2Int chunkCoord = new Vector2Int(chunkX, chunkY);

        if (terrainGen.TryGetChunk(chunkCoord, out TerrainChunk chunk))
        {
            return chunk.mapGen.tiles[localX, localY];
        }

        return null; // Chunk isn't loaded or out of range
    }

    Tile GetTileFromChunks(Dictionary<Vector2, TerrainChunk> allChunks, Vector2 currentChunkCoord, int localX, int localY, int chunkSize)
    {
        int x = localX;
        int y = localY;
        Vector2 chunkOffset = currentChunkCoord;

        if (x < 0)
        {
            chunkOffset.x -= 1;
            x += chunkSize;
        }
        else if (x >= chunkSize)
        {
            chunkOffset.x += 1;
            x -= chunkSize;
        }

        if (y < 0)
        {
            chunkOffset.y -= 1;
            y += chunkSize;
        }
        else if (y >= chunkSize)
        {
            chunkOffset.y += 1;
            y -= chunkSize;
        }

        if (allChunks.TryGetValue(chunkOffset, out TerrainChunk neighborChunk))
            return neighborChunk.mapGen.tiles[x, y];

        return null; // Or fallback tile
    }

    void ApplyBiomeBlurWithNeighbors(TerrainChunk chunk,
    Dictionary<Vector2, TerrainChunk> allChunks,
    int chunkSize,
    int radius,
    float sigma)
    {
        float[,] kernel = chunk.mapGen.GenerateGaussianKernel(radius, sigma);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Dictionary<BiomeType, float> biomeWeights = new Dictionary<BiomeType, float>();

                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        int sampleX = x + dx;
                        int sampleY = y + dy;
                        float weight = kernel[dx + radius, dy + radius];

                        Tile neighborTile = GetTileGlobal(sampleX, sampleY);
                        if (neighborTile == null) continue;
                        Debug.Log("Potato");

                        BiomeType biome = neighborTile.BiomeType;
                        if (!biomeWeights.ContainsKey(biome))
                            biomeWeights[biome] = 0f;
                        biomeWeights[biome] += weight;
                    }
                }

                BiomeType dominantBiome = biomeWeights.OrderByDescending(kvp => kvp.Value).First().Key;


                chunk.mapGen.tiles[x, y].BiomeType = dominantBiome;
            }
        }
    }
    public IEnumerator GetBiomeBorders(Biome biome)
    {
        //Todo: GetBiomeBorders efficiently, but also fully. We aren't getting every border correctly.
        biome.scannerCoroutineRunning = true;
        yield return new WaitForSeconds(.5f);

        //ConcurrentDictionary<Vector2, BiomeType> tempKvp = TerrainGenerator.terrainChunkBiomeDict;
        if (biome.maxChunkCount == biome.terrainChunksInBiomeDict.Count - 1)
        {
            biome.scannerCoroutineRunning = false;
            yield break;
        }

        foreach (TerrainChunk chunk in biome.terrainChunksInBiomeDict.Values)
        {

            foreach (Vector2 offset in TerrainGenerator.neighborOffsets)
            {
                Vector2 neighborCoord = chunk.coord + offset;

                if (terrainGen.terrainChunkDict.TryGetValue(neighborCoord, out TerrainChunk neighbourChunk))
                {
                    if (neighbourChunk.biome.preset != chunk.biome.preset)
                    {
                        chunk.isBorderChunk = true;
                        neighbourChunk.isBorderChunk = true;

                        var color = chunk.meshRenderer.material.color;
                        color.r += 1f; // Mark chunkborders as red
                        chunk.meshRenderer.material.color = color;

                    }
                    else
                        continue;

                }



            }

        }
        biome.scannerCoroutineRunning = false;

    }

    public IEnumerator BlendBiomeBorders(Biome biome)
    {
        yield return new WaitForSeconds(.5f);

        foreach (TerrainChunk chunk in biome.terrainChunksInBiomeDict.Values.Where(n => n.isBorderChunk))
        {

            if (chunk.hasBlended)
                continue;

            float blendBorderSize = chunk.noiseMaps.tiles.GetLength(0) / 4; // how many tiles into chunk you want to blend

            ApplyBiomeBlurWithNeighbors(chunk, terrainGen.terrainChunkDict, (int)chunk.meshSettings.meshWorldSize, (int)blendBorderSize, 1.0f);
            chunk.ApplyTexture();

            chunk.hasBlended = true;
            LoadTiles(chunk);
        }

    }

    public IEnumerator BlendChunkWithNeighbours(TerrainChunk chunk)
    {
        if (chunk.hasBlended)
            yield break;

        yield return new WaitForSeconds(.5f);



        float blendBorderSize = chunk.noiseMaps.tiles.GetLength(0) / 8; // how many tiles into chunk you want to blend

        ApplyBiomeBlurWithNeighbors(chunk, terrainGen.terrainChunkDict, (int)chunk.meshSettings.meshWorldSize, (int)blendBorderSize, 2.0f);
        chunk.ApplyTexture();

        chunk.hasBlended = true;
        LoadTiles(chunk);

    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public enum NormalizeMode { Local, Global };
    public static MapData GenerateNoiseMapData(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre)
    {



        MapData noiseMap = new MapData(mapWidth, mapHeight);

        System.Random prng = new System.Random(settings.seed);

        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0f;
        float amplitude = 1;
        float frequency = 1;

        int xOffset;
        int yOffset;
        xOffset = (int)sampleCentre.x;
        yOffset = (int)sampleCentre.y;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        //Half values used to "zoom" map into center, instead of topright
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;


                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }







                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;

                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }




                noiseMap.data[x, y] = noiseHeight;





                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedNoiseHeight = (noiseMap.data[x, y] + 1) / (maxPossibleHeight / 0.9f);



                    noiseMap.data[x, y] = Mathf.Clamp(normalizedNoiseHeight, 0, int.MaxValue);
                }



            }
        }


        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // Todo: Check min max heights for all data types
                    noiseMap.data[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap.data[x, y]);


                }

            }
        }


        return noiseMap;

    }

    
}



[System.Serializable]
public class NoiseSettings
{

    [Header("Noise Settings")]

    public Noise.NormalizeMode normalizeMode;
    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = .5f;
    public float lacunarity = 2;
    public int seed;
    public Vector2 offset;
    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        if (scale > 1000)
            scale = 1000;
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}
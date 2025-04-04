using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public static float ElevationAmplitude = 180;
    public static float MinElevation = -.292893219f;
    public static float MaxElevation = .224744871f;
    public static int MountainMapScale = 200;
    public static float ElevationMapScale = 2300;
    public static int TemperatureMapScale = 450;
    public static int HumidityMapScale = 300;


    public enum NormalizeMode { Local, Global };
    public static (MapData, MapData, MapData) GenerateNoiseMapData(Vector2 coord, int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre, Material material)
    {



        MapData heightMap = new MapData(mapWidth, mapHeight);
        MapData MoistureData = new MapData(mapWidth, mapHeight);
        MapData HeatData = new MapData(mapWidth, mapHeight);

        System.Random prng = new System.Random(settings.seed);

        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0f;
        float amplitude = 1;
        float frequency = 1;

        int xOffset;
        int yOffset;
        xOffset = (int)coord.x * mapWidth;
        yOffset = (int)coord.y * mapHeight;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-50000, 50000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-50000, 50000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float maxLocalMoistureHeight = float.MinValue;
        float minLocalMoistureHeight = float.MaxValue;

        float maxLocalHeatHeight = float.MinValue;
        float minLocalHeatHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;



        float temperatureValue, humidityValue, elevationValue, mountainValue, freshWaterValue, wetnessValue;
        float elevationValueSmooth;
        int biomeValue;

        float tMod, rough;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                float e = Mathf.PerlinNoise((x + xOffset - settings.seed + .01f) / ElevationMapScale, (y + yOffset - settings.seed + .01f) / ElevationMapScale);
                elevationValue = elevationValueSmooth = Mathf.Pow(e + .5f, .5f) - 1f;


                rough = Mathf.Pow(Mathf.PerlinNoise((x + xOffset + .01f) / 30f, (y + yOffset + .01f) / 30f) + .5f, .1f) - 1f;
                elevationValue += rough;

                mountainValue = Mathf.PerlinNoise((x + xOffset - settings.seed + .01f) / ElevationMapScale, (y + yOffset - settings.seed + .01f) / ElevationMapScale);
                mountainValue = Mathf.Pow(mountainValue, 2f);

                temperatureValue = 1.3f - (e);
                rough = Mathf.Pow(Mathf.PerlinNoise((x + xOffset + + .01f) / 50f, (y + yOffset + .01f) / 50f) + .5f, .1f) - 1f;
                temperatureValue += rough;

                float latitudeMod;
                latitudeMod = ((Mathf.PerlinNoise((x + xOffset + .01f) / TemperatureMapScale, (y + yOffset + .01f) / TemperatureMapScale) + .5f) - 1f) * 1.5f;


                temperatureValue += latitudeMod;
                temperatureValue = Mathf.Clamp01(temperatureValue);
                temperatureValue = Mathf.InverseLerp(0f, 1f, temperatureValue);


                humidityValue = Mathf.PerlinNoise((x + xOffset - settings.seed + .03f) / HumidityMapScale, (y + yOffset - settings.seed + .03f) / HumidityMapScale);
                humidityValue += mountainValue * .5f;
                humidityValue = Mathf.Clamp01(humidityValue);
                humidityValue = Mathf.InverseLerp(0f, 1f, humidityValue);

                float riverWindingScale = 150 + (25f * (mountainValue * 2f - 1f));
                freshWaterValue = Mathf.PerlinNoise((x + xOffset  - settings.seed + .01f) / riverWindingScale, (y + yOffset - settings.seed + .01f) / riverWindingScale) * 2f - 1f;
                rough = Mathf.Pow(Mathf.PerlinNoise((x + xOffset  + .01f) / 15f, (y + yOffset + .01f) / 15f) + .5f, .3f) - 1f;
                freshWaterValue += rough;
                freshWaterValue -= Mathf.PerlinNoise((x + xOffset  + .01f) / 400f, (y + yOffset + .01f) / 400f) / 2f;
                freshWaterValue = Mathf.Abs(freshWaterValue);
                freshWaterValue *= -1f;
                freshWaterValue += 1f;
                if (freshWaterValue > .95f) { freshWaterValue = Mathf.Pow(freshWaterValue, 1f); }
                else if (freshWaterValue > .88f)
                {
                    freshWaterValue = .95f;
                }
                else
                {
                    //freshWaterValue = Mathf.Pow(freshWaterValue, 7f * temperatureValue);
                    freshWaterValue = Mathf.Pow(freshWaterValue, 1f);
                }

                wetnessValue = freshWaterValue;
                float fwThreshhold = 1f + Mathf.Pow(Mathf.PerlinNoise((x + xOffset + .01f) / 10f, (y + yOffset + .01f) / 10f) + .5f, 1f) - 1f;
                fwThreshhold = Mathf.Clamp01(fwThreshhold);
                if (freshWaterValue < fwThreshhold)
                {
                    if (elevationValueSmooth < .02f)
                    {
                        wetnessValue = Mathf.Max(wetnessValue, fwThreshhold);
                    }
                }
                float mtnMod = Mathf.Pow(mountainValue + .5f, 1f) - 1f;
                wetnessValue += mtnMod;

                wetnessValue = Mathf.Clamp01(wetnessValue);

                // -------------------------------------------------------

                // BiomeMap
                biomeValue = Biome.GetBiome(temperatureValue, humidityValue);

                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;


                    float scale2 = settings.scale + 15f;
                    float scale3 = settings.scale - 15f;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    perlinValue += Mathf.PerlinNoise(sampleX * settings.scale / scale2, sampleY) * 2 - 1;
                    perlinValue -= Mathf.PerlinNoise(sampleX * settings.scale / scale3, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;


                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;

                }

                noiseHeight *= (40f * Mathf.PerlinNoise(((x + xOffset) / 120f) + 1000, ((y + yOffset) / 120f) + 1000));

                //ABS and INVERT, and normalize value
                noiseHeight = Mathf.Abs(noiseHeight);
                noiseHeight *= -1f;
                noiseHeight = Mathf.InverseLerp(-75f, .01f, noiseHeight);

                // reduce hills
                if (noiseHeight < settings.flatLevel)
                {
                    noiseHeight = settings.flatLevel;
                }
                else if (noiseHeight >= settings.flatLevel)
                {
                    tMod = Mathf.Pow(temperatureValue + .5f, .6f);
                    noiseHeight = Mathf.Lerp(noiseHeight, settings.flatLevel, (1f - mountainValue) * tMod);
                }

                // apply ElevationMap with respect to TemperatureMap
                noiseHeight += elevationValueSmooth * .1f;

                // flatten high areas with respect to TemperatureMap
                tMod = 1f - Mathf.InverseLerp(temperatureValue, .45f, .55f);
                float heightCutoff = Mathf.Lerp(settings.flatLevel + .01f, 1f, tMod);
                heightCutoff = Mathf.Clamp(heightCutoff, .86f + (settings.flatLevel - settings.seaLevel), 1f);
                if (noiseHeight > heightCutoff)
                {
                    noiseHeight = Mathf.Lerp(noiseHeight, heightCutoff, tMod);
                }

                // create ocean where height is below settings.flatLevel
                if (noiseHeight < settings.flatLevel)
                {
                    //noiseHeight = (settings.seaLevel - .01f);
                    float f = Mathf.InverseLerp(0f, .004f, settings.flatLevel - noiseHeight);
                    freshWaterValue = Mathf.Max(freshWaterValue, f);
                }

                if (noiseHeight >= settings.seaLevel)
                {

                    // add fresh water
                    if (freshWaterValue > 0f)
                    {
                        if (freshWaterValue > .93f)
                        {
                            float f = Mathf.InverseLerp(.93f, 1f, freshWaterValue);
                            noiseHeight = Mathf.Lerp(Mathf.Min(settings.flatLevel, noiseHeight), (settings.seaLevel - .01f), f);
                        }
                        else
                        {
                            rough = 0f;
                            if (freshWaterValue <= .9f)
                            {
                                rough = (Mathf.PerlinNoise((float)(x + xOffset) / .1f + .01f, (float)((y + yOffset) / .1f + .01f)) * 2f - 1f) / 1f;
                            }
                            noiseHeight = Mathf.Lerp(noiseHeight, settings.flatLevel, freshWaterValue + rough);
                        }
                    }


                }
                else
                {
                    if (freshWaterValue > 0f)
                    {
                        if (freshWaterValue > .93f)
                        {
                            float f = Mathf.InverseLerp(.93f, 1f, freshWaterValue);
                            noiseHeight = Mathf.Lerp(Mathf.Min(settings.flatLevel, noiseHeight), (settings.seaLevel - .01f), f);
                        }
                    }
                }

                // create slight roughness in terrain
                if (biomeValue == (int)BiomeType.Desert)
                {
                    float duneMag = .012f * (1f - Mathf.Pow(Mathf.Clamp01(freshWaterValue), 1.3f)) * (1f - Mathf.Pow(wetnessValue, 1.2f));
                    noiseHeight += duneMag * (1f - Mathf.Abs(Mathf.Sin((x + xOffset - settings.seed + .01f + Mathf.Sin(y + yOffset) * 8f) / 15f)));
                }
                else
                {
                    noiseHeight += .0025f * Mathf.PerlinNoise((x + xOffset - settings.seed + .01f) / 2f, (y + yOffset - settings.seed + .01f) / 2f);
                }





                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;

                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                if (humidityValue > maxLocalMoistureHeight)
                {
                    maxLocalMoistureHeight = humidityValue;

                }
                else if (humidityValue < minLocalMoistureHeight)
                {
                    minLocalMoistureHeight = humidityValue;
                }

                if (temperatureValue > maxLocalHeatHeight)
                {
                    maxLocalHeatHeight = temperatureValue;

                }
                else if (temperatureValue < minLocalHeatHeight)
                {
                    minLocalHeatHeight = temperatureValue;
                }


                heightMap.data[x, y] = noiseHeight;
                MoistureData.data[x, y] = humidityValue;
                HeatData.data[x, y] = temperatureValue;




                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedNoiseHeight = (heightMap.data[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    float normalizedMoistureHeight = (MoistureData.data[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    float normalizedHeatHeight = (HeatData.data[x, y] + 1) / (maxPossibleHeight / 0.9f);


                    heightMap.data[x, y] = Mathf.Clamp(normalizedNoiseHeight, 0, int.MaxValue);
                    MoistureData.data[x, y] = Mathf.Clamp(normalizedMoistureHeight, 0, int.MaxValue);
                    HeatData.data[x, y] = Mathf.Clamp(normalizedHeatHeight, 0, int.MaxValue);
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
                    heightMap.data[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, heightMap.data[x, y]);
                    MoistureData.data[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, MoistureData.data[x, y]);
                    HeatData.data[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, HeatData.data[x, y]);


                }

            }
        }


        return (heightMap, MoistureData, HeatData);

    }

    
}



[System.Serializable]
public class NoiseSettings
{
    [Header("Heat Map")]
    [SerializeField]
    public int HeatOctaves = 4;
    [SerializeField]
    public double HeatFrequency = 3.0;
    [SerializeField]
    public float ColdestValue = 0.05f;
    [SerializeField]
    public float ColderValue = 0.18f;
    [SerializeField]
    public float ColdValue = 0.4f;
    [SerializeField]
    public float WarmValue = 0.6f;
    [SerializeField]
    public float WarmerValue = 0.8f;

    [Header("Moisture Map")]
    [SerializeField]
    public int MoistureOctaves = 4;
    [SerializeField]
    public double MoistureFrequency = 3.0;
    [SerializeField]
    public float DryerValue = 0.27f;
    [SerializeField]
    public float DryValue = 0.4f;
    [SerializeField]
    public float WetValue = 0.6f;
    [SerializeField]
    public float WetterValue = 0.8f;
    [SerializeField]
    public float WettestValue = 0.9f;


    [Header("Noise Settings")]

    public Noise.NormalizeMode normalizeMode;
    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = .5f;
    public float lacunarity = 2;
    public int seed;
    public Vector2 offset;

    public float flatLevel = 0;
    public float seaLevel = 0.1f;
    public float snowLevel = 0.9f;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        if (scale == 5f) scale += 0.1f;
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}
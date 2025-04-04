//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TileGen : MonoBehaviour
//{
//    private void LoadTiles()
//    {

//        if (moistureMapValues.data[i, j] < moistureMapValues.Max)
//        {
//            moistureMapValues.Max = moistureMapValues.data[i, j];
//        }
//        if (moistureMapValues.data[i, j] < moistureMapValues.Min)
//        {
//            moistureMapValues.Min = moistureMapValues.data[i, j];

//        }

//        if (heatMapValues.data[i, j] > heatMapValues.Max)
//        {
//            heatMapValues.Max = heatMapValues.data[i, j];
//        }
//        if (heatMapValues.data[i, j] < heatMapValues.Min)
//        {
//            heatMapValues.Min = heatMapValues.data[i, j];
//        }






//    }
//}
//for (int x = 0; x < width; x++)
//{
//    for (int y = 0; y < height; y++)
//    {
//        Tile t = new Tile();
//        t.X = x;
//        t.Y = y;

//        float heightValue = noiseMapValues.data[x, y];
//        heightValue = (heightValue - noiseMapValues.Min) / (noiseMapValues.Max - noiseMapValues.Min);
//        t.HeightValue = heightValue;
//        if (noiseMapValues.data[x, y] < TerrainGenerator.DeepWater)
//        {
//            t.HeightType = HeightType.DeepWater;
//        }
//        else if (noiseMapValues.data[x, y] < TerrainGenerator.ShallowWater)
//        {
//            t.HeightType = HeightType.ShallowWater;
//        }
//        else if (noiseMapValues.data[x, y] < TerrainGenerator.Sand)
//        {
//            t.HeightType = HeightType.Sand;
//        }
//        else if (noiseMapValues.data[x, y] < TerrainGenerator.Grass)
//        {
//            t.HeightType = HeightType.Grass;
//        }
//        else if (noiseMapValues.data[x, y] < TerrainGenerator.Rock)
//        {
//            t.HeightType = HeightType.Rock;
//        }
//        else
//        {
//            t.HeightType = HeightType.Snow;
//        }

//        if (t.HeightType == HeightType.DeepWater)
//        {
//            moistureMapValues.data[t.X, t.Y] += 8f * t.HeightValue;
//        }
//        else if (t.HeightType == HeightType.ShallowWater)
//        {
//            moistureMapValues.data[t.X, t.Y] += 3f * t.HeightValue;
//        }
//        else if (t.HeightType == HeightType.Shore)
//        {
//            moistureMapValues.data[t.X, t.Y] += 1f * t.HeightValue;
//        }
//        else if (t.HeightType == HeightType.Sand)
//        {
//            moistureMapValues.data[t.X, t.Y] += 0.2f * t.HeightValue;
//        }

//        float moistureValue = moistureMapValues.data[x, y];
//        moistureValue = (moistureValue - moistureMapValues.Min) / (moistureMapValues.Max - moistureMapValues.Min);
//        t.MoistureValue = moistureValue;

//        //set moisture type
//        if (moistureValue < settings.noiseSettings.DryerValue) t.
//        = MoistureType.Dryest;
//        else if (moistureValue < settings.noiseSettings.DryValue) t.MoistureType = MoistureType.Dryer;
//        else if (moistureValue < settings.noiseSettings.WetValue) t.MoistureType = MoistureType.Dry;
//        else if (moistureValue < settings.noiseSettings.WetterValue) t.MoistureType = MoistureType.Wet;
//        else if (moistureValue < settings.noiseSettings.WettestValue) t.MoistureType = MoistureType.Wetter;
//        else t.MoistureType = MoistureType.Wettest;


//        // Adjust Heat Map based on Height - Higher == colder
//        if (t.HeightType == HeightType.Forest)
//        {
//            heatMapValues.data[t.X, t.Y] -= 0.1f * t.HeightValue;
//        }
//        else if (t.HeightType == HeightType.Rock)
//        {
//            heatMapValues.data[t.X, t.Y] -= 0.25f * t.HeightValue;
//        }
//        else if (t.HeightType == HeightType.Snow)
//        {
//            heatMapValues.data[t.X, t.Y] -= 0.4f * t.HeightValue;
//        }
//        else
//        {
//            heatMapValues.data[t.X, t.Y] += 0.01f * t.HeightValue;
//        }

//        // Set heat value
//        float heatValue = heatMapValues.data[x, y];
//        heatValue = (heatValue - heatMapValues.Min) / (heatMapValues.Max - heatMapValues.Min);
//        t.HeatValue = heatValue;

//        // set heat type
//        if (heatValue < settings.noiseSettings.ColdestValue) t.HeatType = HeatType.Coldest;
//        else if (heatValue < settings.noiseSettings.ColderValue) t.HeatType = HeatType.Colder;
//        else if (heatValue < settings.noiseSettings.ColdValue) t.HeatType = HeatType.Cold;
//        else if (heatValue < settings.noiseSettings.WarmValue) t.HeatType = HeatType.Warm;
//        else if (heatValue < settings.noiseSettings.WarmerValue) t.HeatType = HeatType.Warmer;
//        else t.HeatType = HeatType.Warmest;

//        Tiles[x, y] = t;
//    }
//}
//    }
//}

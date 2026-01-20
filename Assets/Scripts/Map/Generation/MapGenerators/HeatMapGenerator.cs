//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HeatMapGenerator
//{
//    static float[,] falloffMap;
//    public static (MapData, MapData, MapData) dataTuple;
//    static int HumidityMapScale = 300;
//    public static Tile[,] Tiles { get; private set; }

//    public static HeatMap GenerateHeatMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre)
//    {
//        Tiles = new Tile[width, height];

//        dataTuple = Noise.GenerateNoiseMaps(width, height, settings.noiseSettings, sampleCentre, settings.material);
//        MapData noiseMapValues = dataTuple.Item1;
//        MapData moistureMapValues = dataTuple.Item2;
//        MapData heatMapValues = dataTuple.Item3;




//        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

//        float minValue = float.MaxValue;
//        float maxValue = float.MinValue;
//        if (settings.useFalloff)
//        {
//            if (falloffMap == null)
//            {
//                falloffMap = FalloffGenerator.GenerateFalloffMap(width);
//            }
//        }
//        for (int i = 0; i < width; i++)
//        {
//            for (int j = 0; j < height; j++)
//            {

//                noiseMapValues.data[i, j] *= heightCurve_threadsafe.Evaluate(noiseMapValues.data[i, j] - (settings.useFalloff ? falloffMap[i, j] : 0)) * settings.heightMultiplier;

//                if (noiseMapValues.data[i, j] > maxValue)
//                {
//                    maxValue = noiseMapValues.data[i, j];
//                }
//                if (noiseMapValues.data[i, j] < minValue)
//                {
//                    minValue = noiseMapValues.data[i, j];
//                }







//            }
//        }


//        return new HeatMap(false, noiseMapValues.data, /*Tiles,*/ minValue, maxValue);
//    }
//}


//public class HeatMap
//{
//    public Mesh mesh;
//    public bool hasRequestedMesh;
//    public bool hasMesh;
//    int lod;

//    public event System.Action updateCallback;

//    public LODMesh(int lod)
//    {
//        this.lod = lod;
//    }

//    void OnMeshDataReceived(object meshDataObject)
//    {
//        mesh = ((MeshData)meshDataObject).CreateMesh();
//        hasMesh = true;

//        updateCallback();
//    }
//    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
//    {
//        hasRequestedMesh = true;
//        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);

//    }
//    public Mesh mesh;
//    public bool hasRequestedMesh;
//    public bool hasMesh;
//    int lod;

//    public event System.Action updateCallback;

//    public LODMesh(int lod)
//    {
//        this.lod = lod;
//    }

//    void OnMeshDataReceived(object meshDataObject)
//    {
//        mesh = ((MeshData)meshDataObject).CreateMesh();
//        hasMesh = true;

//        updateCallback();
//    }
//    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
//    {
//        hasRequestedMesh = true;
//        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);

//    }
//}
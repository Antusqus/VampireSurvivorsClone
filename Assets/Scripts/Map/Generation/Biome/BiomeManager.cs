using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    [SerializeField]
    public List<BiomePreset> presets = new List<BiomePreset>();
    public Dictionary<BiomeType, Dictionary<Vector2,Biome>> biomeDict = new Dictionary<BiomeType, Dictionary<Vector2, Biome>>();
    List<MapData> heatMaps = new List<MapData>();

    Tile[,] tiles;

    private void FixedUpdate()
    {
        
    }




    //public void LoadHeatMaps()
    //{
    //    ThreadedDataRequester.RequestData(() => HeatMapGenerator.GenerateHeatMap(width, height, heatMapSettings), OnHeatMapReceived);
    //}

    //void OnHeatMapReceived(object heatMapObject)
    //{
    //    this.heatMap = (HeatMap)heatMapObject;
    //    heatMapReceived = true;
    //    UpdateHeatMaps();
    //}

    //void UpdateHeatMaps()
    //{

    //}

    //private void GenerateHeatMap()
    //{
    //    HeatMap s;
    //}
}

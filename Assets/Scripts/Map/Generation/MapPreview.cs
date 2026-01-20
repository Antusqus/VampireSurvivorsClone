using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public enum DrawMode { NoiseMap, HeightMap, MoistureMap, HeatMap, Mesh, Biome, FalloffMap };
    public DrawMode drawMode;




    public BiomePreset preset;
    public MeshSettings meshSettings;
    //public preset.heightMapSettings preset.heightMapSettings;
    //public preset.heatMapSettings preset.heatMapSettings;
    //public preset.moistureMapSettings preset.moistureMapSettings;


    public TextureData textureData;
    public Material terrainMaterial;

    [Range(0, MeshSettings.numSupportedLOD - 1)]
    public int editorPreviewLOD;

    public bool autoUpdate;

    

    public void DrawMapInEditor()
    {
        MapGenerator mapGen = new MapGenerator();
        NoiseMaps noiseMaps = mapGen.GenerateMaps(Vector2.zero, meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, preset.heightMapSettings, preset.heatMapSettings, preset.moistureMapSettings, Vector2.zero);

        HeightMap heightMap = noiseMaps.heightMap;
        HeatMap heatMap = noiseMaps.heatMap;
        MoistureMap moistureMap = noiseMaps.moistureMap;


        if (drawMode == DrawMode.NoiseMap)
        {
            //S.Lague
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.HeightMap)
        {
            //JGallant
            DrawTexture(TextureGenerator.GetHeightMapTexture(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, noiseMaps.tiles));
        }
        else if (drawMode == DrawMode.MoistureMap)
        {
            DrawTexture(TextureGenerator.GetMoistureMapTexture(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, noiseMaps.tiles));
        }
        else if (drawMode == DrawMode.HeatMap)
        {
            DrawTexture(TextureGenerator.GetHeatMapTexture(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, noiseMaps.tiles));
        }

        else if (drawMode == DrawMode.Biome)
        {
            DrawTexture(TextureGenerator.GetBiomeMapTexture(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, noiseMaps.tiles, 0.05f, 0.18f, 0.4f));
        }


        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));

        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine),0,1)));
        }

    }

    public void DrawTexture(Texture2D texture)
    {

        textureRenderer.sharedMaterial.mainTexture = texture;
        // divide by 10f because it's unreasonably big.
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;
        textureRenderer.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }


    private void OnValidate()
    {

        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (preset.heightMapSettings != null)
        {
            preset.heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            preset.heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }

        //if (textureData != null)
        //{
        //    textureData.OnValuesUpdated -= OnTextureValuesUpdated;
        //    textureData.OnValuesUpdated += OnTextureValuesUpdated;
        //}

    }
}

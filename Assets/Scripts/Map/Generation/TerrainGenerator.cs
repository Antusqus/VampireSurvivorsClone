using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MeshGenerator;

public class TerrainGenerator : MonoBehaviour
{

    public const float viewerMoveThresholdForChunkUpdate = 50f;
    const float sqrviewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;


    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    public Transform viewer;
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    float meshWorldSize;
    int chunksVisibleInViewDist;

    public BiomeManager biomeManager;
    Biome tempNewBiome;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    public static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
	//COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
    };

    public static float DeepWater;
    public static float ShallowWater;
    public static float Sand;
    public static float Grass;
    public static float Rock;
    public static float Snow;

    private void Start()
    {
        biomeManager = FindObjectOfType<BiomeManager>();
        tempNewBiome = new Biome(biomeManager, _chunks: chunksVisibleInViewDist, name: "Biome Zero");

        float maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / meshWorldSize);
        UpdateVisibleChunks();

        DeepWater = mapMaterial.GetFloat("_DeepWater_Height");
        ShallowWater = mapMaterial.GetFloat("_Water_Height");

        Sand = mapMaterial.GetFloat("_Sand_Height");
        Grass = mapMaterial.GetFloat("_Grass_Height");
        Rock = mapMaterial.GetFloat("_Rock_Height");
        Snow = mapMaterial.GetFloat("_Snow_Height");

    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
                
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrviewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();

        }
    }

    public void UpdateVisibleChunks()
    {

        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
       
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);



        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDict.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        //TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, tempNewBiome.biomeObject.transform, viewer, mapMaterial);
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, tempNewBiome.preset.mapSettings, tempNewBiome.preset.meshSettings, detailLevels, colliderLODIndex, tempNewBiome.biomeObject.transform, viewer, mapMaterial);

                        //newChunk.t.BiomeType = GetBiomeType(newChunk.t);


                        if (tempNewBiome.terrainChunksInBiomeDict.Count < tempNewBiome.maxChunkCount)
                        {

                            tempNewBiome.terrainChunksInBiomeDict.Add(viewedChunkCoord, newChunk);

                        }
                        else
                        {
                            tempNewBiome = new Biome(biomeManager, name: "Biome " + viewedChunkCoord);
                        }


                        //TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDict.Add(viewedChunkCoord, newChunk);
                        

                        newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();

                        //}

                    }
                }



            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);

        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }



}
[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLOD - 1)]
    public int lod;
    public float visibleDistThreshold;

    public float sqrVisibleDistThreshold
    {
        get { return visibleDistThreshold * visibleDistThreshold; }
    }
}
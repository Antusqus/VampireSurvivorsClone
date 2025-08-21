using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using static MeshGenerator;

public class TerrainGenerator : MonoBehaviour
{

    public const float viewerMoveThresholdForChunkUpdate = 35f;
    const float sqrviewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;


    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public HeatMapSettings heatMapSettings;
    public MoistureMapSettings moistureMapSettings;
    public TextureData textureSettings;

    public Transform viewer;
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    float meshWorldSize;
    int chunksVisibleInViewDist;

    public BiomeManager biomeManager;
    Biome tempNewBiome;
    Biome tempOldBiome;
    GameObject worldMap;
    public Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    public ConcurrentDictionary<TerrainChunk, Biome> terrainChunkBiomeDict = new ConcurrentDictionary<TerrainChunk, Biome>();

    public List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
    TerrainChunk newChunk;
    TerrainChunk oldChunk;
    public static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
	//COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
	{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
    };

    public static Vector2[] neighborOffsets = new Vector2[]
{
    new Vector2(1, 0),
    new Vector2(-1, 0),
    new Vector2(0, 1),
    new Vector2(0, -1),
    // Optional: include diagonals if needed
    //new Vector2(1, 1),
    //new Vector2(-1, 1),
    //new Vector2(1, -1),
    //new Vector2(-1, -1)
};

    private void Start()
    {
        worldMap = new GameObject("WorldMap");
        worldMap.transform.position = Vector3.zero;

        biomeManager = FindObjectOfType<BiomeManager>();
        tempNewBiome = new Biome(biomeManager);
        tempNewBiome.biomeObject.transform.parent = worldMap.transform;


        float maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
        meshWorldSize = meshSettings.meshWorldSize;

        //for (int i = 0; i < neighborOffsets.Length; i++)
        //{
        //    neighborOffsets[i].x *= meshWorldSize;
        //    neighborOffsets[i].y *= meshWorldSize;

        //}

        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / meshWorldSize);
        UpdateVisibleChunks();

        foreach (TerrainChunk chunk in visibleTerrainChunks)
        {
            chunk.UpdateCollisionMesh();
            if (chunk.isBorderChunk)
                biomeManager.StartCoroutine(biomeManager.BlendChunkWithNeighbours(chunk));

        }

    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
                if(chunk.isBorderChunk)
                    biomeManager.StartCoroutine(biomeManager.BlendChunkWithNeighbours(chunk));

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
                        //TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, heatMapSettings, moistureMapSettings, meshSettings, detailLevels, colliderLODIndex, tempNewBiome.biomeObject.transform, viewer, mapMaterial);
                        newChunk = new TerrainChunk(viewedChunkCoord, tempNewBiome.preset.heightMapSettings, tempNewBiome.preset.heatMapSettings, tempNewBiome.preset.moistureMapSettings, meshSettings, detailLevels, colliderLODIndex, tempNewBiome.biomeObject.transform, viewer, mapMaterial);
                        newChunk.biome = tempNewBiome;
                        //newChunk.t.BiomeType = GetBiomeType(newChunk.t);

                        if (tempNewBiome.terrainChunksInBiomeDict.Count < tempNewBiome.maxChunkCount)
                        {

                            tempNewBiome.terrainChunksInBiomeDict[viewedChunkCoord] = newChunk;


                        }
                        else
                        {
                            tempOldBiome = tempNewBiome;
                            tempNewBiome = new Biome(biomeManager);
                            tempNewBiome.biomeObject.transform.parent = worldMap.transform;
                            biomeManager.AddSpawnedBiome(tempNewBiome);
                        }


                        //TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
                        terrainChunkDict[viewedChunkCoord] = newChunk;
                        terrainChunkBiomeDict[newChunk] = tempNewBiome;

                        newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();

                        //if (tempOldBiome != null && !tempOldBiome.smoothened)
                        //    biomeManager.StartCoroutine(biomeManager.BlendBiomes());


                    }
                }



            }
        }

        if(tempOldBiome != null && !tempOldBiome.scannerCoroutineRunning)
        {
            biomeManager.StartCoroutine(biomeManager.GetBiomeBorders(tempOldBiome));
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

    public bool TryGetChunk(Vector2 coord, out TerrainChunk chunk)
    {
        return terrainChunkDict.TryGetValue(coord, out chunk);
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
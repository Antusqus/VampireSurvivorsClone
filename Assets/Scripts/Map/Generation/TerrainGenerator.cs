using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MeshGenerator;

public class TerrainGenerator : MonoBehaviour
{

    const float viewerMoveThresholdForChunkUpdate = 25f;
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

    List<Biome> filledBiomeList = new List<Biome>();
    public BiomeManager biomeManager;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
    private void Start()
    {
        biomeManager = FindObjectOfType<BiomeManager>();
        float maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / meshWorldSize);
        UpdateVisibleChunks();
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

    Biome GenerateBiome()
    {
        OceanBiome newBiome = new OceanBiome(biomeManager);


        return newBiome.terrainChunksInBiomeDict.Count < newBiome.chunks ? newBiome : new OceanBiome(biomeManager);

    }

    void UpdateVisibleChunks()
    {

        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);


        Biome tempNewBiome = new Biome(biomeManager, name:"Biome " + filledBiomeList.Count);


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
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, tempNewBiome.biomeObject.transform, viewer, mapMaterial, tempNewBiome);


                        if (tempNewBiome.terrainChunksInBiomeDict.Count < tempNewBiome.chunks)
                        {
                            tempNewBiome.terrainChunksInBiomeDict.Add(viewedChunkCoord, newChunk);

                        }
                        else
                        {
                            filledBiomeList.Add(tempNewBiome);
                            tempNewBiome = new Biome(biomeManager, name: "Biome " + filledBiomeList.Count);
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
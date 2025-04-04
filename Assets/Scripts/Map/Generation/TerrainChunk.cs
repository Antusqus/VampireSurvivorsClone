using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    const float colliderGenerationDistanceThreshold = 5;
    MapGenerator mapGen;
    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    public Vector2 coord;
    GameObject meshObject;
    GameObject heatMap;
    Vector2 sampleCentre;
    Bounds bounds;

    MeshRenderer meshRenderer;
    public MeshRenderer heatMapRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    public NoiseMaps noiseMaps;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDist;
    Tile t;
    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;

    //TODO: Add biome to terrainchunk. This way we can generate the correct chunk type when we want to.
    //CONTINUE HERE: 


    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
    {
        mapGen = new MapGenerator();
        t = new Tile();
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;

        this.viewer = viewer;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk " + coord);
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        //meshRenderer.material = material;


        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;


        //heatMap = new GameObject("Heatmap Chunk");
        //heatMapRenderer = heatMap.AddComponent<MeshRenderer>();

        //heatMap.transform.position = new Vector3(position.x, 0, position.y + 40);
        //heatMap.transform.parent = meshObject.transform;
        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }

        }

        maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
    }

    public void Load()
    {
        
        ThreadedDataRequester.RequestData(() => mapGen.GenerateMaps(this.coord, meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
    }

    void OnHeightMapReceived(object mapObj)
    {
        NoiseMaps noiseMaps = (NoiseMaps)mapObj;
        this.noiseMaps = noiseMaps;
        mapGen.UpdateNeighbors();
        mapGen.GenerateRivers();
        mapGen.BuildRiverGroups();
        mapGen.DigRiverGroups();
        mapGen.AdjustMoistureMap();

        mapGen.UpdateBitmasks();
        mapGen.FloodFill();

        mapGen.GenerateBiomeMap();
        mapGen.UpdateBiomeBitmask();


        meshRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, noiseMaps.tiles, 0.05f, 0.18f, 0.4f);

        //meshRenderer.materials[0].mainTexture = TextureGenerator.GetHeatMapTexture(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, noiseMaps.tiles);
        // Make ref to NoiseSettings 'cold' values
        heightMapReceived = true;
        UpdateTerrainChunk();
    }


    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }


    public void UpdateTerrainChunk()
    {
        if (heightMapReceived)
        {

            float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            bool wasVisible = IsVisible();
            bool visible = viewerDistFromNearestEdge <= maxViewDist;

            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistFromNearestEdge > detailLevels[i].visibleDistThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;

                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(noiseMaps, meshSettings);
                    }
                }

            }
            if (wasVisible != visible)
            {

                SetVisible(visible);

                if (OnVisibilityChanged != null)
                {
                    OnVisibilityChanged(this, visible);

                }
            }
        }
    }
    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDistFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDistFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDistThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(noiseMaps, meshSettings);
                }
            }

            if (sqrDistFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }

    }
    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);

    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }
}


class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;

    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;

        updateCallback();
    }


    public void RequestMesh(NoiseMaps noiseMaps, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        //ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(noiseMaps.heightMap.values, meshSettings, lod), OnMeshDataReceived);
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(noiseMaps.heatMap.values, meshSettings, lod), OnMeshDataReceived);


    }
}

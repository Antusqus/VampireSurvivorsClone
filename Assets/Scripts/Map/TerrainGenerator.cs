using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// IDEA: Generate terrain per chunks. Divide x amount of chunks to spend for the generator to generate certain biomes. 
/// Create a graph for the chunk distribution, lest the biomes overlap too jarringly (e.g lava next to ice) ?
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    // Define tile size
    public int tileSize = 20;

    public Tilemap tilemap;

    public TileBase tileLand;
    public TileBase tilePavement;
    public TileBase tileFlower;

    // Start is called before the first frame update
    void Start()
    {
        int x = tileSize, y = tileSize;



        
    }


    /*function biome(e) {
    // a threshold between 0.2 and 0.5 work well in the demo
    // but each generator will need its own parameter tuning
    if (e < waterlevel) return WATER;
    else return LAND;
}*/



}

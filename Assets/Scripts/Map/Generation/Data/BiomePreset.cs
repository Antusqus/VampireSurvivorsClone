using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BiomePreset : UpdatableData
{
    public BiomeType biomeType;
    public HeightMapSettings heightMapSettings;
    public HeatMapSettings heatMapSettings;
    public MoistureMapSettings moistureMapSettings;
    public TextureData textureData;

}

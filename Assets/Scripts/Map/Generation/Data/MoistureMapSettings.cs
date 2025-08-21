using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoistureMapSettings", menuName = "2D Top-down Rogue-like/Terrain Data/Moisture Map Settings")]

public class MoistureMapSettings : MapSettings
{

    [Header("Moisture Map")]

    [SerializeField]
    public float DryerValue = 0.27f;
    [SerializeField]
    public float DryValue = 0.4f;
    [SerializeField]
    public float WetValue = 0.6f;
    [SerializeField]
    public float WetterValue = 0.8f;
    [SerializeField]
    public float WettestValue = 0.9f;
}

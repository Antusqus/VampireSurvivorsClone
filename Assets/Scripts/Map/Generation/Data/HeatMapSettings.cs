using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeatMapSettings", menuName = "2D Top-down Rogue-like/Terrain Data/Heat Map Settings")]

public class HeatMapSettings : MapSettings
{
    [Header("Heat Map")]

    [SerializeField]
    public float ColdestValue = 0.05f;
    [SerializeField]
    public float ColderValue = 0.18f;
    [SerializeField]
    public float ColdValue = 0.4f;
    [SerializeField]
    public float WarmValue = 0.6f;
    [SerializeField]
    public float WarmerValue = 0.8f;
}

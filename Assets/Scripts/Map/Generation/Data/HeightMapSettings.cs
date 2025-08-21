using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeightMapSettings", menuName = "2D Top-down Rogue-like/Terrain Data/Height Map Settings")]

public class HeightMapSettings : MapSettings
{
    [Header("Height Map")]

    [SerializeField]
    public float DeepWater = 0.2f;
    [SerializeField]
    public float ShallowWater = 0.4f;
    [SerializeField]
    public float Sand = 0.6f;
    [SerializeField]
    public float Grass = 0.8f;
    [SerializeField]
    public float Forest = 0.9f;

    public float Rock = 0.9f;

    //Could add snow. Currently snow is used as else case.

}

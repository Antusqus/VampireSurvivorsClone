using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSettings : UpdatableData
{
    public NoiseSettings noiseSettings;
    public Material material;

    public bool useFalloff;

    public float heightMultiplier = 1f;
    public AnimationCurve heightCurve;

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();

        base.OnValidate();
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-down Rogue-like/Weapon Data")]
public class WeaponData : ItemData
{
    [HideInInspector] public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;

    public Weapon.Stats GetLevelData(int level)
    {
        //Pick stats from the next level.
        if (level - 2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        //Otherwise, pick one of the stats from the random growth array.
        if (randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        Debug.LogWarning(string.Format("Weapon has no levelup stats configured for Level {0}", level));
        return new Weapon.Stats();
    }
}

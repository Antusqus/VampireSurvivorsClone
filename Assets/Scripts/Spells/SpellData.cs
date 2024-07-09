using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Spell Data", menuName = "2D Top-down Rogue-like/Spell Data")]
public class SpellData : ItemData
{
    [HideInInspector] public string behaviour;
    public Spell.Stats baseStats;
    public Spell.Stats[] linearGrowth;
    public Spell.Stats[] randomGrowth;

    public Spell.Stats GetLevelData(int level)
    {
        //Pick stats from the next level.
        if (level - 2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        //Otherwise, pick one of the stats from the random growth array.
        if (randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        Debug.LogWarning(string.Format("Spell has no levelup stats configured for Level {0}", level));
        return new Spell.Stats();
    }
}
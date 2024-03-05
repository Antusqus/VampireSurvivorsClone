using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-down Rogue-like/Passive Data")]
public class PassiveData : ItemData
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public Passive.Modifier GetLevelData(int level)
    {
        Debug.Log("Passive item upgrade level = " + level);
        //Pick stats from the next level.
        if (level - 2 < growth.Length)
            return growth[level - 2];

        Debug.LogWarning(string.Format("Passive has no levelup stats configured for Level {0}", level));

        return new Passive.Modifier();
    }

}

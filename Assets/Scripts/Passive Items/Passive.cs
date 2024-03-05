using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : Item
{
    public PassiveData data;
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]

    public struct Modifier
    {
        public string name, description;
        public CharacterData.Stats boosts;
    }

    // For dynamically created passives, call initialise to set everything up.
    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        if (!CanLevelUp())
        {
            Debug.LogWarning(
                string.Format(
                    "Cannot level up {0} to {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel)
                );
            return false;
        }


        currentBoosts += data.GetLevelData(++currentLevel).boosts;
        return true;
    }
}

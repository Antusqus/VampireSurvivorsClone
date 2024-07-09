using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    [HideInInspector] public PlayerStats owner;
    [HideInInspector] public Spell spell;

    public float GetDamage()
    {
        return spell.GetDamage();
    }
}

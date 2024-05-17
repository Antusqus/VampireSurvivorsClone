using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Attack Data", menuName = "2D Top-down Rogue-like/Enemy Data")]


public class EnemyAttackData : ScriptableObject
{
    [HideInInspector] public string behaviour;
    public EnemyAttack.Stats baseStats;

}

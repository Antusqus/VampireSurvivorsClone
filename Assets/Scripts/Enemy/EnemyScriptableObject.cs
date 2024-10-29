using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]

public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float maxHealth;
    [SerializeField]
    Damage damage;
    //[SerializeField]
    //float cooldown;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public Damage _Damage { get => damage; set => damage = value; }
    //public float Cooldown { get => cooldown; set => cooldown = value; }
}

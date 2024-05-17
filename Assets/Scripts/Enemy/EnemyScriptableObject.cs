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
    float damage;
    [SerializeField]
    float cooldown;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Damage { get => damage; set => damage = value; }
    public float Cooldown { get => cooldown; set => cooldown = value; }
}

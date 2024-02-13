using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon") ]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
    public GameObject prefab;

    [SerializeField]
    float damage;

    [SerializeField]
    float speed;

    [SerializeField]
    float cooldownDuration;

    [SerializeField]
    int pierce;

    [SerializeField]
    int level;  // Not meant to be modified ingame [Only editor]

    [SerializeField]
    GameObject nextLevelPrefab; // Prefab for visual updates on level up of item.

    [SerializeField]
    Sprite icon; // Not meant to be modified ingame [Only editor]

    [SerializeField]
    new string name;

    [SerializeField]
    string description;

    public GameObject Prefab { get => prefab; private set => prefab = value; }
    public float Damage { get => damage; private set => damage = value; }
    public float Speed { get => speed; private set => speed = value; }
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }
    public int Pierce { get => pierce; private set => pierce = value; }
    public int Level { get => level; set => level = value; }
    public GameObject NextLevelPrefab { get => nextLevelPrefab; set => nextLevelPrefab = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
}

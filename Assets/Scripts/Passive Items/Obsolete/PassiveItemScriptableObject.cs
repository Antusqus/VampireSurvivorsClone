using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;

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

    public float Multiplier { get => multiplier; set => multiplier = value; }
    public int Level { get => level; set => level = value; }
    public GameObject NextLevelPrefab { get => nextLevelPrefab; set => nextLevelPrefab = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
}

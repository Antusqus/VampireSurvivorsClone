using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/Characters")]

[System.Obsolete("Obsolete. Replaced by CharacterData")]
public class CharacterScriptableObject : ScriptableObject
{ 
    [SerializeField]
    Sprite icon;

    [SerializeField]
    new string name;

[SerializeField]
    GameObject startingWeapon;

    [SerializeField]
    float maxHealth;

    [SerializeField]
    float recovery;

    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float might;

    [SerializeField]
    float projectileSpeed;

    [SerializeField]
    float magnet;

    public GameObject StartingWeapon { get => startingWeapon; set => startingWeapon = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Recovery { get => recovery; set => recovery = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float Might { get => might; set => might = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public float Magnet { get => magnet; set => magnet = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public string Name { get => name; set => name = value; }
}

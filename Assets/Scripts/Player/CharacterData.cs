using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Character Data", menuName = "2D Top-down Rogue-like/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    Sprite icon;

    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    WeaponData startingWeapon;
    public WeaponData StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, maxStamina, maxMana, recovery, moveSpeed;
        public float might, speed, magnet;
        public int maxSummons;

        public Stats(float maxHealth = 1000, float maxStamina = 100, float maxMana = 100, float recovery = 0, float movespeed = 1f, float might =1f, float speed = 1f, float magnet =30f, int maxSummons= 6)
        {
            this.maxHealth = maxHealth;
            this.maxStamina = maxStamina;
            this.maxMana = maxMana;

            this.recovery = recovery;
            this.moveSpeed = movespeed;
            this.might = might;
            this.speed = speed;
            this.magnet = magnet;
            this.maxSummons = maxSummons;
        }

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.maxStamina += s2.maxStamina;
            s1.maxMana += s2.maxMana;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.speed += s2.speed;
            s1.magnet += s2.magnet;
            s1.maxSummons += s2.maxSummons;
            return s1;
        }

    }
    public Stats stats = new Stats(1000);
}

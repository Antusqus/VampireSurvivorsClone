using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class Damage
{
    [SerializeField] private PhysicalDamage.Slash slash;
    [SerializeField] private PhysicalDamage.Pierce pierce;
    [SerializeField] private PhysicalDamage.Blunt blunt;

    [SerializeField] private ElementalDamage.Fire fire;
    [SerializeField] private ElementalDamage.Cold cold;
    [SerializeField] private ElementalDamage.Lightning lightning;
    [SerializeField] private ElementalDamage.Arcane arcane;

    public PhysicalDamage.Slash Slash { get => slash; set => slash = value; }
    public PhysicalDamage.Pierce Pierce { get => pierce; set => pierce = value; }
    public PhysicalDamage.Blunt Blunt { get => blunt; set => blunt = value; }
    public ElementalDamage.Fire Fire { get => fire; set => fire = value; }
    public ElementalDamage.Cold Cold { get => cold; set => cold = value; }
    public ElementalDamage.Lightning Lightning { get => lightning; set => lightning = value; }
    public ElementalDamage.Arcane Arcane { get => arcane; set => arcane = value; }

    public Damage(float slash = 0, float pierce = 0, float blunt = 0, float fire = 0, float cold = 0, float lightning = 0, float arcane = 0)
    {
        this.Slash.dmg = slash;
        this.Pierce.dmg = pierce;
        this.Blunt.dmg = blunt;
        this.Fire.dmg = fire;
        this.Cold.dmg = cold;
        this.Lightning.dmg = lightning;
        this.Arcane.dmg = arcane;
    }

    //public bool AllZeroes()
    //{
    //    return GetType().GetProperties()
    //    .Where(pi => pi.PropertyType == typeof(float)) // check type
    //    .Select(pi => (float)pi.GetValue(this)) // get the values
    //    .Any(x => x != 0); // filter the result (note the !)
    //}

    public float SumTotal()
    {
        return 
            Slash.dmg +
            Pierce.dmg +
            Blunt.dmg +
            Fire.dmg +
            Cold.dmg +
            Lightning.dmg +
            Arcane.dmg;
    }

}

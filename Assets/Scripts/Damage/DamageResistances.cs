using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class DamageResistances
{

    //Attach this to Unit.
    public enum ArmourType
    {
        None,
        Light,
        Medium,
        Heavy
    };
    
    [SerializeField] private ArmourType _armourType;

    [SerializeField] private float armour;
    [SerializeField] private float fire;
    [SerializeField] private float cold;
    [SerializeField] private float lightning;
    [SerializeField] private float arcane;
    public ArmourType _ArmourType { get => _armourType; set => _armourType = value; }
    public float Armour { get => armour; set => armour = value; }

    public float Fire { get => fire; set => fire = value; }
    public float Cold { get => cold; set => cold = value; }
    public float Lightning { get => lightning; set => lightning = value; }
    public float Arcane { get => arcane; set => arcane = value; }


    public DamageResistances(ArmourType a = ArmourType.None, float _aVal = 0, float _fRes = 0, float _cRes = 0, float _lRes = 0, float _aRes = 0)
    {
        _ArmourType = a;
        Armour = _aVal;

        Fire = _fRes;
        Cold = _cRes;
        Lightning = _lRes;
        Arcane = _aRes;
    }

}






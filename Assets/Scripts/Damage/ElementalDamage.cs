using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]

public class ElementalDamage : DamageType
{
    [System.Serializable]

    public class Fire
    {
        public float dmg;
        public Fire(float _dmg)
        {
            dmg = _dmg;
        }
    }
    [System.Serializable]

    public class Cold
    {
        public float dmg;
        public Cold(float _dmg)
        {
            dmg = _dmg;
        }
    }
    [System.Serializable]

    public class Lightning
    {
        public float dmg;
        public Lightning(float _dmg)
        {
            dmg = _dmg;
        }
    }
    [System.Serializable]

    public class Arcane
    {
        public float dmg;
        public Arcane(float _dmg)
        {
            dmg = _dmg;
        }
    }
}


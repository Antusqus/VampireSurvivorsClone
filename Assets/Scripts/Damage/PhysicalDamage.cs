using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]

public class PhysicalDamage : DamageType
{
    [System.Serializable]
    public class Slash
    {
        public float dmg;
        public Slash(float _dmg)
        {
            dmg = _dmg;
        }
    }

    [System.Serializable]
    public class Pierce
    {
        public float dmg;
        public Pierce(float _dmg)
        {
            dmg = _dmg;
        }
    }
    [System.Serializable]

    public class Blunt
    {
        public float dmg;
        public Blunt(float _dmg)
        {
            dmg = _dmg;
        }
    }
}


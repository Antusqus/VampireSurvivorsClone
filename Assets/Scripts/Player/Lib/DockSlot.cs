using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockSlot
{
    public int nr;
    public GameObject dockSlot;
    bool assigned;
    Minion _minion;
    public DockSlot(int _slotNr, GameObject _dockSlot, bool _assigned = false)
    {
        nr = _slotNr;
        dockSlot = _dockSlot;
        _minion = null;
    }

    public bool Assigned { get => assigned; set => assigned = value; }
    public Minion Minion {
        get
        {
            if (Assigned)
            {
                return Minion;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (Assigned)
            {
                _minion = value ;
            }
            else
            {
                _minion = null;
            }
        }
    }
}

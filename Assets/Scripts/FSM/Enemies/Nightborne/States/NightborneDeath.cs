using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightborneDeath : IState
{
    protected Nightborne unit;

    public NightborneDeath(Nightborne _owner) { this.unit = _owner; }
    public virtual void Enter()
    {
        unit.am.SetBool("Death", true);
        Debug.Log("Owner: " + unit + " entering Death state");
    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {
        //Debug.Log("Owner: " + unit + " Exit atk state");

    }

}

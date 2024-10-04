using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightborneAtkState : IState
{
    protected Nightborne unit;

    public NightborneAtkState(Nightborne _owner) { this.unit = _owner; }
    public virtual void Enter()
    {
        unit.am.SetBool("Atk", true);
        //Debug.Log("Owner: " + unit + " entering atk state");
    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {
        unit.am.SetBool("Atk", false);
        //Debug.Log("Owner: " + unit + " Exit atk state");

    }

}

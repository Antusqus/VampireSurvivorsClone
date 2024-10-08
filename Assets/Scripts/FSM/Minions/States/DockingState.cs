using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingState : IState
{
    Minion owner;

    public DockingState(Minion owner) { this.owner = owner; }

    public void Enter()
    {
        //Debug.Log(owner.SummonNr + "entering dock state");
    }

    public void Execute()
    {
        //Debug.Log(owner.SummonNr + "updating dock state");
        owner.dockTarget = owner.Slot.dockSlot.transform.position;

    }

    public void Exit()
    {
        //Debug.Log(owner.SummonNr + "exiting dock state");
    }
}

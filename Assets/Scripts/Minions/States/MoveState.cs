using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IState
{
    SummonedMinion owner;

    public MoveState(SummonedMinion owner) { this.owner = owner; }

    public void Enter()
    {
        Debug.Log(owner.SummonNr + "entering move state");
    }

    public void Execute()
    {
        Debug.Log(owner.SummonNr + "updating move state");
            if (owner.GetClosestEnemy() == null)
            {
                owner.stateMachine.ChangeState(new DockingState(owner));
            }
    }

    public void Exit()
    {
        Debug.Log(owner.SummonNr + "exiting move state");
    }
}

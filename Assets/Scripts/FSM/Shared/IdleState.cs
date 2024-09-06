using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    protected Unit owner;

    public IdleState(Unit owner) { this.owner = owner; }
    public virtual void Enter()
    {
        Debug.Log("Owner: " + owner + " entering idle state");
    }

    public virtual void Execute()
    {
        Debug.Log("Owner: " + owner + " Execute idle state");

    }

    public virtual void Exit()
    {
        Debug.Log("Owner: " + owner + " Exit idle state");

    }
}

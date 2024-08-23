using UnityEngine;

public class IdleState : IState
{
    SummonedMinion owner;
    float pollingTime;

    public IdleState(SummonedMinion owner) { this.owner = owner; }

    public void Enter()
    {
        pollingTime = 0f;
        Debug.Log(owner.SummonNr + " entering idle state");
    }

    public void Execute()
    {
        Debug.Log(owner.SummonNr + "updating idle state");
        owner.dockTarget = owner.Slot.dockSlot.transform.position;

        if (pollingTime <= 0f)
            {
                if (owner.GetClosestEnemy() != null)
                {
                    owner.stateMachine.ChangeState(new MoveState(owner));
                return;
                }
                Debug.Log("Closest enemy null");

                pollingTime = 3f;
            }
            pollingTime -= Time.deltaTime;
    }

    public void Exit()
    {
        Debug.Log(owner.SummonNr + "exiting idle state");
    }
}
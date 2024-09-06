using UnityEngine;

public class MinionIdleState : IdleState
{
    Minion minion;
    float pollingTime;

    public MinionIdleState(Minion minion ):base(minion){ this.minion = minion; }

    public override void Execute()
    {
        Debug.Log(minion.SummonNr + "updating idle state");
        minion.dockTarget = minion.Slot.dockSlot.transform.position;

        if (pollingTime <= 0f)
            {
                if (minion.GetClosestEnemy() != null)
                {
                    minion.stateMachine.ChangeState(new MoveState(minion));
                return;
                }
                Debug.Log("Closest enemy null");

                pollingTime = 3f;
            }
            pollingTime -= Time.deltaTime;
    }
}
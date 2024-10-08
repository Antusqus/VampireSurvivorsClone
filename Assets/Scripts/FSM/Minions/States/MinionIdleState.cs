using UnityEngine;

public class MinionIdleState : IdleState
{
    Minion _minion;
    float pollingTime;

    public MinionIdleState(Minion minion) :base(minion){ _minion = minion; }

    public override void Execute()
    {
        //Debug.Log(_minion.SummonNr + "updating idle state");
        _minion.dockTarget = _minion.Slot.dockSlot.transform.position;

        if (pollingTime <= 0f)
            {
                if (_minion.GetClosestEnemy() != null)
                {
                    _minion.stateMachine.ChangeState(new MoveState(_minion));
                return;
                }
                Debug.Log("Closest enemy null");

                pollingTime = 3f;
            }
            pollingTime -= Time.deltaTime;
    }
}
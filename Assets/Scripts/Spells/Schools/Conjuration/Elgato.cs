using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elgato : SummonedMinion
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();



        if (bestTarget)
        {
            transform.position = Vector2.MoveTowards(transform.position, bestTarget.transform.position, player.CurrentMoveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, dockTarget, player.CurrentMoveSpeed * Time.deltaTime);

            if (TargetReached(dockTarget))
            {
                stateMachine.ChangeState(new IdleState(this));

            }
        }



    }


    bool TargetReached(Vector3 target)
    {
        if (transform.position == target && stateMachine.currentState is DockingState)
        {
            return true;
        }


        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElgatoSpell : Minion
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
            transform.position = Vector2.MoveTowards(transform.position, bestTarget.transform.position, 5 * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, dockTarget, 5* Time.deltaTime);

            if (TargetReached(dockTarget))
            {
                stateMachine.ChangeState(new MinionIdleState(this));

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

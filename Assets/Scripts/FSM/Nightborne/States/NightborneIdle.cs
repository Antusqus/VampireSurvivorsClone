using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightborneIdle : IState
{
    Nightborne unit;

    public NightborneIdle(Nightborne _unit) { unit = _unit; }
    float pollTimer = 3f;

    public virtual void Enter()
    {
        unit.StartCoroutine(unit.WaitForAnim());

        foreach (AnimatorControllerParameter parameter in unit.am.parameters)
        {
            unit.am.SetBool(parameter.name, false);
        }
    }

    public virtual void Execute()
    {
        if (pollTimer < 0)
        {
            if (unit.GetClosestTarget() != null)
            {
                unit.stateMachine.ChangeState(new NightborneRun(unit));
            }
            pollTimer = 1f;
        }

        pollTimer -= Time.deltaTime;

    }

    public virtual void Exit()
    {
    }
}

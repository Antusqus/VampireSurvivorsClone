using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightborneRun : IState
{
    Nightborne unit;
    public NightborneRun(Nightborne _unit) { this.unit = _unit; }

    public virtual void Enter()
    {
        unit.am.SetBool("Run", true);
        //Debug.Log("Owner: " + unit + " entering run state");
    }

    public virtual void Execute()
    {
        //Debug.Log("Owner: " + unit + " Execute run state");
        if (unit.am.GetBool("Run"))
        {
            unit.transform.position = Vector2.MoveTowards(unit.transform.position, unit.bestTarget.transform.position, unit.currentMoveSpeed * Time.deltaTime);
            Vector3 direction = (unit.bestTarget.transform.position - unit.transform.position).normalized;
            unit.SpriteDirectionChecker(direction);
        }




    }

    public virtual void Exit()
    {
        unit.am.SetBool("Run", false);
        //Debug.Log("Owner: " + unit + " Exit run state");

    }
}

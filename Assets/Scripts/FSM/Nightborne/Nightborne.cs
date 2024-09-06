using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nightborne : Unit
{
    Animator am;
    // Start is called before the first frame update
    protected override void Start()
    {
        stateMachine.ChangeState(new IdleState(this));
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (stateMachine.currentState != null) stateMachine.currentState.Execute();
    }
}

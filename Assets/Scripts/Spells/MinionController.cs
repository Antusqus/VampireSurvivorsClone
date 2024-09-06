using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : SpellEffect
{
    protected Minion minion;
    
    protected PlayerStats player;



    // Start is called before the first frame update
    protected virtual void Start()
    {

        player = FindObjectOfType<PlayerStats>();

        minion = gameObject.GetComponent<Minion>();


    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if (minion.stateMachine.currentState != null) minion.stateMachine.currentState.Execute();

    }

    
}

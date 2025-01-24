using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeBaseState : IState
{
    protected PlayerStats player;
    protected ComboPart part;

    public PlayerMeleeBaseState(PlayerStats owner, ComboPart combo) { this.player = owner; this.part = combo; }
    public virtual void Enter()
    {
        Debug.Log("Owner: " + player + " entering " + part.PartName);

        player.am.SetBool("Slashing", true);
        player.am.SetBool(part.PartName, true);
        //player.input.continueCombo = false;
    }

    public virtual void Execute()
    {
        player.input.HandleComboChain(part);

        //Debug.Log("Owner: " + player + " Execute " + part.PartName);
        //player.input.Move();
        //player.input.HandleMovement();
    }

    public virtual void Exit()
    {
        Debug.Log("Exit atk state of " + part.PartName );

        player.am.SetBool(part.PartName, false);
        player.am.SetBool("Slashing", false);
    }
}

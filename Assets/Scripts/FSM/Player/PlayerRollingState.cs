using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollingState : IState
{
    protected PlayerStats player;

    public PlayerRollingState(PlayerStats owner) { this.player = owner; }
    public virtual void Enter()
    {
        Debug.Log("Owner: " + player + " entering rolling state");

       
            player.am.SetBool("Rolling", true);
            player.input.slideSpeed = 350f;
        



    }

    public virtual void Execute()
    {
        //Debug.Log("Owner: " + player + " Execute rolling state");
        player.input.HandleRollSliding();
    }

    public virtual void Exit()
    {
        Debug.Log("Owner: " + player + " Exit rolling state");
        player.am.SetBool("Rolling", false);
        player.input.slideSpeed = 0f;

    }
}

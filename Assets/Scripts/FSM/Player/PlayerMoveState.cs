using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IState
{
    protected PlayerStats player;

    public PlayerMoveState(PlayerStats owner) { this.player = owner; }
    public virtual void Enter()
    {
        player.am.SetBool("Move", true);

        //Debug.Log("Owner: " + player + " entering move state");
    }

    public virtual void Execute()
    {
        //Debug.Log("Owner: " + player + " Execute move state");
        //player.input.Move();

        if (player.input._moveAction.WasReleasedThisFrame())
        {
            player.stateMachine.ChangeState(new PlayerIdleState(player));
        }

    }

    public virtual void Exit()
    {
        //Debug.Log("Owner: " + player + " Exit move state");

    }


}

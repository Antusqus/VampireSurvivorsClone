using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IState
{
    protected PlayerStats player;

    public PlayerIdleState(PlayerStats owner) { this.player = owner; }
    public virtual void Enter()
    {
        Debug.Log("Owner: " + player + " entering idle state");
    }

    public virtual void Execute()
    {
        //Debug.Log("Owner: " + player + " Execute idle state");
        //player.input.Move();
        //player.input.HandleMovement();

        if (player.input._moveAction.triggered || player.input._moveAction.IsInProgress())
        {
            player.stateMachine.ChangeState(new PlayerMoveState(player));
        }
    }

    public virtual void Exit()
    {
        Debug.Log("Owner: " + player + " Exit idle state");

    }

}

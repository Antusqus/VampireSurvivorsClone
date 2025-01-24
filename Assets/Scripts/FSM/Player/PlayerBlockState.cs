using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockState : IState
{
    protected PlayerStats player;

    public PlayerBlockState(PlayerStats owner) { this.player = owner; }
    public virtual void Enter()
    {
        Debug.Log("Owner: " + player + " entering blk state");
        player.am.SetBool("Blocking", true);
        player.input.moveDir = Vector2.zero;


    }

    public virtual void Execute()
    {
        Debug.Log("Owner: " + player + " Execute blk state");
        player.input.HandleBlocking();
    }

    public virtual void Exit()
    {
        Debug.Log("Owner: " + player + " Exit blk state");
        player.am.SetBool("Blocking", false);
        //player.stateMachine.ChangeState(new PlayerIdleState(player));

    }
}

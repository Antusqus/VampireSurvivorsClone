using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeFinisherState : PlayerMeleeBaseState
{


    public PlayerMeleeFinisherState(PlayerStats owner, ComboPart combo) : base(owner, combo)
    {
        this.player = owner;
        this.part = combo;
    }

}

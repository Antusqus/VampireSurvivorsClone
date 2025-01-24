using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeEntryState : PlayerMeleeBaseState
{


    public PlayerMeleeEntryState(PlayerStats owner, ComboPart combo):base(owner, combo)
    {
        this.player = owner;
        this.part = combo;
    }

}

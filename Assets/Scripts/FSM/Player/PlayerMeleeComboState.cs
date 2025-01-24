using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeComboState : PlayerMeleeBaseState
{


    public PlayerMeleeComboState(PlayerStats owner, ComboPart combo) : base(owner, combo)
    {
        this.player = owner;
        this.part = combo;
    }

}

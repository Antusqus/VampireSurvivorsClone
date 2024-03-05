using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGem : Pickup
{
    public int expGranted;
    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }

        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreaseExp(expGranted);
    }

}

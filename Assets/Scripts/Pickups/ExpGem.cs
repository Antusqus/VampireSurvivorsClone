using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGem : Pickup, ICollectible
{
    public int expGranted;
    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreaseExp(expGranted);
    }

}

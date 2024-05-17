using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Pickup
{
    public int healthToRestore;
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (healthToRestore != 0) target.RestoreHealth(healthToRestore);
    }
}

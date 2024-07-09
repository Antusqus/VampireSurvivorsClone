using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Pickup
{
    public int healthToRestore;
    protected override void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        if (!target) return;

        base.OnDestroy();
        if (healthToRestore != 0) target.RestoreHealth(healthToRestore);
    }
}

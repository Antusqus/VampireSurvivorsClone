using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGem : Pickup
{
    public int exp;


    protected override void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        if (!target) return;

        base.OnDestroy();
        if (exp != 0) target.IncreaseExp(exp);
    }

}

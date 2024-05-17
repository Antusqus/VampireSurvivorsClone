using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGem : Pickup
{
    public int exp;


    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (exp != 0) target.IncreaseExp(exp);
    }

}

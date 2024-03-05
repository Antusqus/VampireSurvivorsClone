using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete("This is obsolete and will be replaced.")]

public class KnifeController : WeaponController
{


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();

        GameObject spawnedKnife = Instantiate(weaponData.prefab);
        spawnedKnife.transform.position = transform.position;
        spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector);
    }
}

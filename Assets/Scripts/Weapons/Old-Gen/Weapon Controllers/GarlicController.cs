using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete("This is obsolete and will be replaced.")]

public class GarlicController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedGarlic = Instantiate(weaponData.prefab);
        spawnedGarlic.transform.position = transform.position;
        spawnedGarlic.transform.parent = transform;
    }
}

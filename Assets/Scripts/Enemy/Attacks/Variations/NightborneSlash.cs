using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightborneSlash : EnemyAttack
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {

        if (Vector2.Distance(player.transform.position, transform.position) < currentStats.range)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f && !isRearming)
            {
                Attack();
            }
        }
    }

}

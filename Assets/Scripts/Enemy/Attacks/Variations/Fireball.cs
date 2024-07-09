using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : EnemyAttack
{

    protected float currentAttackInterval;
    protected int currentAttackCount;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }


    public override bool CanAttack()
    {

        if (currentCharges <= 0)
            StartCoroutine(Rearm());

        return currentCooldown <= 0 && !isRearming;
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!CanAttack()) return false;

        else
        {
            currentCooldown = currentStats.cooldown;
            FireballProj();


            if (currentCooldown <= 0)
                currentCooldown = currentStats.cooldown;
            if (currentCharges > 0)
            {
                currentCharges--;
                attackCount--;
            }



            if (attackCount > 0)
            {
                currentAttackCount = attackCount;
                currentAttackInterval = 1;
            }
            return true;
        }

    }

    public EnemyProjectile FireballProj()
    {
        if (!owner)
            return null;

        EnemyProjectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position,
            Quaternion.identity
            );
        prefab.atk = this;
        prefab.owner = owner;

        return prefab;
    }



}

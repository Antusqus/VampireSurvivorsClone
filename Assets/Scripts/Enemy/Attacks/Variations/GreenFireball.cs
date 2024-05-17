using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenFireball : Fireball
{
    protected override bool Attack(int attackCount = 1)
    {
        if (!CanAttack()) return false;

        else
        {
            currentCooldown += currentStats.cooldown;
            EnemyProjectile prefab = FireballProj();

            if (currentCooldown <= 0)
                currentCooldown += currentStats.cooldown;

            attackCount--;

            if (attackCount > 0)
            {
                currentAttackCount = attackCount;
                currentAttackInterval = 1;
            }
            return true;
        }

    }

}

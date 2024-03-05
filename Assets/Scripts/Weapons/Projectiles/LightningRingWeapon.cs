using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRingWeapon : ProjectileWeapon
{

    List<EnemyStats> allSelectedEnemies = new List<EnemyStats>();

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.hitEffect)
        {
            Debug.LogWarning(string.Format("No prefab set for projectile: {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        // No assigned projectile.
        if (!CanAttack()) return false;

        // Reset the active spawncount if this is the first time the attack has fired.
        if (currentCooldown <= 0)
        {
            allSelectedEnemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());
            currentCooldown += currentStats.cooldown;
            currentAttackCount = attackCount;
        }

        EnemyStats target = PickEnemy();
        if (target)
        {
            DamageArea(target.transform.position, currentStats.area, GetDamage());
            Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
        }

        if (attackCount > 0)
        {
            currentAttackCount = attackCount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;

    }

    EnemyStats PickEnemy()
    {
        EnemyStats target = null;
        while(!target && allSelectedEnemies.Count > 0)
        {
            int idx = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[idx];

            // If the target is dead, remove from target list and skip.
            if (!target)
            {
                allSelectedEnemies.RemoveAt(idx);
                continue;
            }

            // Check if enemy is on screen.

            Renderer r = target.GetComponent<Renderer>();
            if(!r || !r.isVisible)
            {
                allSelectedEnemies.Remove(target);
                target = null;
                continue;
            }
        }

        allSelectedEnemies.Remove(target);
        return target;
    }

    void DamageArea(Vector2 pos, float radius, float dmg)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(pos, radius);
        foreach (Collider2D t in targets)
        {
            EnemyStats es = t.GetComponent<EnemyStats>();
            if (es) es.TakeDamage(dmg, transform.position);
        }
    }
}

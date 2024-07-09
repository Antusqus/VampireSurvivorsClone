using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{
    int currentSpawnCount; // How many whip attacks spawned during this iteration
    float currentSpawnYOffset; // Offset used to space out multiply spawned whips.
    

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
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
            currentSpawnCount = 0;
            currentSpawnYOffset = 0f;
        }

        //Otherwise, calculate the offset of our multiply spawned attacks.

        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax), currentSpawnYOffset);

        WeaponProjectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)spawnOffset, Quaternion.identity);
        prefab.owner = owner;

        // Flip sprite depending on direction.
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(-Mathf.Abs(prefab.transform.localScale.x), prefab.transform.localScale.y, prefab.transform.localScale.z);
        }

        prefab.weapon = this;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;

        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
            currentSpawnYOffset += 1;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }
}

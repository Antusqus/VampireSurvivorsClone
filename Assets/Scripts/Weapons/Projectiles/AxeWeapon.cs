using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    protected override void Start()
    {
        base.Start();
    }
    protected override float GetSpawnAngle()
    {
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : Random.Range(1, 7);
        float angle = 90f - Mathf.Sign(movement.lastMovedVector.x) * (5 * offset);

        return angle;
    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
            );
    }
}

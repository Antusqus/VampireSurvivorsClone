using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpell : Spell
{


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override bool CanCast()
    {

        if (currentCharges > 0 && !isRearming) return true;
        return base.CanCast();
    }

    protected override void Cast()
    {

        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("No proj prefab set for {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return;
        }

        if (!CanCast()) return;

        StartCoroutine(SpawnPrefab());

        if (currentCharges > 0)
        {
            currentCharges--;
            currentCastInterval = data.baseStats.castInterval;
        }

    }

    protected virtual float GetSpawnAngle()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = transform.position.z;
        Vector3 targetDir = mousePos - transform.position;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) ;
        if (angle > Mathf.PI) { angle -= 2 * Mathf.PI; }
        else if (angle <= -Mathf.PI) { angle += 2 * Mathf.PI; }

        float angleInDeg = angle * Mathf.Rad2Deg;
        return angleInDeg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
            );
    }

    private IEnumerator SpawnPrefab()
    {
        Stats stats = GetStats();
        // Change waittimer to a formula regarding castspeed + some cast timer value
        ownerInput.transform.GetChild(4).gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        ownerInput.transform.GetChild(4).gameObject.SetActive(false);

        float spawnAngle = GetSpawnAngle();
        SpellProjectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
            );

        prefab.spell = this;
        prefab.owner = owner;
    }
}

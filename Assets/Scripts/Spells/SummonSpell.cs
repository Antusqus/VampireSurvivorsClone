using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpell : Spell
{
    int summonCount;
    SummonedMinion[] summonedMinions;

    protected override void Start()
    {
        base.Start();
        //Todo: Make summoncount a stat for the player
        summonedMinions = new SummonedMinion[6];
        summonCount = 0;

    }
    protected override void Cast()
    {

        if (!currentStats.minionPrefab)
        {
            Debug.LogWarning(string.Format("No minion prefab set for {0}", name));
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

    private IEnumerator SpawnPrefab()
    {
        Stats stats = GetStats();
        // Change waittimer to a formula regarding castspeed + some cast timer value
        ownerInput.transform.GetChild(4).gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        ownerInput.transform.GetChild(4).gameObject.SetActive(false);

        SummonedMinion prefab = Instantiate(
            currentStats.minionPrefab,
            owner.transform.position,
            Quaternion.Euler(0, 0, 0)
            );

        if (summonedMinions[summonCount])
        {
            Destroy(summonedMinions[summonCount].gameObject);
            summonedMinions[summonCount] = null;
        }

        Debug.Log("Inserting: Minion " + summonCount +" out of " + summonedMinions.Length);
        summonedMinions[summonCount] = prefab;

        summonCount++;
        summonCount %= summonedMinions.Length;

        prefab.spell = this;
        prefab.SummonNr = summonCount;

        prefab.owner = owner;
    }
}

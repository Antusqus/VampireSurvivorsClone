using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpell : Spell
{
    int summonCount;
    SummonedMinion[] summonedMinions;
    PlayerStats player;
    SummonTable summonTable;

    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerStats>();
        summonedMinions = new SummonedMinion[player.CurrentMaxSummons];
        for (int i = 0; i < player.CurrentMaxSummons; i++)
        {
            summonedMinions[i] = null;
        }
        summonCount = 0;
        summonTable = player.summonTable;
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
            summonTable.Unassign(summonedMinions[summonCount].Slot);
            Destroy(summonedMinions[summonCount].gameObject);
            summonedMinions[summonCount] = null;
        }

        Debug.Log("Inserting: Minion " + summonCount +" out of " + summonedMinions.Length);
        summonedMinions[summonCount] = prefab;

        prefab.spell = this;
        prefab.owner = owner;

        prefab.SummonNr = summonCount;
        prefab.Slot = FindEmptySlot(prefab);


        summonCount++;
        summonCount %= summonedMinions.Length;

    }

    DockSlot FindEmptySlot(SummonedMinion prefab)
    {
        for (int i = 0; i < summonTable.dockSlots.Count ; i++)
        {
            if (!summonTable.dockSlots[i].Assigned)
            {

                summonTable.Assign(summonTable.dockSlots[i], prefab);
                return summonTable.dockSlots[i];
            }

        }

        Debug.Log("Forcing slot position");
        return summonTable.dockSlots[summonCount];
    }
}

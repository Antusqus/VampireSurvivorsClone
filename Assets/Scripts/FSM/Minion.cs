using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Unit
{
    public Vector3 dockTarget;
    protected Transform bestTarget;
    protected Rigidbody2D rb;
    public SpellEffect spell;
    public float pollingTime;
    protected int summonNr;
    private DockSlot slot;
    public int SummonNr { get => summonNr; set => summonNr = value; }
    public DockSlot Slot { get => slot; set => slot = value; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        spell = GetComponent<SpellEffect>();
        stateMachine.ChangeState(new DockingState(this));
        pollingTime = 0f;


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    public Transform GetClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, layerMask: LayerMask.GetMask("Enemy"));
        float closestDistanceSqr = Mathf.Infinity;

        Debug.Log(enemies.Length + " Enemies found");
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 directionToTarget = enemies[i].transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = enemies[i].transform;
            }
        }
        Debug.Log(bestTarget + " Target found");
        return bestTarget;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats enemy = other.GetComponent<EnemyStats>();
        BreakableProps prop = other.GetComponent<BreakableProps>();

        if (enemy)
        {

            enemy.TakeDamage(spell.GetDamage(), transform.position);

            Spell.Stats stats = spell.spell.GetStats();

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (prop)
        {
            prop.TakeDamage(spell.GetDamage());

            Spell.Stats stats = spell.spell.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

        }

    }
}

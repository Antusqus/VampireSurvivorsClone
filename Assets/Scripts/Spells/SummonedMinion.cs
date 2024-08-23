using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedMinion : SpellEffect
{
    public StateMachine stateMachine = new StateMachine();
    protected int summonNr;
    private DockSlot slot;
    protected Rigidbody2D rb;
    protected Transform bestTarget;
    protected PlayerStats player;
    public Vector3 dockTarget;
    public float pollingTime;

    Vector3 currentPosition;

    public int SummonNr { get => summonNr; set => summonNr = value; }
    public DockSlot Slot { get => slot; set => slot = value; }

    // Start is called before the first frame update
    protected virtual void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerStats>();

        pollingTime = 0f;
        stateMachine.ChangeState(new DockingState(this));

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentPosition = transform.position;

        if (stateMachine.currentState != null) stateMachine.currentState.Execute();

    }

    public Transform GetClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(owner.transform.position, 10f, layerMask: LayerMask.GetMask("Enemy"));
        float closestDistanceSqr = Mathf.Infinity;

        Debug.Log(enemies.Length + " Enemies found");
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 directionToTarget = enemies[i].transform.position - currentPosition;
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

            enemy.TakeDamage(GetDamage(), transform.position);

            Spell.Stats stats = spell.GetStats();

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (prop)
        {
            prop.TakeDamage(GetDamage());

            Spell.Stats stats = spell.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

        }

    }
}

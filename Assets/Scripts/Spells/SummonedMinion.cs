using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedMinion : SpellEffect
{
    int summonNr;
    float circleposition;
    float angle;
    Vector3 dockPos;
    Vector3 axis;
    protected Rigidbody2D rb;
    Transform bestTarget = null;
    PlayerStats player;
    float pollingTime;
    float closestDistanceSqr;
    Camera cam;

    public int SummonNr { get => summonNr; set => summonNr = value; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerStats>();
        closestDistanceSqr = Mathf.Infinity;

        pollingTime = 0f;
        cam = Camera.main;
        circleposition = summonNr / 6;
        angle = circleposition * Mathf.PI * 2.0f;



        axis = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {


        if (pollingTime <= 0f)
        {
            GetClosestEnemy();
            pollingTime = 3f;
        }

        if (bestTarget)
        {
            transform.position = Vector2.MoveTowards(transform.position, bestTarget.transform.position, player.CurrentMoveSpeed * Time.deltaTime);
        }
        else
        {
            dockPos = new Vector3(Mathf.Sin(angle) * 6, Mathf.Cos(angle) * 6, 0);
            angle += Time.deltaTime * player.CurrentProjectileSpeed;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position + dockPos, player.CurrentMoveSpeed * Time.deltaTime * 3);

        }

        pollingTime -= Time.deltaTime;
    }

    Transform GetClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(cam.transform.position, 10f, layerMask: LayerMask.GetMask("Enemy"));
        Vector3 currentPosition = transform.position;
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

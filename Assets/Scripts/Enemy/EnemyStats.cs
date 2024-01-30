using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    public EnemyScriptableObject enemyData;

    //Current stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;


    public float despawnDistance = 20f;
    Transform player;

    void Awake()
    {
        currentDamage = enemyData.Damage;
        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.MoveSpeed;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.OnEnemyKilled();
        Destroy(gameObject);
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        //Deal damage to player when colliders collided.
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }
}

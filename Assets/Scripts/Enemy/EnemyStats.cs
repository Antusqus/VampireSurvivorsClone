using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : Unit
{

    public EnemyScriptableObject enemyData;

    //Current stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;
    [HideInInspector]
    public float currentCooldown;

    public EnemyProjectile projectilePrefab;
    public EnemyAttackData atkData;

    public float despawnDistance = 20f;
    Transform player;


    [Header("Damage Feedback")]
    public Color dmgColor = new Color(1, 0, 0, 1);
    public float dmgFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;

    void Awake()
    {
        currentDamage = enemyData.Damage;
        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.MoveSpeed;
    }

    protected override void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        movement = GetComponent<EnemyMovement>();

        if (atkData != null && projectilePrefab != null)
        {
            Type atkType = Type.GetType(atkData.behaviour);
            if (atkType != null)
            {
                GameObject go = new GameObject(projectilePrefab.name + " Controller");
                EnemyAttack enemyAttack = (EnemyAttack)go.AddComponent(atkType);
                enemyAttack.Initialise(atkData);
                enemyAttack.transform.SetParent(transform);
                enemyAttack.transform.localPosition = Vector2.zero;

            }
        }


        
    }

    protected override void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());
        //Transform temp = Instantiate(transform, transform.position, Quaternion.identity);
        if (dmg > 0)
        {
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
        }
        if (knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    IEnumerator DamageFlash()
    {
        sr.color = dmgColor;
        yield return new WaitForSeconds(dmgFlashDuration);
        sr.color = originalColor;
    }
    public void Kill()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.OnEnemyKilled();
        Destroy(gameObject, 0.2f);
    }

    IEnumerator KillFade()
    {
        // Waits for a single frame.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();

        if (collision.GetType() == typeof(PolygonCollider2D))
        {
            this.TakeDamage(player.CurrentMight, player.transform.position);

        }
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[UnityEngine.Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }


}

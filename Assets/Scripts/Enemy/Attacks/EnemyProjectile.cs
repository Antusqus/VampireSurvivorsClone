using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to an enemy projectile attack.
/// </summary>
public class EnemyProjectile : EnemyAttackEffect
{
    protected Rigidbody2D rb;
    public PlayerStats target;
    public EnemyAttack.Stats stats;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected virtual void Start()
    {
        target = FindObjectOfType<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        stats = atk.GetStats();
        AcquireAutoAimFacing();

        Physics2D.IgnoreLayerCollision(7,9,true);
        Physics2D.IgnoreLayerCollision(9, 9, true);


        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.velocity = transform.right * stats.speed;
        }

        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);


    }

    protected virtual void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            EnemyAttack.Stats stats = atk.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;

        if (target)
        {
            Vector2 difference = target.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // Point the projectile towards where we aiming at.
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        stats = atk.GetStats();

        if (player)
        {
            player.TakeDamage(stats.damage);
            Destroy(gameObject);
        }

    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        PlayerStats player = other.gameObject.GetComponent<PlayerStats>();
        stats = atk.GetStats();

        if (player)
        {
            player.TakeDamage(stats.damage);
            Destroy(gameObject);
        }

    }

}

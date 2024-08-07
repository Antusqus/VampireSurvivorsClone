using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : SpellEffect
{

    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public bool gravEnabled = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);
    public Vector2 gravityDir = new Vector2(0, -9.81f);



    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        Spell.Stats stats = spell.GetStats();

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }

        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y),
            1
            );

        piercing = stats.piercing;

        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);

        if (hasAutoAim) AcquireAutoAimFacing();
    }

    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        if (targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // Point the projectile towards where we aiming at.
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    protected virtual void FixedUpdate()
    {
        if (gravEnabled)
        {
            rb.AddForce(Physics.gravity * rb.mass);
        }
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Spell.Stats stats = spell.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        if (es)
        {
            // If there is an owner: Calculate knockback using the owner instead of the projectile;
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            es.TakeDamage(GetDamage(), source);

            Spell.Stats stats = spell.GetStats();
            piercing--;

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Spell.Stats stats = spell.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

        }
        if (piercing <= 0) Destroy(gameObject);

    }


    private void OnParticleTrigger()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        Debug.Log("Triggered particle!");

        if (!ps)
        {
            return;
        }


    }
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats es = other.GetComponent<EnemyStats>();

            // If there is an owner: Calculate knockback using the owner instead of the projectile;
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            es.TakeDamage(GetDamage(), source);

            Spell.Stats stats = spell.GetStats();
            piercing--;

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (other.CompareTag("Prop"))
        {
            BreakableProps p = other.GetComponent<BreakableProps>();
            if (!p) return;
            p.TakeDamage(GetDamage());
            piercing--;

            Spell.Stats stats = spell.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

        }
        if (piercing <= 0) Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for enemy attacks. Use in conjunction with enemystats.
/// </summary>
/// 
public class EnemyAttack : MonoBehaviour
{
    protected EnemyStats owner;

    [System.Serializable]

    public struct Stats
    {
        public string name, description;

        [Header("Visual")]
        public EnemyProjectile projectilePrefab; // if attached, projectile will spawn on weapon cooldown.
        //public Aura auraPrefab;
        //public ParticleSystem hitEffect;
        //public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; // 0 = last forever

        public Damage damage;
        public float cooldown, rearmTime;
        public float speed, range;

        public int atkCharges; // Attacks before needing longer cooldown (rearm).

        public Damage GetDamage()
        {
            //Define damage types such that they are apparent from the attack's statsheet.
            //return new Damage(damage + Random.Range(0, damageVariance));
            return damage;
        }
    }

    protected Stats currentStats;
    protected EnemyMovement movement;
    protected EnemyAttackData data;
    protected PlayerStats player;
    protected float currentCooldown;
    protected int currentCharges;
    protected bool isRearming;



    protected virtual void Awake()
    {
        if (data) currentStats = data.baseStats;
    }

    protected virtual void Start()
    {
        if (data)
        {
            Initialise(data);
            player = FindObjectOfType<PlayerStats>();
        }
    }
    // Update is called once per frame
    protected virtual void Update()
    {

        if (Vector2.Distance(player.transform.position, transform.position) < currentStats.range)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f && !isRearming)
            {
                Attack();
            }
        }





    }
    public virtual void Initialise(EnemyAttackData data)
    {
        owner = GetComponentInParent<EnemyStats>();
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<EnemyMovement>();
        currentCooldown = currentStats.cooldown;
        currentCharges = currentStats.atkCharges;
    }

    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            currentCooldown = currentStats.cooldown;

            if (currentCharges > 0)
                currentCharges--;

            return true;
        }

        return false;
    }

    public IEnumerator Rearm()
    {
        isRearming = true;
        while (currentCharges < data.baseStats.atkCharges)
        {
            currentCharges++;
        }
        yield return new WaitForSeconds(data.baseStats.rearmTime);
        isRearming = false;
    }

    public virtual Damage GetDamage()
    {
        return currentStats.GetDamage();
    }

    public virtual Stats GetStats() { return currentStats; }

    //protected virtual float GetSpawnAngle()
    //{
    //    float angle = Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;


    //    return angle;
    //}

}

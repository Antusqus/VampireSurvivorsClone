using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete("This is obsolete and will be replaced.")]

public class ProjectileWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;

    protected Vector3 targetDir;
    public float destroyAfterSeconds;
    readonly float spriteAngleOffset = -45;


    // Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

    public float GetCurrentDamage()
    {
        currentDamage = weaponData.Damage * FindObjectOfType<PlayerStats>().CurrentMight;
        return currentDamage;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);

    }

    public void DirectionChecker(Vector3 dir)
    {
        targetDir = dir;

        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        angle += spriteAngleOffset;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {

            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            ReducePierce();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);
        }

        else if (collision.CompareTag("Prop"))
        {
            if (collision.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                ReducePierce();
                breakable.TakeDamage(GetCurrentDamage());
            }
        }
    }

    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}

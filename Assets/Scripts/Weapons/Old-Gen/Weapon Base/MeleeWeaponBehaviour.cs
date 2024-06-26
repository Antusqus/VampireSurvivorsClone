using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This is obsolete and will be replaced.")]

public class MeleeWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float destroyAfterSeconds;

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

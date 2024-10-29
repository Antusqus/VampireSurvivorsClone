using UnityEngine;


public abstract class EnemyAttackEffect : MonoBehaviour
{
    [HideInInspector] public EnemyStats owner;
    [HideInInspector] public EnemyAttack atk;

    public Damage GetDamage()
    {
        return atk.GetDamage();
    }
}


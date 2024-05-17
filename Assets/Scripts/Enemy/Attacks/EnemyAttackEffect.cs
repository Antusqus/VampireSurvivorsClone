using UnityEngine;


public abstract class EnemyAttackEffect : MonoBehaviour
{
    [HideInInspector] public EnemyStats owner;
    [HideInInspector] public EnemyAttack atk;

    public float GetDamage()
    {
        return atk.GetDamage();
    }
}


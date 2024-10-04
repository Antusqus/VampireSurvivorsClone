using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nightborne : Unit
{

    public Animator am;
    public Transform bestTarget = null;
    public EnemyStats es;
    SpriteRenderer sr;

    const string animBaseLayer = "Base Layer";
    public static int atk1Hash = Animator.StringToHash(animBaseLayer + ".Nightborne_Attack");
    public static int deathHash = Animator.StringToHash(animBaseLayer + ".Nightborne_Death");
    public static int runHash = Animator.StringToHash(animBaseLayer + ".Nightborne_Run");
    Vector3 tempLocalScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        am = GetComponent<Animator>();
        es = GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();
        Idle();
        tempLocalScale = transform.localScale;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        if (bestTarget)
        {
            if (Vector2.Distance(transform.position, bestTarget.transform.position) < es.atkData.baseStats.range)
            {
                stateMachine.ChangeState(new NightborneAtkState(this));
                bestTarget = null;
            }
        }

    }

    protected void Idle()
    {
        stateMachine.ChangeState(new NightborneIdle(this));
    }

    protected void Attack()
    {
        stateMachine.ChangeState(new NightborneAtkState(this));
    }

    public void SpriteDirectionChecker(Vector2 dir)
    {
        if (dir.x < 0 && tempLocalScale.x > 0)
        {
            //sr.flipX = true;
            tempLocalScale.x *= -1;
            transform.localScale = tempLocalScale;
        }
        else if (dir.x > 0 && tempLocalScale.x < 0)
        {
            //sr.flipX = false;
            tempLocalScale.x *= -1;
            transform.localScale = tempLocalScale;

        }
    }
    public IEnumerator WaitForAnim(float animEndPerc = 0.99f)
    {
        // Thread will wait for animation until given float percentage of animation frames have finished.
        while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < animEndPerc)
        {
            yield return null;
        }
    }

    public Transform GetClosestTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, 10f, layerMask: LayerMask.GetMask("Player"));
        float closestDistanceSqr = Mathf.Infinity;

        //Debug.Log(targets.Length + " Targets found");
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 directionToTarget = targets[i].transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = targets[i].transform;
            }
        }

        return bestTarget;
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("Potato: " + collision);
    //    PlayerStats player = collision.GetComponent<PlayerStats>();

    //    if (player)
    //    {
    //        player.TakeDamage(es.currentDamage);
    //        Debug.Log("Slashed");
    //    }
    //}

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        PlayerStats player = other.gameObject.GetComponent<PlayerStats>();

        if (player)
        {
            player.TakeDamage(es.currentDamage);
        }

    }


}

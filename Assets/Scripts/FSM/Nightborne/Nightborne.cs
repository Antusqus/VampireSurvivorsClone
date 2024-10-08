using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nightborne : EnemyStats
{

    //const string animBaseLayer = "Base Layer";
    //public static int atk1Hash = Animator.StringToHash(animBaseLayer + ".Nightborne_Attack");
    //public static int deathHash = Animator.StringToHash(animBaseLayer + ".Nightborne_Death");
    //public static int runHash = Animator.StringToHash(animBaseLayer + ".Nightborne_Run");
    Vector3 tempLocalScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Idle();
        tempLocalScale = transform.localScale;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();



        if (bestTarget)
        {
            if (Vector2.Distance(transform.position, bestTarget.transform.position) < atkData.baseStats.range)
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

    

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStats player = collision.GetComponent<PlayerStats>();

        if (player && !collision.isTrigger)
        {
            player.TakeDamage(atkData.baseStats.GetDamage());
        }

        if (collision.GetType() == typeof(PolygonCollider2D))
        {
            this.TakeDamage(player.CurrentMight, player.transform.position);

        }
    }

    protected override void OnCollisionStay2D(Collision2D other)
    {
        PlayerStats player = other.gameObject.GetComponent<PlayerStats>();

        if (player)
        {
            player.TakeDamage(currentDamage);
        }

    }


}

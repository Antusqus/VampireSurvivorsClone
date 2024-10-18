using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();
    public Animator am;
    public AnimationClip deathAnim;

    protected virtual void Start()
    {
        am = GetComponent<Animator>();
        //stateMachine.ChangeState(new TestState(this));
    }

    protected virtual void Update()
    {
        stateMachine.Update();
    }

    public virtual IEnumerator WaitForDeathAnim(int deathAnimHash = 0, float animEndPerc = 0.96f)
    {
        // Thread will wait for animation until given float percentage of animation frames have finished.
        if (deathAnim)
        {

            if (deathAnimHash != 0)
            {
                while (am.GetCurrentAnimatorStateInfo(0).fullPathHash != deathAnimHash)
                {
                    yield return null;
                }

                while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < animEndPerc)
                {
                    yield return null;
                }
            }

        }
        Destroy(gameObject);

    }
}
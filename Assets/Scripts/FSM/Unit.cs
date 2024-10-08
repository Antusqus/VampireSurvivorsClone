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

    protected virtual IEnumerator WaitForDeathAnim(float animEndPerc = 0.99f)
    {
        // Thread will wait for animation until given float percentage of animation frames have finished.
        if (deathAnim)
        {
            while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < animEndPerc)
            {
                yield return null;
            }
        }

        Destroy(gameObject);

    }
}
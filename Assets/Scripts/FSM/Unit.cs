using UnityEngine;

public class Unit : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();

    protected virtual void Start()
    {
        //stateMachine.ChangeState(new TestState(this));
    }

    protected virtual void Update()
    {
        stateMachine.Update();
    }
}
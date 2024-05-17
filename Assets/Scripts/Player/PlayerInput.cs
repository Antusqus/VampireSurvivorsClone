using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    [Header("References")]
    Rigidbody2D rb;
    PlayerStats player;
    Animator am;

    [HideInInspector]
    public Vector2 moveDir;

    [HideInInspector]
    public float lastHorizontalVector;

    [HideInInspector]
    public float lastVerticalVector;

    [HideInInspector]
    public Vector2 lastMovedVector;

    float slideSpeed;

    bool comboCoroutineRunning;

    const string animBaseLayer = "Base Layer";
    static int atk1Hash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Attack1");
    static int atk2Hash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Attack2");
    static int atk3Hash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Attack3");

    ComboPart p1 = new ComboPart("Combo1", true, atk1Hash);
    ComboPart p2 = new ComboPart("Combo2", false, atk2Hash);
    ComboPart p3 = new ComboPart("Combo3", false, atk3Hash);


    bool continueCombo;

    bool waitForImpact;

    private static int currentClickCount;
    private static int preComboClickCount;

    private State state;
    public enum State
    {
        Normal,
        Rolling,
        Slashing,
        Blocking
    }

    // Start is called before the first frame update
    void Start()

    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        lastMovedVector = new Vector2(1, 0f); //Default projectile direction (right)



        List<ComboPart> comboParts = new List<ComboPart> { p1, p2, p3 };

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.currentState != GameManager.GameState.Gameplay)
        {
            Debug.Log("Game not in gameplay!");
            return;

        }

        switch (state)
        {
            case State.Normal:
                HandleClick();
                Move();
                HandleMovement();
                HandleRoll();
                HandleBlock();
                break;
            case State.Rolling:
                HandleRollSliding();
                break;
            case State.Slashing:
                HandleClick();
                Move();
                HandleMovement();
                HandleComboChain();
                break;
            case State.Blocking:
                HandleBlocking();
                break;

        }

    }
    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.velocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }

    #region Handlers
    private void HandleBlock()
    {
        if (AnimatorIsPlaying("HeroKnight_Run"))
            return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            state = State.Blocking;
            am.SetBool("Blocking", true);
        }


    }

    private void HandleBlocking()
    {
        if (AnimatorIsPlaying("HeroKnight_BlockIdle"))
        {
            StartCoroutine(WaitForImpact());
        }
    }
    void HandleClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            if (p1.waitForInput && !comboCoroutineRunning)
            {
                am.SetBool("Combo1", true);
                am.SetBool("Slashing", true);
                state = State.Slashing;
            }
            currentClickCount++;
        }

    }
    /// <summary>
    /// We want to store our last moved vector for gameplay purposes (e.g. directional attacks).
    /// </summary>
    void HandleMovement()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }


        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f); // Last moved x
        }
        if (moveDir.y != 0)
        {

            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector); // Last moved y

        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector); // While moving
        }

    }

    void HandleRoll()
    {
        if (AnimatorIsPlaying("HeroKnight_Run"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.TakeAction(100);
                state = State.Rolling;
                slideSpeed = 350f;
                am.SetBool("Rolling", true);
            }
        }

    }

    void HandleRollSliding()
    {
        transform.position += new Vector3(moveDir.x * player.CurrentMoveSpeed * Time.deltaTime, moveDir.y * player.CurrentMoveSpeed * Time.deltaTime, 0);
        slideSpeed -= slideSpeed * 10f * Time.deltaTime;
        if (AnimatorIsPlaying("HeroKnight_Roll"))
        {
            StartCoroutine(WaitForAnim());
            if (slideSpeed < 5f)
            {
                state = State.Normal;
                am.SetBool("Rolling", false);

            }
        }

    }

    void HandleComboChain()
    {
        if (comboCoroutineRunning)
            return;

        if (p1.waitForInput)
        {
            StartCoroutine(WaitForComboAnim(p1, p2));
            comboCoroutineRunning = true;
        }

        else if (p2.waitForInput)
        {
            StartCoroutine(WaitForComboAnim(p2, p3));
            comboCoroutineRunning = true;
        }

        else if (p3.waitForInput)
        {
            StartCoroutine(WaitForComboAnim(p3, p1));
            comboCoroutineRunning = true;
        }

    }


    #endregion

    bool AnimatorIsPlaying()
    {
        return am.GetCurrentAnimatorStateInfo(0).length >
               am.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && am.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    private IEnumerator WaitForImpact()
    {

        while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.15f)
        {
            yield return null;
        }
        waitForImpact = true;

        player.GrantIFrames();


        while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.95f)
        {
            yield return null;
        }
        waitForImpact = false;
        am.SetBool("Blocking", false);
        am.SetBool("BlockImpact", false);

        state = State.Normal;


    }
    private IEnumerator BraceImpact()
    {
        transform.position -= new Vector3(lastMovedVector.x, lastMovedVector.y, 0) / 2 * Time.deltaTime;
        am.SetBool("BlockImpact", true);

        player.GrantIFrames();

        while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.95f)
        {
            yield return null;
        }
        waitForImpact = false;
        am.SetBool("Blocking", false);
        am.SetBool("BlockImpact", false);

    }

    private IEnumerator WaitForComboAnim(ComboPart currentSequence, ComboPart followupSequence)
    {
        while (am.GetCurrentAnimatorStateInfo(0).fullPathHash != currentSequence.anim_hash)
        {
            yield return null;
        }

        Debug.Log("Playing anim");
        float waitTime = am.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(waitTime * .20f);
        preComboClickCount = currentClickCount;
        currentSequence.waitForInput = false;
        followupSequence.waitForInput = true;



        yield return new WaitForSeconds(waitTime * .80f);
        am.SetBool(currentSequence.part, false);
        if (continueCombo)
        {
            Debug.Log(string.Format("Chaining: {0}>{1}", currentSequence.part, followupSequence.part));
            am.SetBool(followupSequence.part, true);
        }
        else
        {
            followupSequence.waitForInput = false;
            p1.waitForInput = true;
            currentClickCount = 0;
            preComboClickCount = 0;
        }
        comboCoroutineRunning = false;

    }


    private IEnumerator WaitForAnim(float animEndPerc = 0.99f)
    {
        // Thread will wait for animation until given float percentage of animation frames have finished.
        while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < animEndPerc)
        {
            yield return null;
        }
    }





    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
        EnemyProjectile ep = collision.gameObject.GetComponentInParent<EnemyProjectile>();

        if (enemy || ep)
        {

            if (waitForImpact)
            {
                StartCoroutine(BraceImpact());
            }

        }

        if (enemy && collision.IsTouchingLayers(7))
        {
            Debug.Log("Hit connected!");
            enemy.TakeDamage(player.CurrentMight, enemy.transform.position);
        }

    }

    #region EndStates USE IN UNITY
    /// <summary>
    /// These methods are used in Unity itself, with animation events.
    /// That way we can forcibly exit states after animations run out.
    /// </summary>
    void EndStates()
    {
        foreach (AnimatorControllerParameter parameter in am.parameters)
        {
            am.SetBool(parameter.name, false);
        }
        state = State.Normal;

    }

    void EndOrContinueCombo()
    {
        if (currentClickCount > preComboClickCount)
        {
            continueCombo = true;
            return;
        }

        continueCombo = false;
        am.SetBool("Slashing", false);
        am.SetBool("Combo1", false);
        am.SetBool("Combo2", false);
        am.SetBool("Combo3", false);
        p1.waitForInput = true;
        p2.waitForInput = false;

        p3.waitForInput = false;


        state = State.Normal;
    }


    void EndBlock()
    {
        am.SetBool("BlockImpact", false);
        am.SetBool("Blocking", false);
    }
    #endregion
}


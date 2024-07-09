using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


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
    static int blockIdleHash = Animator.StringToHash(animBaseLayer + ".HeroKnight_BlockIdle");
    static int blockHash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Block");

    ComboPart p1 = new ComboPart("Combo1", true, atk1Hash);
    ComboPart p2 = new ComboPart("Combo2", false, atk2Hash);
    ComboPart p3 = new ComboPart("Combo3", false, atk3Hash);



    [Header("States")]
    bool continueCombo;

    bool perfectBlockcr;

    bool blocking;
    bool rechargingBlock;
    bool braceForImpact;
    bool bracing;
    public bool prepCast;

    [Header("Buttons")]
    private InputAction _moveAction;
    private InputAction _slashAction;
    private InputAction _rollAction;
    private InputAction _blockAction;


    private Grimoire grimoire;


    private static float braceMaxDuration = 10f;
    float braceTimer = braceMaxDuration;


    private static int currentClickCount = 0;
    private static int preComboClickCount = 0;

    public DefaultPlayerActions playerActions;

    private State state;
    public enum State
    {
        Normal,
        Rolling,
        Slashing,
        Blocking,
        Casting
    }

    private void Awake()
    {
        playerActions = new DefaultPlayerActions();
    }

    private void OnEnable()
    {
        _moveAction = playerActions.Player.Move;
        _moveAction.Enable();

        _slashAction = playerActions.Player.Fire;
        _slashAction.performed += HandleClick;


        _slashAction.Enable();

        _rollAction = playerActions.Player.Roll;
        _rollAction.performed += HandleRoll;

        _rollAction.Enable();

        _blockAction = playerActions.Player.Block;
        _blockAction.performed += PerformBlock;
        _blockAction.canceled += CancelBlock;

        _blockAction.Enable();

    }

    private void OnDisable()
    {
        _moveAction.Disable();

        _slashAction.Disable();
        _slashAction.performed -= HandleClick;

        _rollAction.Disable();
        _rollAction.performed -= HandleRoll;

        _blockAction.Disable();
        _blockAction.performed -= PerformBlock;

    }
    // Start is called before the first frame update
    void Start()

    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        lastMovedVector = new Vector2(1, 0f); //Default projectile direction (right)
        perfectBlockcr = false;
        blocking = false;
        rechargingBlock = false;


        List<ComboPart> comboParts = new List<ComboPart> { p1, p2, p3 };
    }




    // Update is called once per frame
    void Update()
    {
        if (GameManager.currentState != GameManager.GameState.Gameplay)
        {
            return;
        }
        if (!blocking && braceTimer < braceMaxDuration)
        {
            braceTimer += Time.deltaTime;
        }

        if (braceTimer <= 0 && !rechargingBlock)
            StartCoroutine(RechargeBlock());
        switch (state)
        {
            case State.Normal:
                Move();
                HandleMovement();
                break;
            case State.Rolling:
                HandleRollSliding();
                break;
            case State.Slashing:
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
    private void PerformBlock(InputAction.CallbackContext context)
    {
        if (AnimatorIsPlaying("HeroKnight_Run"))
            return;

        blocking = true;
        state = State.Blocking;
    }

    private void CancelBlock(InputAction.CallbackContext context)
    {
        if (blocking)
        {
            if (AnimatorIsPlaying("HeroKnight_Block"))
            {
                WaitForAnim();
            }

            EndBlock();

        }

    }


    void HandleBlocking()
    {
        if (blocking)
        {
            braceTimer -= Time.deltaTime;

            if (!perfectBlockcr)
            {
                Debug.Log("starting perfect block");
                perfectBlockcr = true;
                StartCoroutine(PerfectBlock());
            }
        }
    }

    void HandleClick(InputAction.CallbackContext context)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
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

        moveDir = _moveAction.ReadValue<Vector2>();

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

    void HandleRoll(InputAction.CallbackContext context)
    {
        if (AnimatorIsPlaying("HeroKnight_Run"))
        {
            player.TakeStamina(100);
            state = State.Rolling;
            slideSpeed = 350f;
            am.SetBool("Rolling", true);
        }

    }

    void HandleRollSliding()
    {
        transform.position += new Vector3(moveDir.x * player.CurrentMoveSpeed * Time.deltaTime, moveDir.y * player.CurrentMoveSpeed * Time.deltaTime, 0);
        slideSpeed -= slideSpeed * 10f * Time.deltaTime;
        if (AnimatorIsPlaying("HeroKnight_Roll"))
        {
            StartCoroutine(WaitForAnim());
            if (slideSpeed < braceMaxDuration)
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
            comboCoroutineRunning = true;

            StartCoroutine(WaitForComboAnim(p1, p2));
        }

        else if (p2.waitForInput)
        {
            comboCoroutineRunning = true;

            StartCoroutine(WaitForComboAnim(p2, p3));
        }

        else if (p3.waitForInput)
        {
            comboCoroutineRunning = true;

            StartCoroutine(WaitForComboAnim(p3, p1));
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
    private IEnumerator PerfectBlock()
    {
        am.SetBool("Blocking", true);
        while (am.GetCurrentAnimatorStateInfo(0).fullPathHash != blockIdleHash)
        {
            yield return null;
        }


        float waitTime = am.GetCurrentAnimatorStateInfo(0).length;
        float impactMoment = .05f;
        float impactExit = .60f;

        yield return new WaitForSeconds(waitTime * impactMoment);
        braceForImpact = true;
        Debug.Log("Perfect brace");
        player.GrantIFrames(waitTime * (impactExit - impactMoment));

        yield return new WaitForSeconds(waitTime * impactExit);
        braceForImpact = false;
        Debug.Log("Exit brace");
    }

    private IEnumerator BraceImpact(Vector3 dir)
    {
        bracing = true;
        am.SetBool("Blocking", true);
        am.SetBool("BlockImpact", true);
        transform.position += dir * Time.deltaTime;

        Debug.Log(string.Format("Moving {0} to {1}", transform.position, dir));
        while (am.GetCurrentAnimatorStateInfo(0).fullPathHash != blockHash)
        {
            yield return null;
        }

        float waitTime = am.GetCurrentAnimatorStateInfo(0).length;


        Debug.Log("Braced for perfect block");
        yield return new WaitForSeconds(waitTime);

        EndBlock();
        bracing = false;

    }

    private IEnumerator RechargeBlock()
    {
        rechargingBlock = true;
        EndBlock();
        while (braceTimer < braceMaxDuration)
            yield return null;
        rechargingBlock = false;
    }

    private IEnumerator WaitForComboAnim(ComboPart currentSequence, ComboPart followupSequence)
    {
        while (am.GetCurrentAnimatorStateInfo(0).fullPathHash != currentSequence.anim_hash)
        {
            yield return null;
        }

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

    /// <summary>
    /// Check for incoming projectiles and enemy attacks. 
    /// If char is perfect-blocking in the direction of incoming damage, negate damage.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
        EnemyProjectile ep = collision.gameObject.GetComponent<EnemyProjectile>();

        if (enemy || ep)
        {
            if (braceForImpact)
            {
                Vector2 dir = transform.InverseTransformDirection(collision.transform.position);

                // If blocking left and right
                if (CheckIncomingDirection(dir))
                {
                    if (!bracing)
                    {
                        StartCoroutine(BraceImpact(dir));
                    }

                }

            }

        }
    }
    private bool CheckIncomingDirection(Vector2 dir)
    {
        if ((lastHorizontalVector < 0 && dir.x > 0) || (lastHorizontalVector > 0 && dir.x < 0) || lastVerticalVector < 0 && dir.y > 0 || lastVerticalVector > 0 && dir.y < 0)
            return true;
        return false;
    }

    #region EndStates USE IN UNITY
    /// <summary>
    /// These methods can also be used in Unity, called by animation events.
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
        Debug.Log("Endblock called!");
        blocking = false;
        perfectBlockcr = false;

        am.SetBool("BlockImpact", false);
        am.SetBool("Blocking", false);
        state = State.Normal;
    }
    #endregion
}


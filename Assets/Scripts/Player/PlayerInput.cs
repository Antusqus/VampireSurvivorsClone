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
    public Rigidbody2D rb;
    PlayerStats player;
    Animator am;

    //[HideInInspector]
    public Vector2 moveDir;

    [HideInInspector]
    public float lastHorizontalVector;

    [HideInInspector]
    public float lastVerticalVector;

    [HideInInspector]
    public Vector2 lastMovedVector;

    public float slideSpeed;

    public bool comboCoroutineRunning;

    const string animBaseLayer = "Base Layer";
    static int atk1Hash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Attack1");
    static int atk2Hash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Attack2");
    static int atk3Hash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Attack3");
    static int blockIdleHash = Animator.StringToHash(animBaseLayer + ".HeroKnight_BlockIdle");
    static int blockHash = Animator.StringToHash(animBaseLayer + ".HeroKnight_Block");

    public ComboPart p1 = new ComboPart("Combo1", true, atk1Hash, 0);
    public ComboPart p2 = new ComboPart("Combo2", false, atk2Hash, 1);
    public ComboPart p3 = new ComboPart("Combo3", false, atk3Hash, 2);
    public List<ComboPart> comboChain = new List<ComboPart>();


    [Header("States")]
    public bool continueCombo;

    bool perfectBlockcr;
    bool clickHeld;
    bool moveHeld;
    bool blocking;
    bool rechargingBlock;
    bool braceForImpact;
    bool bracing;
    public bool prepCast;

    [Header("Buttons")]
    public InputAction _moveAction;
    public InputAction _slashAction;
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
        Attacking,
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
        _moveAction.performed += HandleMovement;
        _moveAction.canceled += StopMovement;
        _moveAction.Enable();

        _slashAction = playerActions.Player.Fire;
        _slashAction.performed += HandleClick;
        _slashAction.canceled += StopClickHeld;


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
        _moveAction.performed -= HandleMovement;


        _slashAction.Disable();
        _slashAction.performed -= HandleClick;

        _rollAction.Disable();
        _rollAction.performed -= HandleRoll;

        _blockAction.Disable();
        _blockAction.performed -= PerformBlock;
        _blockAction.canceled -= CancelBlock;

    }
    // Start is called before the first frame update
    void Start()

    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        //lastMovedVector = new Vector2(1, 0f); //Default projectile direction (right)
        perfectBlockcr = false;
        blocking = false;
        rechargingBlock = false;
        continueCombo = false;

        player.stateMachine.ChangeState(new PlayerIdleState(player));
        comboChain.Add(p1);
        comboChain.Add(p2);
        comboChain.Add(p3);


    }
    // Update is called once per frame
    void Update()
    {
        HandleComboStart();

        if (moveDir != Vector2.zero)
            transform.position += (Vector3)(moveDir * player.CurrentMoveSpeed * Time.deltaTime);

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

        player.stateMachine.currentState.Execute();
        //switch (state)
        //{
        //    case State.Normal:
        //        Move();
        //        HandleMovement();
        //        break;
        //    case State.Rolling:
        //        HandleRollSliding();
        //        break;
        //    case State.Attacking:
        //        Move();
        //        HandleMovement();
        //        HandleComboChain();
        //        break;
        //    case State.Blocking:
        //        HandleBlocking();
        //        break;
        //}

    }
    //public void Move()
    //{
    //    if (GameManager.instance.isGameOver)
    //    {
    //        return;
    //    }

    //    //rb.velocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    //    //player.stateMachine.ChangeState(new PlayerMoveState(player));
    //    moveDir = _moveAction.ReadValue<Vector2>();
    //    transform.position += (Vector3)(moveDir * player.CurrentMoveSpeed * Time.deltaTime);


    //}
    private void StopMovement(InputAction.CallbackContext context)
    {
        Debug.Log("Stopping");
        moveDir = Vector2.zero;

    }

    #region Handlers

    public void HandleMovement(InputAction.CallbackContext context)
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
    private void PerformBlock(InputAction.CallbackContext context)
    {
        //if (AnimatorIsPlaying("HeroKnight_Run"))
        //    rb.velocity = Vector2.zero;

        //blocking = true;
        player.stateMachine.ChangeState(new PlayerBlockState(player));

    }

    private void CancelBlock(InputAction.CallbackContext context)
    {

        player.stateMachine.ChangeState(new PlayerIdleState(player));

    }


    public void HandleBlocking()
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
        Debug.Log("Clicked");
        clickHeld = true;

        currentClickCount++;
    }

    void StopClickHeld(InputAction.CallbackContext context)
    {
        Debug.Log("Click Released");

        clickHeld = false;
    }
    /// <summary>
    /// We want to store our last moved vector for gameplay purposes (e.g. directional attacks).
    /// </summary>


    void HandleRoll(InputAction.CallbackContext context)
    {
        if (AnimatorIsPlaying("HeroKnight_Run"))
        {
            if (!player.CanSpendStamina(100))
            {
                player.stateMachine.ChangeState(new PlayerIdleState(player));
            }
            else
            {
                player.stateMachine.ChangeState(new PlayerRollingState(player));
            }

        }

    }

    public void HandleRollSliding()
    {
        transform.position += new Vector3(moveDir.x * player.CurrentMoveSpeed * Time.deltaTime, moveDir.y * player.CurrentMoveSpeed * Time.deltaTime, 0);
        slideSpeed -= slideSpeed * 10f * Time.deltaTime;
        if (AnimatorIsPlaying("HeroKnight_Roll"))
        {
            StartCoroutine(WaitForAnim());
            if (slideSpeed < 10f)
            {
                player.stateMachine.ChangeState(new PlayerIdleState(player));
            }
        }

    }

    public void HandleComboStart()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId))
            {

            }
            // was pressed on GUI
            else
            {

                //if clickcount increased, start combat anim
                if (preComboClickCount < currentClickCount && !comboCoroutineRunning)
                {
                    player.stateMachine.ChangeState(new PlayerMeleeEntryState(player, p1));
                }
            }
            // was pressed outside GUI
        }
    }
    public void HandleComboChain(ComboPart part)
    {
        if (comboCoroutineRunning)
            return;

        int nextPartIndex = (part.index + 1) % comboChain.Count;
        ComboPart nextPart = comboChain[nextPartIndex];

        //if (/*part.waitingForInput*/!)
        {
            comboCoroutineRunning = true;

            StartCoroutine(WaitForComboAnim(comboChain[part.index], comboChain[nextPart.index]));

        }

    }

    #endregion

    public bool AnimatorIsPlaying()
    {
        return am.GetCurrentAnimatorStateInfo(0).length >
               am.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    public bool AnimatorIsPlaying(string stateName)
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
        preComboClickCount = currentClickCount;
        Debug.Log("comboCoroutine ACTIVE");
        yield return new WaitUntil(() => am.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !am.IsInTransition(0));

        if (continueCombo)
        {
            Debug.Log(string.Format("Chaining: {0}>{1}", currentSequence.part, followupSequence.part));
            switch (currentSequence.index)
            {
                case 0:
                    player.stateMachine.ChangeState(new PlayerMeleeComboState(player, followupSequence));
                    break;
                case 1:
                    player.stateMachine.ChangeState(new PlayerMeleeFinisherState(player, followupSequence));
                    break;
                case 2:
                    player.stateMachine.ChangeState(new PlayerMeleeEntryState(player, followupSequence));
                    break;
                default:
                    Debug.LogError("SOMETHING WENT WRONG IN COMBOHANDLER");
                    break;
            }
        }

        comboCoroutineRunning = false;
        Debug.Log("comboCoroutine DEACTIVE");
        yield break;

    }


    public IEnumerator WaitForAnim(float animEndPerc = 0.99f)
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
                //if (CheckIncomingDirection(dir))
                //{
                //    if (!bracing)
                //    {
                //        StartCoroutine(BraceImpact(dir));
                //    }

                //}

            }

        }
    }
    //private bool CheckIncomingDirection(Vector2 dir)
    //{
    //    if ((lastHorizontalVector < 0 && dir.x > 0) || (lastHorizontalVector > 0 && dir.x < 0) || lastVerticalVector < 0 && dir.y > 0 || lastVerticalVector > 0 && dir.y < 0)
    //        return true;
    //    return false;
    //}

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
        Debug.Log("END OR CONTINUE");
        if (currentClickCount > preComboClickCount || clickHeld)
        {
            Debug.Log("CONTINUE: \n preComboClickCount: " + preComboClickCount + "\ncurrentClickCount: " + currentClickCount);
            continueCombo = true;
        }
        else
        {
            Debug.Log("END");
            continueCombo = false;
            player.stateMachine.ChangeState(new PlayerIdleState(player));
        }

        //continueCombo = false;

        ////p1.waitingForInput = true;
        ////p2.waitingForInput = false;

        ////p3.waitingForInput = false;


        //player.stateMachine.ChangeState(new PlayerIdleState(player));
    }

    void EndBlock()
    {
        Debug.Log("Endblock called!");
        blocking = false;
        perfectBlockcr = false;

        am.SetBool("BlockImpact", false);
        am.SetBool("Blocking", false);
    }
    #endregion
}


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public abstract class Spell : MonoBehaviour
{

    public Image Icon;
    public TextMeshProUGUI chargeCounter;
    private DefaultPlayerActions playerActions;
    private InputAction _castAction;

    protected PlayerStats owner;
    protected PlayerInput ownerInput;

    [System.Serializable]
    public struct Stats
    {
        public string name, description;
        public int spellSlot;

        [Header("Visual")]
        public SpellProjectile projectilePrefab; // if attached, projectile will spawn on weapon cooldown.
        public Aura auraPrefab;
        public SummonedMinion minionPrefab;

        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; // 0 = last forever
        public float damage, damageVariance, area, speed, cooldown, castInterval, knockback, manacost, castSpeed;
        public int maxCharges, piercing, maxInstances;

        // Allows the + operator to add 2 Stats together. Important for increasing weapon stats
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.castSpeed = s1.castSpeed + s2.castSpeed;

            result.cooldown = s1.cooldown + s2.cooldown;
            result.maxCharges = s1.maxCharges + s2.maxCharges;
            result.piercing = s1.piercing + s2.piercing;
            result.castInterval = s1.castInterval + s2.castInterval;
            result.knockback = s1.knockback + s2.knockback;
            result.manacost = s1.manacost + s2.manacost;
            return result;
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected Stats currentStats;
    public SpellData data;
    private bool buttonPressed;
    protected float currentCooldown;
    protected int currentCharges;
    protected bool isRearming;
    protected float currentCastInterval;


    protected virtual void Awake()
    {
        playerActions = new DefaultPlayerActions();
        if (data) currentStats = data.baseStats;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        owner = GetComponentInParent<PlayerStats>();
        ownerInput = GetComponentInParent<PlayerInput>();
        currentCooldown = 0;
        currentCharges = currentStats.maxCharges;
        chargeCounter.text = currentCharges.ToString();

    }

    private void OnEnable()
    {
        _castAction = playerActions.FindAction(Icon.name);
        _castAction.performed += PerformSpell;
        _castAction.canceled += ReleaseSpell;
        _castAction.Enable();
    }

    private void OnDisable()
    {
        _castAction.performed -= PerformSpell;
        _castAction.canceled -= ReleaseSpell;
        _castAction.Disable();

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        chargeCounter.text = currentCharges.ToString();

        if (buttonPressed)
        {
            if (currentCooldown <= 0f && currentCharges == currentStats.maxCharges)
            {
                Cast();

            }

            if (currentCastInterval > 0)
            {
                currentCastInterval -= Time.deltaTime;
                if (currentCastInterval <= 0)
                {
                    Cast();
                }

            }
        }


        if (currentCharges <= 0 && !isRearming)
            StartCoroutine(Rearm());

        if (currentCooldown >= 0f)
            currentCooldown -= Time.deltaTime;
        Icon.fillAmount = IconFillAmount();
    }

    void PerformSpell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (context.interaction is PressInteraction)
            {
                Debug.Log("Pressed");
                Cast();
            }

            if (context.interaction is HoldInteraction)
            {
                Debug.Log("Held");
                buttonPressed = true;
            }

            
        }
    }

    void ReleaseSpell(InputAction.CallbackContext context)
    {
        buttonPressed = false;
    }

    float IconFillAmount()
    {
        float fill = 1 - (currentCooldown / currentStats.cooldown);
        return fill;
    }

    protected virtual void Cast()
    {
        owner.TakeMana(currentStats.manacost);
    }
    public virtual bool CanCast()
    {
        return currentCooldown <= 0f && !isRearming && (currentStats.manacost <= owner.CurrentMana);
    }

    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.CurrentMight;
    }

    public IEnumerator Rearm()
    {
        isRearming = true;
        Debug.Log("Rearming spell");
        currentCooldown = data.baseStats.cooldown;

        while (currentCharges < data.baseStats.maxCharges)
        {
            yield return new WaitForSeconds(data.baseStats.cooldown / data.baseStats.maxCharges);
            currentCharges++;
        }
        isRearming = false;
    }

    public virtual Stats GetStats() { return currentStats; }
}

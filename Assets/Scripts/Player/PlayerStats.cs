using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : Unit
{

    public CharacterData charData;
    public CharacterData.Stats baseStats;
    public PlayerInput input;

    [SerializeField] CharacterData.Stats actualStats;


    //Current stats
    float health;
    float stamina;
    float mana;

    #region Current Stat Properties

    public ParticleSystem damageEffect;
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text =
                        string.Format("Health: {0} / {1} ",
                         health, actualStats.maxHealth);
                }
            }
        }
    }

    public float MaxHealth
    {
        get { return actualStats.maxHealth; }
        // Trying to set max health updates the UI on pause screen too.
        set
        {
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text =
                        string.Format("Health: {0} / {1} ",
                         health, actualStats.maxHealth);
                }
            }
        }
    }

    public float CurrentStamina
    {
        get { return stamina; }
        set
        {
            if (stamina != value)
            {
                stamina = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentStaminaDisplay.text =
                        string.Format("Stamina: {0} / {1} ",
                         stamina, actualStats.maxStamina);
                }
            }
        }
    }

    public float MaxStamina
    {
        get { return actualStats.maxStamina; }
        // Trying to set max stamina updates the UI on pause screen too.
        set
        {
            if (actualStats.maxStamina != value)
            {
                actualStats.maxStamina = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentStaminaDisplay.text =
                        string.Format("Stamina: {0} / {1} ",
                         stamina, actualStats.maxStamina);
                }
            }
        }
    }

    public float CurrentMana
    {
        get { return mana; }
        set
        {
            if (mana != value)
            {
                mana = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentManaDisplay.text =
                        string.Format("Mana: {0} / {1} ",
                         mana, actualStats.maxMana);
                }
            }
        }
    }

    public float MaxMana
    {
        get { return actualStats.maxMana; }
        // Trying to set max stamina updates the UI on pause screen too.
        set
        {
            if (actualStats.maxMana != value)
            {
                actualStats.maxMana = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentManaDisplay.text =
                        string.Format("Mana: {0} / {1} ",
                         mana, actualStats.maxMana);
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }
        set { Recovery = value; }
    }

    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
                }
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }

    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "MoveSpeed: " + actualStats.moveSpeed;
                }
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }

    public float Might
    {
        get { return actualStats.might; }
        set
        {
            if (actualStats.might != value)
            {
                actualStats.might = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + actualStats.might;
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return ProjSpeed; }
        set { ProjSpeed = value; }
    }

    public float ProjSpeed
    {

        get { return actualStats.speed; }
        set
        {
            if (actualStats.speed != value)
            {
                actualStats.speed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Proj. Speed: " + actualStats.speed;
                }
            }
        }
    }
    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }

    public float Magnet
    {

        get { return actualStats.magnet; }
        set
        {
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
                }
            }
        }
    }

    public int CurrentMaxSummons
    {
        get { return MaxSummons; }
        set { MaxSummons = value; }
    }

    public int MaxSummons
    {

        get { return actualStats.maxSummons; }
        set
        {
            if (actualStats.maxSummons != value)
            {
                actualStats.maxSummons = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMaxSummonsDisplay.text = "Max Summons: " + actualStats.maxSummons;
                }
            }
        }
    }


    #endregion

    [Header("Experience/Level")]
    public int exp = 0;
    public int lvl = 1;
    public int expCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLvl;
        public int endLvl;
        public int expCapIncrease;
    }

    public List<LevelRange> levelRanges;

    PlayerCollector collector;
    PlayerInventory inventory;

    public int weaponIndex;
    public int passiveItemIndex;

    [Header("Damage Feedback")]
    public Color dmgColor = new Color(1, 0, 0, 1);
    public float dmgFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    Color originalColor;
    SpriteRenderer sr;

    //iframes
    [Header("iframes")]
    public float iframeDuration;
    public float iframeTimer;
    public bool isInvincible;

    [Header("UI")]
    public Image healthBar;
    public Image staminaBar;
    public Image manaBar;
    public Image expBar;
    public TextMeshProUGUI lvlDisplay;

    public SummonTable summonTable;
    void Awake()
    {
        Physics2D.IgnoreLayerCollision(6, 6); // Allow player (6) layer to ignore terrain layer (7) props

        //Physics2D.IgnoreLayerCollision(6, 7); //

        charData = CharacterSelector.GetData();

        if (CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();
        am = GetComponent<Animator>();

        baseStats = actualStats = charData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
        stamina = actualStats.maxStamina;
    }

    // Start is called before the first frame update
    protected override void Start()
    {

        inventory.Add(charData.StartingWeapon);
        expCap = levelRanges[0].expCapIncrease;

        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
        GameManager.instance.currentStaminaDisplay.text = "Stamina: " + CurrentStamina;
        GameManager.instance.currentManaDisplay.text = "Mana: " + CurrentMana;


        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "MoveSpeed: " + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Proj. Speed: " + CurrentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;
        GameManager.instance.currentMaxSummonsDisplay.text = "MaxSummons: " + CurrentMaxSummons;


        GameManager.instance.AssignChosenCharUI(charData);

        UpdateHealthBar();
        UpdateStaminaBar();
        UpdateManaBar();
        UpdateExpBar();
        UpdateLvlDisplay();

        sr = GetComponent<SpriteRenderer>();
        input = GetComponent<PlayerInput>();
        originalColor = sr.color;

    }

    protected override void Update()
    {
        HandleIFrames();
        Recover();
        RecoverStamina();
        RecoverMana();
    }

    public void HandleIFrames()
    {

        if (iframeTimer > 0)
        {
            iframeTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }

        collector.SetRadius(actualStats.magnet);

    }

    public void IncreaseExp(int amount)
    {
        exp += amount;
        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if (exp >= expCap)
        {
            lvl++;
            exp -= expCap;

            int expCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (lvl >= range.startLvl && lvl <= range.endLvl)
                {
                    expCapIncrease = range.expCapIncrease;
                    break;
                }
            }
            expCap += expCapIncrease;

            UpdateLvlDisplay();
            GameManager.instance.StartLevelUp();
        }
    }

    public virtual void TakeDamage(Damage dmg)
    {
        //Resistance should account for damage type. Means the incoming attack's type should be known before taking dmg.


        //Player takes damage if not invincible
        if (!isInvincible)
        {
            StartCoroutine(DamageFlash());

            if (dmg.SumTotal() == 0)
                return;

            float incomingDmg = CalcIncomingDamage(dmg);
            GameManager.GenerateFloatingText(Mathf.FloorToInt(incomingDmg).ToString(), transform);
            CurrentHealth -= incomingDmg;


            if (damageEffect)
            {
                Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 1f);
            }

            GrantIFrames();

            UpdateHealthBar();
            if (CurrentHealth <= 0)
            {
                Kill();
            }


        }

    }

    public float CalcIncomingDamage(Damage dmg)
    {
        DamageResistances res = actualStats.defences;
        float dmgMulti = 1.1f;
        float tempS, tempP, tempB, tempF, tempC, tempL, tempA, totalDmg;

        switch (res._ArmourType)
        {
            case DamageResistances.ArmourType.None:
                tempS = dmg.Slash.dmg * dmgMulti;
                tempP = dmg.Pierce.dmg * dmgMulti;
                tempB = dmg.Blunt.dmg * dmgMulti;
                break;
            case DamageResistances.ArmourType.Light:
                tempS = dmg.Slash.dmg * dmgMulti;
                break;
            case DamageResistances.ArmourType.Medium:
                tempP = dmg.Pierce.dmg * dmgMulti;
                break;
            case DamageResistances.ArmourType.Heavy:
                tempB = dmg.Blunt.dmg * dmgMulti;
                break;
        }
        float dmgFormula = 1f - ((0.052f * res.Armour) / (0.9f + 0.048f * Math.Abs(res.Armour)));
        tempS = dmg.Slash.dmg * dmgFormula;
        tempP = dmg.Pierce.dmg * dmgFormula;
        tempB = dmg.Blunt.dmg * dmgFormula;


        tempF = dmg.Fire.dmg * (1 - res.Fire / 100);
        tempC = dmg.Cold.dmg * (1 - res.Cold / 100);
        tempL = dmg.Lightning.dmg * (1 - res.Lightning / 100);
        tempA = dmg.Arcane.dmg * (1 - res.Arcane / 100);

        totalDmg = tempS + tempP + tempB + tempF + tempC + tempL + tempA;
        Debug.Log("Temp Damage: " + totalDmg);
        return totalDmg;
    }

    public bool CanSpendStamina(float staminaCost)
    {
        if (staminaCost == 0 || staminaCost > CurrentStamina)
        {
            Debug.Log("Not enough stamina!");
            return false;
        }

        CurrentStamina -= staminaCost;

        UpdateStaminaBar();
        return true;
    }

    public bool CanSpendMana(float manaCost)
    {
        if (manaCost == 0)
            return false;

        if (CurrentMana >= manaCost)
            CurrentMana -= manaCost;

        UpdateManaBar();
        return true;
    }

    /// <summary>
    /// Grants the player the given amount of iframes in seconds as a float.
    /// </summary>
    /// <param name="iframes"></param>
    public void GrantIFrames(float iframes = 0)
    {
        if (iframes > 0)
            iframeTimer = iframes;
        else
            iframeTimer = iframeDuration;

        isInvincible = true;
    }

    IEnumerator DamageFlash()
    {
        sr.color = dmgColor;
        yield return new WaitForSeconds(dmgFlashDuration);
        sr.color = originalColor;
    }
    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    void UpdateStaminaBar()
    {
        staminaBar.fillAmount = CurrentStamina / actualStats.maxStamina;
    }
    void UpdateManaBar()
    {
        manaBar.fillAmount = CurrentMana / actualStats.maxMana;
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)exp / expCap;
    }

    void UpdateLvlDisplay()
    {
        lvlDisplay.text = "Lv " + lvl.ToString();
    }

    public void Kill()
    {
        if (deathAnim)
        {
            Debug.Log(deathAnim.name + " Playing.");
            am.Play(deathAnim.name);
            StartCoroutine(WaitForDeathAnim());

        }



    }
    public override IEnumerator WaitForDeathAnim(int deathAnimHash = 0, float animEndPerc = 0.99f)
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
            }

            while (am.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < animEndPerc)
            {
                yield return null;
            }

        }

        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(lvl);
            GameManager.instance.AssignChosenWeaponsAndPassivesUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }

    }
    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }

        UpdateHealthBar();

    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            UpdateHealthBar();

        }

    }
    void RecoverStamina()
    {

        {
            if (CurrentStamina < actualStats.maxStamina)
            {
                CurrentStamina += CurrentRecovery * 100 * Time.deltaTime;
                if (CurrentStamina > actualStats.maxStamina)
                {
                    CurrentStamina = actualStats.maxStamina;
                }
                UpdateStaminaBar();
            }

        }
    }

    void RecoverMana()
    {
        {
            if (CurrentMana < actualStats.maxMana)
            {
                CurrentMana += CurrentRecovery * 100 * Time.deltaTime;
                if (CurrentMana > actualStats.maxMana)
                {
                    CurrentMana = actualStats.maxMana;
                }
                UpdateManaBar();
            }

        }
    }


    [System.Obsolete("Old function that is kept to maintain compatibility with inv. manager. Removed soon.")]
    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("Inventory slot already full");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); //Set weapon as child of player
        Debug.Log($"Adding {weapon.name} to {weaponIndex}");

        //inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());
        weaponIndex++;
    }

    [System.Obsolete("No need to spawn passive items directly now.")]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveSlots.Count - 1)
        {
            Debug.LogError("Inventory slot already full");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //Set weapon as child of player
        //inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());
        passiveItemIndex++;
    }

}

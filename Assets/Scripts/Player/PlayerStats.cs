using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour
{

    public CharacterData charData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;


    //Current stats
    float health;

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
                if (GameManager.instance !=  null)
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

    PlayerInventory inventory;

    public int weaponIndex;
    public int passiveItemIndex;

    //iframes
    [Header("iframes")]
    public float iframeDuration;
    float iframeTimer;
    bool isInvincible;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI lvlDisplay;


    void Awake()
    {

        Physics2D.IgnoreLayerCollision(6, 7); // Allow player (6) layer to ignore terrain layer (7) props

        charData = CharacterSelector.GetData();

        if(CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();

        baseStats = actualStats = charData.stats;
        health = actualStats.maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {

        inventory.Add(charData.StartingWeapon);
        expCap = levelRanges[0].expCapIncrease;

        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth; 
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "MoveSpeed: " + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Proj. Speed: " + CurrentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

        GameManager.instance.AssignChosenCharUI(charData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLvlDisplay();
    }

    void Update()
    {
        if (iframeTimer > 0)
        {
            iframeTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
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

    public void TakeDamage(float dmg)
    {
        //Player takes damage if not invincible
        if (!isInvincible)
        {
            CurrentHealth -= dmg;

            if(damageEffect)
            {
                Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
            }

            iframeTimer = iframeDuration;
            isInvincible = true;

            UpdateHealthBar();
            if (CurrentHealth <= 0)
            {
                Kill();
            }

            
        }

    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
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

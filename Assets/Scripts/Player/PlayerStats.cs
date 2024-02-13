using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour
{

    public CharacterScriptableObject charData;

    //Current stats
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stat Properties
   
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.instance !=  null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "MoveSpeed: " + currentMoveSpeed;
                }
            }
        }
    }
    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            if (currentMight != value)
            {
                currentMight = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Proj. Speed: " + currentProjectileSpeed;
                }
            }
        }
    }
    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
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

    InventoryManager inventory;

    public int weaponIndex;
    public int passiveItemIndex;

    public GameObject wep2, passiveitemtest1, passiveitemtest2;
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

        charData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();




        CurrentHealth = charData.MaxHealth;
        CurrentRecovery = charData.Recovery;
        CurrentMoveSpeed = charData.MoveSpeed;
        CurrentMight = charData.Might;
        CurrentProjectileSpeed = charData.ProjectileSpeed;
        CurrentMagnet = charData.Magnet;

        SpawnWeapon(charData.StartingWeapon);
        //SpawnWeapon(wep2);

        //SpawnPassiveItem(passiveitemtest1);
        SpawnPassiveItem(passiveitemtest2);


    }

    // Start is called before the first frame update
    void Start()
    {
        expCap = levelRanges[0].expCapIncrease;

        GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth; 
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "MoveSpeed: " + currentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Proj. Speed: " + currentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;

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
        healthBar.fillAmount = currentHealth / charData.MaxHealth;
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
            GameManager.instance.AssignChosenWeaponsAndPassivesUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.instance.GameOver();
        }

    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < charData.MaxHealth)
        {
            CurrentHealth += amount;

            if (CurrentHealth > charData.MaxHealth)
            {
                CurrentHealth = charData.MaxHealth;
            }
        }


    }

    void Recover()
    {
        if (CurrentHealth < charData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            if (CurrentHealth > charData.MaxHealth)
            {
                CurrentHealth = charData.MaxHealth;
            }
        }

    }

    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("Inventory slot already full");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); //Set weapon as child of player
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());
        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("Inventory slot already full");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //Set weapon as child of player
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());
        passiveItemIndex++;
    }

}

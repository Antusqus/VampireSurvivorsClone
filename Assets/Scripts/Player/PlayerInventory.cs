using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]

    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player.", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty() { return item == null; }
    }

    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]

    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassives = new List<PassiveData>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    public PlayerStats player;

    private void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    public bool Has(ItemData type) { return Get(type); }
    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
                return p;
        }

        return null;
    }

    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data == type)
                return w;
        }

        return null;
    }

    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for (int i=0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        if (removeUpgradeAvailability) availablePassives.Remove(data);

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data == data)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailability);
        else if (data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailability);
        return false;
    }

    public int Add(WeaponData data)
    {
        int slotNum = -1;

        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            weaponSlots[slotNum].Assign(spawnedWeapon);

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
                GameManager.instance.EndLevelUp();

            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}.", data.name));
        }

        return -1;
    }

    public int Add(PassiveData data)
    {
        int slotNum = -1;

        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) return slotNum;

        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform);
        p.transform.localPosition = Vector2.zero;

        passiveSlots[slotNum].Assign(p);

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
        return slotNum;
    }

    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is PassiveData) return Add(data as PassiveData);
        return -1;

    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", weapon.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive p = passiveSlots[slotIndex].item as Passive;
            if (!p.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", p.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    void ApplyUpgradeOptions(bool test = false)
    {

        Debug.Log("Applying upgrade options!");
        try
        {


            // For every upgrade button, loop through our available upgrade options. Then apply either a level up or item addition to the player inventory.

            List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
            List<PassiveData> availablePassiveItemUpgrades = new List<PassiveData>(availablePassives);

            foreach (UpgradeUI upgradeOption in upgradeUIOptions)
            {

                Debug.Log("Randomizing options!");

                if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
                    return;

                int upgradeType;

                if (availableWeaponUpgrades.Count == 0)
                {
                    upgradeType = 2;
                }
                else if (availablePassiveItemUpgrades.Count == 0)
                {

                    upgradeType = 1;
                }
                else
                {

                    upgradeType = UnityEngine.Random.Range(1, 3);
                }

                if (upgradeType == 1)
                {

                    WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                    availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                    if (chosenWeaponUpgrade != null)
                    {
                        EnableUpgradeUI(upgradeOption);

                        bool isLevelUp = false;
                        for (int i = 0; i < weaponSlots.Count; i++)
                        {
                            Weapon w = weaponSlots[i].item as Weapon;
                            if (w != null && w.data == chosenWeaponUpgrade)
                            {
                                // Don't upgrade if weapon is max level

                                if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    isLevelUp = true;
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, w.currentLevel + 1));
                                Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                                upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                                upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                                isLevelUp = true;
                                break;
                            }
                        }

                        if (!isLevelUp)
                        {


                            upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
                            upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                            upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;

                        }
                    }
                }
                else if (upgradeType == 2)
                {

                    PassiveData chosenPassiveUpgrade = availablePassiveItemUpgrades[UnityEngine.Random.Range(0, availablePassiveItemUpgrades.Count)];
                    availablePassiveItemUpgrades.Remove(chosenPassiveUpgrade);

                    if (chosenPassiveUpgrade != null)
                    {
                        EnableUpgradeUI(upgradeOption);

                        bool isLevelUp = false;

                        // For every passive we have, hook an event listener to the assigned Unity button that upgrades our item when clicked.
                        for (int i = 0; i < passiveSlots.Count; i++)
                        {

                            Passive p = passiveSlots[i].item as Passive;
                            if (p != null && p.data == chosenPassiveUpgrade)
                            {
                                // Don't upgrade if item is max level
                                if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    isLevelUp = false;
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, p.currentLevel + 1));
                                Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                                upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                                upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                                upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                                isLevelUp = true;
                            }
                        }

                        if (!isLevelUp)
                        {

                            upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
                            upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveUpgrade.baseStats.description;
                            upgradeOption.upgradeNameDisplay.text = chosenPassiveUpgrade.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;

                        }
                    }
                }
            }
        }

        catch (System.ArgumentOutOfRangeException e)
        {
            Debug.LogError(e.Data.Values + e.Message + " potato");
        }
    }
        
    

    void RemoveUpgradeOptions()
    {
        foreach ( UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiliPepper : Pickup
{
    public WeaponData data;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        PlayerStats player = FindObjectOfType<PlayerStats>();
        Type weaponType = Type.GetType(data.behaviour);

        GameObject go = new GameObject(name + " Controller");
        Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
        spawnedWeapon.Initialise(data);
        spawnedWeapon.transform.SetParent(player.transform);
        spawnedWeapon.transform.localPosition = new Vector2(0,90);
        spawnedWeapon.OnEquip();
        Destroy(go, 12);
    }
}

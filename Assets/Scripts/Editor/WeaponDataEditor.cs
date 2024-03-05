using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{

    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    void OnEnable()
    {
        // Cache the weapon data value.
        weaponData = (WeaponData)target;

        // Retrieve all weapon subtypes and cache it.
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubtypes = subTypesString.ToArray();

        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes, weaponData.behaviour));
    }

    public override void OnInspectorGUI()
    {
        //Draw dropdown in Inspector
        selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);

        if (selectedWeaponSubtype > 0)
        {
            weaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(weaponData);
            DrawDefaultInspector();
        }
    }
}

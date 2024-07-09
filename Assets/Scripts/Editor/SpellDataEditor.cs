using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(SpellData))]
public class SpellDataEditor : Editor
{

    SpellData spellData;
    string[] spellSubtypes;
    int selectedSpellSubtype;

    void OnEnable()
    {
        // Cache the spell data value.
        spellData = (SpellData)target;

        // Retrieve all spell subtypes and cache it.
        System.Type baseType = typeof(Spell);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        spellSubtypes = subTypesString.ToArray();

        selectedSpellSubtype = Math.Max(0, Array.IndexOf(spellSubtypes, spellData.behaviour));
    }

    public override void OnInspectorGUI()
    {
        //Draw dropdown in Inspector
        selectedSpellSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedSpellSubtype), spellSubtypes);

        if (selectedSpellSubtype > 0)
        {
            spellData.behaviour = spellSubtypes[selectedSpellSubtype].ToString();
            EditorUtility.SetDirty(spellData);
            DrawDefaultInspector();
        }
    }
}

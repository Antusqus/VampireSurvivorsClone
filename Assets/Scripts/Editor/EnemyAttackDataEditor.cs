using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(EnemyAttackData))]
public class EnemyAttackDataEditor : Editor
{

    EnemyAttackData enemyAttackData;
    string[] attackSubtypes;
    int selectedAttackSubtype;

    void OnEnable()
    {
        // Cache the enemy attack data value.
        enemyAttackData = (EnemyAttackData)target;

        // Retrieve all enemy attack subtypes and cache it.
        System.Type baseType = typeof(EnemyAttack);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        attackSubtypes = subTypesString.ToArray();

        selectedAttackSubtype = Math.Max(0, Array.IndexOf(attackSubtypes, enemyAttackData.behaviour));
    }

    public override void OnInspectorGUI()
    {
        //Draw dropdown in Inspector
        selectedAttackSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedAttackSubtype), attackSubtypes);

        if (selectedAttackSubtype > 0)
        {
            enemyAttackData.behaviour = attackSubtypes[selectedAttackSubtype].ToString();
            EditorUtility.SetDirty(enemyAttackData);
            DrawDefaultInspector();
        }
    }
}

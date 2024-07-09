using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Grimoire : MonoBehaviour, IPointerClickHandler
{
    public List<SpellData> spells = new List<SpellData>();
    GameObject canvas;


    public void Start()
    {
        PopulateList();
        canvas = GameObject.Find("Canvas/Grimoire/GrimoireUI");
        canvas.SetActive(false);

        foreach (SpellData spell in spells)
        {
            if (spell.icon)
            {

                GameObject newImageObj = new GameObject(spell.icon.name);
                newImageObj.transform.SetParent(canvas.transform);

                RectTransform trans = newImageObj.AddComponent<RectTransform>();
                trans.transform.SetParent(newImageObj.transform); // setting parent
                trans.localScale = Vector3.one;

                Image icon = newImageObj.AddComponent<Image>();
                icon.sprite = spell.icon;
                newImageObj.AddComponent<SpellOnClick>();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        canvas.SetActive(!canvas.activeInHierarchy);
    }

    void PopulateList()
    {
        string[] assetNames = AssetDatabase.FindAssets("t:SpellData", new[] { "Assets/Scriptable Objects/Spells/Schools" });

        spells.Clear();
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var character = AssetDatabase.LoadAssetAtPath<SpellData>(SOpath);

            if(!spells.Contains(character))
                spells.Add(character);

        }
    }
}

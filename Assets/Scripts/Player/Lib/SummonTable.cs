using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonTable : MonoBehaviour
{
    public List<DockSlot> dockSlots;
    PlayerStats player;
    GameObject harbour;

    private float circleSlices;
    private float angle;

    void Start()
    {
        harbour = GameObject.Find("SummonTable");

        player = FindObjectOfType<PlayerStats>();
        dockSlots = new List<DockSlot>(player.MaxSummons);
        CreateDockSlots();
    }

    private void Update()
    {
        harbour.transform.RotateAround(player.transform.localPosition, Vector3.forward, 30 * Time.deltaTime);

    }


    Vector3 DockPos(float radius, float angle)
    {
        Vector3 pos;
        //dockPos = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);

        pos.x = player.transform.position.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = player.transform.position.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        pos.z = player.transform.position.z;

        return pos;
    }

    void CreateDockSlots()
    {
        circleSlices = 360 / player.CurrentMaxSummons;
        harbour.transform.SetParent(player.transform);

        for (int i = 0; i < player.CurrentMaxSummons; i++)
        {
            angle = i * circleSlices;
            GameObject so = new GameObject("Dockslot: " + i);

            so.AddComponent<SphereCollider>();
            so.transform.SetParent(harbour.transform);
            DockSlot slot = new(_slotNr: i, _dockSlot: so);
            
            so.transform.position = DockPos(radius: player.CurrentMaxSummons, angle);
            dockSlots.Add(slot);
        }

    }

    public DockSlot GoNext(DockSlot currentSlot, SummonedMinion minion)
    {
        DockSlot nextSlot;
        nextSlot = dockSlots[(currentSlot.nr + 1) % (dockSlots.Count)];

        Unassign(currentSlot);
        Assign(nextSlot, minion);
        return nextSlot;

    }
    public void Assign(DockSlot slot, SummonedMinion minion)
    {
        slot.Assigned = true;
        slot.Minion = minion;
    }

    public void Unassign(DockSlot slot)
    {
        slot.Assigned = false;
        slot.Minion = null;
    }
}


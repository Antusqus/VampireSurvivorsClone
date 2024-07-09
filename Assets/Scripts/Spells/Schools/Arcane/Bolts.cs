using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bolts : SpellProjectile
{
    Vector3 mousePos;

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Spell.Stats stats = spell.GetStats();
        float step = stats.speed * Time.fixedDeltaTime;


        if (transform.position != mousePos)
        {
            transform.position = Vector2.MoveTowards(transform.position, mousePos, step);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }
}

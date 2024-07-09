using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flux : SpellProjectile
{
    Vector3 mousePos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {

        Spell.Stats stats = spell.GetStats();
        float step = stats.speed * Time.fixedDeltaTime;


        if (transform.position != mousePos)
        {
            transform.position = Vector2.MoveTowards(transform.position, mousePos, step);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }
}

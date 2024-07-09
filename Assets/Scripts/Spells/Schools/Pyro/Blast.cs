using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blast : SpellProjectile
{
    Vector3 mousePos;

    protected override void Start()
    {
        base.Start();
        mousePos = (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position).normalized;


        //StartCoroutine(BlastRoutine());
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {


    }

    //private IEnumerator BlastRoutine()
    //{
    //    yield return new WaitForSeconds(10);
    //    Spell.Stats stats = spell.GetStats();


    //    transform.position += stats.speed * Time.deltaTime * mousePos;
    //    transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
    //}
}

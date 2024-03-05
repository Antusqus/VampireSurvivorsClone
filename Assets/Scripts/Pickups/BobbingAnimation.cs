using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; // Bobber
    public float magnitude; // Bobber Strength
    public Vector3 direction;
    Vector3 initialPosition;

    Pickup pickup;

    private void Start()
    {
        pickup = GetComponent<Pickup>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (pickup && !pickup.hasBeenCollected)
        {
            // Sine function for bobber

            transform.position = initialPosition + magnitude * Mathf.Sin(Time.time * frequency) * direction;

        }
    }
}
    
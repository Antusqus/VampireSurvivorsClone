using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{

    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if collided gameobject has collectible interface.
        if(collision.gameObject.TryGetComponent(out ICollectible collectible))
        {
            //Pulling animation
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(forceDirection * pullSpeed);

            collectible.Collect();
        }
    }
}

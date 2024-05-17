using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool stationary = false;
    EnemyStats enemy;
    Transform player;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    // Start is called before the first frame update
    void Start()
    {
        stationary = true;
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerInput>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            transform.position = MoveVector();

        }
    }

    public Vector2 MoveVector()
    {
        if (!stationary)
            return Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);

        return gameObject.transform.position;
    }
    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}

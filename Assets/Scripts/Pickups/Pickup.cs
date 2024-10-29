using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is used for item pickups in player's collection radius.
/// </summary>
public class Pickup : MonoBehaviour
{
    public float lifespan = 0.5f;
    protected PlayerStats target;
    protected float speed;
    Vector2 initialPosition;
    float initialOffset;


    [System.Serializable]
    public struct BobbingAnimation
    {
        public float freq;
        public Vector2 dir;
    }

    public BobbingAnimation bobAnim = new BobbingAnimation
    {
        freq = 2f, dir = new Vector2(0, 0.3f)
    };
    
    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobAnim.freq);
    }

    protected virtual void Update()
    {
        if (target)
        {
            Vector2 dist = target.transform.position - transform.position;
            if (dist.sqrMagnitude > speed * speed * Time.deltaTime)
            {
                transform.position += (Vector3)dist.normalized * speed * Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = initialPosition + bobAnim.dir * Mathf.Sin(Time.time * initialOffset);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        if (!target) return;
        //Debug.Log("Item being vaccuumed");
    }
}

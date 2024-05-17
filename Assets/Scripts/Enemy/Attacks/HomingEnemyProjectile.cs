using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to an enemy projectile with homing property
/// </summary>
public class HomingEnemyProjectile : EnemyProjectile
{
    public float curveTime = 0;
    public AnimationCurve turnSpeedCurve;
    public float turnSpeedCurveTime = 1;
    public float turnSpeed = 150;

    private void FixedUpdate()
    {
        Vector2 direction = (Vector2)target.transform.position - rb.position;
        direction.Normalize();
        curveTime += Time.fixedDeltaTime / turnSpeedCurveTime;
        float rotateSpeed = turnSpeed * turnSpeedCurve.Evaluate(curveTime);
        float rotateAmount = Vector3.Cross(direction, transform.right).z;

        rb.angularVelocity = -rotateSpeed * rotateAmount;
        rb.velocity = transform.right * stats.speed;
    }
}

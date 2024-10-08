using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    //References

    Animator am;
    PlayerInput pm;
    SpriteRenderer sr;
    Transform resourceBars;
    Vector2 tempLocalScale;
    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerInput>();
        sr = GetComponent<SpriteRenderer>();
        resourceBars = transform.Find("Resource Bar Canvas");
        tempLocalScale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        if (pm.moveDir.x != 0 || pm.moveDir.y != 0)
        {
            am.SetBool("Move", true);
            SpriteDirectionChecker(pm.moveDir);
        }
        else
        {
            am.SetBool("Move", false);
        }

    }

    public void SpriteDirectionChecker(Vector2 dir)
    {
        if (dir.x < 0 && tempLocalScale.x > 0)
        {
            //sr.flipX = true;
            tempLocalScale.x *= -1;
            transform.localScale = tempLocalScale;
            resourceBars.localScale = tempLocalScale;


        }
        else if (dir.x > 0 && tempLocalScale.x < 0)
        {
            //sr.flipX = false;
            tempLocalScale.x *= -1;
            transform.localScale = tempLocalScale;
            resourceBars.localScale = tempLocalScale;

        }
    }


}

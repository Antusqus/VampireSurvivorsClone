using UnityEngine;
using System.Collections;

public class HitBoxManager : MonoBehaviour
{

    // Set these in the editor
    public PolygonCollider2D frame2;
    public PolygonCollider2D frame3;
    public PolygonCollider2D frame4;

    public PolygonCollider2D frame5;



    // Used for organization
    private PolygonCollider2D[] colliders;

    // Collider on this game object
    private PolygonCollider2D localCollider;

    // We say box, but we're still using polygons.
    public enum HitBoxes
    {
        frame2Box,
        frame3Box,
        frame4Box,
        frame5Box,


        clear // special case to remove all boxes
    }

    void Start()
    {
        // Set up an array so our script can more easily set up the hit boxes
        colliders = new PolygonCollider2D[] { frame2, frame3, frame4, frame5};

        // Create a polygon collider
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = false; // Set as a trigger so it doesn't collide with our environment
        localCollider.pathCount = 0; // Clear auto-generated polygons
    }
    public void SetHitBox(HitBoxes val)
    {
        if (val != HitBoxes.clear)
        {
            localCollider.SetPath(0, colliders[(int)val].GetPath(0));
            return;
        }
        localCollider.pathCount = 0;
    }
}
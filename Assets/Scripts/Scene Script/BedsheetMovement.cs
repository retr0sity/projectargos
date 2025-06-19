using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedsheetMovement : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;

    private int i;

    void Start()
    {
        transform.position = points[startingPoint].position;
    }
    void Update()
    {
        // Ensure the index i is within the bounds of the points array
        if (i >= points.Length) return;  // Prevent out of bounds error

        // Move to the next point when the distance is small
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++; // Move to the next point

            if (i == points.Length) // If we've reached the last point, stop updating
            {
                // Optionally disable the script or stop movement
                this.enabled = false; // Disables the Update method from running
                return;  // Exit the Update function to avoid any further movement
            }
        }

        // Move towards the current target point
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
}

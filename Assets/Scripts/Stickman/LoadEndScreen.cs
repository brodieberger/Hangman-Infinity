using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEndScreen : MonoBehaviour
{
    // Speed at which the object rises
    public float speed = 2.0f;

    // Number of units to rise
    public float riseDistance = 5.0f;

    // Flag to track if the object is rising
    private bool isRising = false;

    // Initial position of the object
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    public void StartRising()
    {
        // Invoke the Rise function
        Invoke("Rise", 1.0f);
    }

    private void Rise()
    {
        isRising = true;
    }

    private void Update()
    {
        if (isRising)
        {
            // Move the object upwards
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            // Check if the object has risen to the desired height
            if (transform.position.y - initialPosition.y >= riseDistance)
            {
                // Reset the flag
                isRising = false;
            }
        }
    }
}

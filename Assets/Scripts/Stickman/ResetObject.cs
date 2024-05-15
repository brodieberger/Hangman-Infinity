using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResettableObject : MonoBehaviour
{
    // Variables to store initial state
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    public Rigidbody2D rigidbodyRight;
    public Rigidbody2D rigidbodyLeft;

    public StickmanScript StickmanScript;

    void Start()
    {
        // Store the initial state when the scene loads
        StoreInitialState();
    }

    void Update()
    {
        if (StickmanScript.resetCharacterNow == true)
        {
            ResetObject();
            rigidbodyRight.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.None;
            rigidbodyLeft.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.None;
        }
    }

    // Method to store the initial state
    private void StoreInitialState()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    // Method to reset the object to its initial state
    public void ResetObject()
    {
        // Reset position, rotation, and scale to initial values
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

    }
}

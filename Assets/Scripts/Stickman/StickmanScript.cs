using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StickmanScript : MonoBehaviour
{


    public GameObject[] objectsToDisappear;
    public GameObject rightString;
    public GameObject leftString;
    public HingeJoint2D rightRope;
    public HingeJoint2D leftRope;
    public Rigidbody2D rightArm;
    public Rigidbody2D leftArm;

    public TMP_Text counterText; // Reference to the Text component to display the counter
    public int counter = 6;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool lose = false;

    public Boolean resetCharacterNow = false;
    public bool stickUpdate = false;
    public bool success = false;

    public GameManager GameManager;


    void Update()
    {
        stickUpdate = GameManager.UpdateStick();
        success = GameManager.Success();
        counter = GameManager.ReturnLives();
        resetCharacterNow = false;

        if (stickUpdate == true)
        {
            UpdateStickAction();
            GameManager.StickUpdated();
        }
   
    }

    public void Start()
    {
        counter = GameManager.ReturnLives();
        UpdateLives();
        counterText.text = "Lives: " + counter.ToString();
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetCharacterAction()
    {
        resetCharacterNow = true;
        rightString.SetActive(true);
    }

    public void UpdateStickAction()
    {
        // Check if variable is within the range of objectsToDisappear array length
        if (counter >= 0 && counter <= objectsToDisappear.Length)
        {
            // Loop through each object in the array
            for (int i = 0; i < objectsToDisappear.Length; i++)
            {
                // Get the renderer component of the object
                Renderer renderer = objectsToDisappear[i].GetComponent<Renderer>();

                // Check if the renderer exists
                if (renderer != null)
                {
                    // Set the visibility of the renderer based on the condition
                    renderer.enabled = i < counter;
                }
                else
                {
                    Debug.LogWarning("Renderer component not found on object: " + objectsToDisappear[i].name);
                }
            }
        }
        else
        {
            Debug.LogWarning("Variable is out of range for objectsToDisappear array length.");
        }

        if (counter == 1 && lose == false)
        {
            rightString.SetActive(false);
            rightArm.constraints = RigidbodyConstraints2D.None;
            leftArm.constraints = RigidbodyConstraints2D.None;
        }
        else if (counter == 0 && lose == false)
        {
            leftString.SetActive(false);
            rightString.SetActive(false);
            rightRope.enabled = false;
            leftRope.enabled = false;
        }
        else
        {
            rightString.SetActive(true);
        }
        if (success == true){
            ResetCharacterAction();
            rightString.SetActive(true);
        }
        
    }

    public void UpdateLives()
    {
        counter = GameManager.ReturnLives();
    }
}
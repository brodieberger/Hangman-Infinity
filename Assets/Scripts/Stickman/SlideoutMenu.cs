using System.Collections;
using UnityEngine;

public class SlideoutMenu : MonoBehaviour
{
    public GameObject[] ButtonsToMove; // Array of GameObjects to move
    public float slideDuration = 1f; // Duration of the slide in seconds

    private Vector3[] originalPositions; // Array to store the original positions of buttons

    private void Start()
    {
        // Record the original positions of buttons when the script starts
        originalPositions = new Vector3[ButtonsToMove.Length];
        for (int i = 0; i < ButtonsToMove.Length; i++)
        {
            originalPositions[i] = ButtonsToMove[i].transform.position;
        }
    }

    public void MoveToRight()
    {
        Vector3[] targetPositions = new Vector3[ButtonsToMove.Length];
        for (int i = 0; i < ButtonsToMove.Length; i++)
        {
            targetPositions[i] = originalPositions[i] + new Vector3(309, 0, 0); // Slide each button to the right
        }

        StartCoroutine(SlideCoroutine(targetPositions));
    }

    public void MoveBack()
    {
        StartCoroutine(SlideCoroutine(originalPositions));
    }

    private IEnumerator SlideCoroutine(Vector3[] targetPositions)
    {
        Vector3[] startPositions = new Vector3[ButtonsToMove.Length];

        for (int i = 0; i < ButtonsToMove.Length; i++)
        {
            startPositions[i] = ButtonsToMove[i].transform.position;
        }

        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;

            for (int i = 0; i < ButtonsToMove.Length; i++)
            {
                ButtonsToMove[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], t);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < ButtonsToMove.Length; i++)
        {
            ButtonsToMove[i].transform.position = targetPositions[i];
        }
    }
}

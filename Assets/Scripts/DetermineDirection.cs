using System.Collections;
using UnityEngine;

public class DetermineDirection : MonoBehaviour
{
    // https://docs.unity3d.com/ScriptReference/Touch-phase.html

    // How far the user needs to swipe in order to count
    public float swipeDistance = .75f;
    // How fast the object will finish moving off screen if swipe accepted
    public float stepSize = .75f;
    // How far the object will rotate with swipe
    public float rotationAngle = 30f;
    // End position after release
    public float endPosition = -3f;

    private int stepsTaken = 0;
    private bool reset = false;
    private float resetStep;
    public bool fixSteps = false;
    public float fixedSteps = 10f;

    private Vector3 transformStart; // Where object moving starts
    private Quaternion rotationStart; // Starting rotation of moving object

    private Vector3 touchStart; // Where current touch starts
    private Vector3 lastPosition; // Position of touch in previous frame
    private float travelDistance; // Distance from last frame to current position
    private float overallDistance; // Distance from start position to current position
    private bool swipeAccepted = false; // Controls when a swipe is accepted

    // Start is called before the first frame update
    void Start()
    {
        // Set start position to current position
        transformStart = transform.position;
        // Set starting rotation
        rotationStart = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // If there is at least one touch on the screen
        if (Input.touchCount > 0 && !reset)
        {
            // Get the first touch
            Touch touch = Input.GetTouch(0);
            // Determine what to do based on the phase of the touch
            switch (touch.phase)
            {
                // If the phase is Began, meaning this is the first frame the touch has been recorded
                case TouchPhase.Began:
                    // Convert the position of the touch from pixel to world coordinates
                    touchStart = Camera.main.ScreenToWorldPoint(touch.position);
                    // Set lastPosition to the start position
                    lastPosition = touchStart;
                    // Reset swipeAccepted
                    swipeAccepted = false;
                    stepsTaken = 0;
                    break;
                // If this touch has moved
                case TouchPhase.Moved:
                    // Get the current position in world coordinates
                    Vector3 currentPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    // Determine the total distance the touch has traveled
                    overallDistance = touchStart.x - currentPosition.x;
                    // Determine the distance traveled since last frame
                    travelDistance = lastPosition.x - currentPosition.x;
                    // Move attached object the travel distance
                    transform.position -= new Vector3(travelDistance, 0);

                    if (transform.name.Contains("RX"))
                    {
                        transform.RotateAround(transform.position, Vector3.forward, -rotationAngle * travelDistance);
                    }
                    stepsTaken += 1;

                    // Update lastPosition
                    lastPosition = currentPosition;
                    break;
                // If the touch has ended 
                case TouchPhase.Ended:
                    // If the overall distance is greater than the set swipeDistance
                    if (Mathf.Abs(overallDistance) >= swipeDistance)
                        swipeAccepted = true; // Record a true swipe
                    else
                    {
                        reset = true;
                    }
                    break;
            };
            // If the swipe was acepted
            if (swipeAccepted && !reset)
            {
                // If the distance is positive
                if (overallDistance > 0)
                {
                    // The user swiped left
                    Debug.Log("swiped left");
                    // Initialize coroutine to move object to final position and reset
                    StartCoroutine(FlingAndReturn(true));
                }
                // If the distance is negative
                if (overallDistance < 0)
                {
                    // The user swiped right
                    Debug.Log("swiped right");
                    // Initialize coroutine to move object to final position and reset
                    StartCoroutine(FlingAndReturn(false));
                }
            }
        }

        if (reset)
        {
            resetStep = Mathf.Abs(overallDistance) / stepsTaken;

            if (fixSteps)
                resetStep = Mathf.Abs(overallDistance) / fixedSteps;

            if (overallDistance > 0)
            {
                transform.RotateAround(transform.position, Vector3.forward, rotationAngle * resetStep);
                transform.position += new Vector3(resetStep, 0);
            }
            else
            {
                transform.RotateAround(transform.position, Vector3.forward, -rotationAngle * resetStep);
                transform.position -= new Vector3(resetStep, 0);
            }

            if (Mathf.Abs(transform.position.x - transformStart.x) <= resetStep)
            {
                transform.position = transformStart; // Reset position
                transform.rotation = rotationStart; // Reset orientation
                reset = false;
            }
        }
    }

    // Moves object to final position and resets
    private IEnumerator FlingAndReturn(bool leftORright)
    {
        // If leftORright is true we swiped left
        if (leftORright)
        {
            // Iterate until it reaches the desired position
            while (transform.position.x > -endPosition)
            {
                // Update position
                transform.position -= new Vector3(stepSize, 0);
                // Pause for a frame
                yield return null;
            }
        }
        // Otherwise we swiped right
        else
        {
            // Iterate until it reaches the desired position
            while (transform.position.x < endPosition)
            {
                // Update position
                transform.position += new Vector3(stepSize, 0);
                // Pause for a frame
                yield return null;
            }
        }
        // Wait .2 seconds
        yield return new WaitForSeconds(.2f);
        transform.position = transformStart; // Reset position
        transform.rotation = rotationStart; // Reset orientation
    }
}

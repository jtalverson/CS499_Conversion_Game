using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.iOS;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    private Vector3 startPosition;         // Page start position
    private Quaternion startRotation;      // Page initial rotation
    private Vector3 startRotationAngles;   // Euler angles of starting rotation
    private Vector3 currentRotationAngles; // Euler angles of current rotation
    private float rotationDifference;      // Difference between z-axis angles of current and start

    public bool canSwipe = true;             // Is the user allowed to swipe
    public float maxRotation = 65f;          // Maximum amount the page is allowed to rotate per swipe in degrees
    public float acceptDistance = 350f;      // Acceptance swipe distance
    public float angleCap = 200f;            // Value has to be higher than this to subtract 360 (for negative angles)
    public float rotationAdjustment = 10f;   // Speeds up rotation by this factor
    public float positionAdjustment = .004f; // Speeds up movement by this factor
    public float finalX = 5.75f;             // Final X-position of page
    public float xStepSize = .4f;            // X step size
    public float xTimeGap = .05f;            // Time gap between steps
    public float respawnDelay = .1f;         // Delay between replacing page and allowing new swipe

    private float movementRatio; // Ratio between distance moved and acceptance distance
    private float oldRatio;      // Ratio of previous step
    private float oldPosition;   // Position of previous step

    private Vector2 touchStart;  // Starting position of the current touch
    private float distanceMoved; // Difference between X of touchStart and current location

    private bool establishStepSize = true; // Only want to establish step size once per movement
    private float resetPosStep;            // Position step size for reset
    private float resetRotStep;            // Rotation step size for reset
    private bool reset = false;            // Whether or not we should be resetting currently
    public float allowedGap = .01f;        // Distance from current position to inital allowable to finish resetting
    public float fixedSteps = 10f;         // Number of steps to move back

    private bool enterMovedPhase = false;  // Whether the touch has been detected while allowed to swipe
    private bool touchedUI = false;        // States whether or not a UI element has been touched

    public float buttonSteps = 9f;         // Number of steps taken when a button is pressed
    public float buttonTimeGap = .05f;     // Time between steps when a button is pressed
    public float maxButtonRotation = 45f;  // Max rotation of page when a button is pressed

    public float finalY = -6f;         // Final Y-position when the timer runs out
    public float rejectSteps = 9f;     // Steps to final position from base
    public float rejectTimeGap = .05f; // Time gap between reject movement
    private float rejectPosStep;       // Position step size for time up

    [HideInInspector]
    private Vector3 swipeAcceptedInitialPosition; // Inital position of the object once a swipe is complete

    public float animationLength = 1; // Duration of movement animation

    public string currentAnswer;      // Holds the current answer as a string
    public GameController controller; // GameController object

    private bool startTimer = false;  // Controls when the timer can be started

    public GameObject pauseMenu;      // Used to determine if swiping is allowed

    public AudioSource SwipeNoiseSource; // Overall audio source
    public AudioClip SwipeNoiseClip;     // Swipe sound clip
    public AudioClip RightNoiseClip;     // Right sound clip
    public AudioClip WrongNoiseClip;     // Wrong sound clip
    public GameObject GameBackground;    // Background object
    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.localRotation;
        startRotationAngles = startRotation.eulerAngles;
    }

    // When the page is enabled in the hierarchy
    private void OnEnable()
    {
        // allow the user to swipe
        canSwipe = true;
        // if we are allowed to start the timer we do and set the permission to false
        if (startTimer)
        {
            controller.timer.timerIsRunning = true;
            startTimer = false;
        }
    }

    // When the page is disabled in the hierarchy
    private void OnDisable()
    {
        // Stop allowing the user to swipe
        canSwipe = false;
        // if we are not allowed to swipe set start timer to true
        if (!startTimer)
            startTimer = true;
        // Stop the timer
        controller.timer.timerIsRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If the pause menu is active return immediately
        if (pauseMenu.activeInHierarchy)
            return;
        // Get the current euler angles of transform
        currentRotationAngles = transform.localRotation.eulerAngles;
        // If there is at least one touch and we can swipe and we are not resetting
        if (Input.touchCount > 0 && canSwipe && !reset)
        {
            // Get the first touch in the list
            Touch touch = Input.GetTouch(0);
            // Check to see if the touch is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                // Creare a new pointer and assign it to the touch's position
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = touch.position;
                // Create a new raycast result list and call RaycastAll to store all object hit 
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
                // If there are any objects in the list
                if (raycastResults.Count > 0)
                    // Iterate over them
                    foreach (var cast in raycastResults)
                    {
                        Debug.Log(cast.gameObject.name);
                        // Check to see if any of the names match the specific UI elements
                        if (cast.gameObject.name == "DisapproveBtn" || cast.gameObject.name == "ApproveBtn" 
                            || cast.gameObject.name.Contains("Pause"))
                        {
                            // If so we have touched a button and we should indicate with touchedUI
                            Debug.Log("Touched Buttons");
                            touchedUI = true;
                            return;
                        }
                    }
            }
            // If the touch is in the Began phase
            if (touch.phase == TouchPhase.Began)
            {
                // Record the starting position
                touchStart = touch.position;
                // Reset distance moved
                distanceMoved = 0f;
                // Reset old ratio
                oldRatio = 0f;
                // Reset old position to the current position
                oldPosition = touch.position.x;
                // Allows us to enter the moved phase
                enterMovedPhase = true; // Necessary because swiping while the previous swipe was being processed caused issues
            }
            // Touch is in moved phase an its inital phase was detected while we are allowed to swipe
            else if (touch.phase == TouchPhase.Moved && enterMovedPhase)
            {
                // Update distance moved
                distanceMoved = touch.position.x - touchStart.x;
                // Update movement ratio
                movementRatio = distanceMoved / acceptDistance;
                // Update rotation difference
                rotationDifference = Mathf.Abs(currentRotationAngles.z - startRotationAngles.z);
                // Account for counterclockwise rotation
                if (rotationDifference > angleCap)
                    rotationDifference = 360f - rotationDifference;
                // If we have not rotated the max angle
                if (rotationDifference < maxRotation)
                    // Rotate the page
                    transform.Rotate(transform.forward, (movementRatio - oldRatio) * rotationAdjustment);
                // Update the old ratio
                oldRatio = movementRatio;
                // Adjust the position of the page
                transform.position += new Vector3(positionAdjustment * (touch.position.x - oldPosition), 0f, 0f);
                // Update old position
                oldPosition = touch.position.x;
            }
            // The touch has been released
            else if (touch.phase == TouchPhase.Ended)
            {
                // Disable swiping
                canSwipe = false;
                // If the distance moved is acceptable
                if (Mathf.Abs(distanceMoved) > acceptDistance)
                {
                    // Play the swipe noise clip
                    SwipeNoiseSource.PlayOneShot(SwipeNoiseClip);
                    // Store the position of the transform at this point
                    swipeAcceptedInitialPosition = transform.position;
                    // Begin AcceptOrReject coroutine
                    StartCoroutine("AcceptOrReject");
                    Debug.Log("Good swipe");
                }
                // If we have touched a UI element 
                else if (touchedUI)
                {
                    // Reset that to false
                    Debug.Log("Released UI");
                    touchedUI = false;
                }
                // Otherwise the distance was not acceptable
                else
                {
                    // Set reset to true
                    reset = true;
                    Debug.Log("Swipe rejected");
                }
                // Reset enter moved phase
                enterMovedPhase = false;
            }
        }
        // If we want to reset
        if (reset)
        {
            // If we are to establish the step size (necessary because the position changes which would change the step size if updated each frame)
            if (establishStepSize)
            {
                // Calculate position and rotation step size and set establish to false
                resetPosStep = Mathf.Abs(transform.position.x - startPosition.x) / fixedSteps;
                resetRotStep = rotationDifference / fixedSteps;
                establishStepSize = false;
            }
            // If we have swiped right move it to the left and rotate clockwise
            if (distanceMoved > 0)
            {
                transform.RotateAround(transform.position, Vector3.forward, -resetRotStep);
                transform.position -= new Vector3(resetPosStep, 0);
            }
            // Otherwise we have swiped left so move it to the right and rotate counterclockwise
            else
            {
                transform.RotateAround(transform.position, Vector3.forward, resetRotStep);
                transform.position += new Vector3(resetPosStep, 0);
            }
            // If the distance between the current position and ending position is close
            if (Mathf.Abs(transform.position.x - startPosition.x) <= allowedGap)
            {
                transform.position = startPosition; // Reset position
                transform.rotation = startRotation; // Reset orientation
                reset = false;                      // Set reset to false
                establishStepSize = true;           // Reset establish step size
                canSwipe = true;                    // Enable swiping
                distanceMoved = 0f;                 // Reset distance moved
            }
        }
    }

    // Handles the final movement of accepting and rejecting after a swipe
    public IEnumerator AcceptOrReject()
    {
        // Stop the timer
        controller.timer.timerIsRunning = false;

        // Swiped right
        if (distanceMoved > 0)
        {
            // Establish elapsed time to zero
            float elapsedTime = 0.0f;
            Debug.Log("Swiped RIGHT");
            // Iterate while elapsed time is less than animation length
            while (elapsedTime < animationLength)
            {
                // Update elapsed time
                elapsedTime += Time.deltaTime;
                // Linearly interpolate page to desired position
                transform.position = Vector3.Lerp(swipeAcceptedInitialPosition,
                    new Vector3(finalX, transform.position.y),
                    easeOutCubic(elapsedTime / animationLength));
                // Pause until next frame
                yield return null;
            }
            // If the first character of the answer is Y
            if (currentAnswer[0] == 'Y')
            {
                // Populate new answers and process the answer as correct
                controller.Populate(true);
                // Update the score with the time remaining and signal correct answer
                controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, true);
                // Play right noise clip
                SwipeNoiseSource.PlayOneShot(RightNoiseClip);
                Debug.Log("right answer");
            }
            // Otherwise the answer was wrong
            else
            {
                // Populate new answers and process the answer as incorrect
                controller.Populate(false);
                // Update the score with the time remaining and signal incorrect answer
                controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, false);
                // Play wrong noise clip
                SwipeNoiseSource.PlayOneShot(WrongNoiseClip);
                Debug.Log("wrong answer");
            }
        }
        // Swiped left
        else
        {
            // Establish elapsed time to zero
            float elapsedTime = 0.0f;
            Debug.Log("Swiped LEFT");
            // Iterate while elapsed time is less than animation length
            while (elapsedTime < animationLength)
            {
                // Update elapsed time
                elapsedTime += Time.deltaTime;
                // Linearly interpolate page to desired position
                transform.position = Vector3.Lerp(swipeAcceptedInitialPosition,
                    new Vector3(-finalX, transform.position.y),
                    easeOutCubic(elapsedTime / animationLength));
                // Pause until next frame
                yield return null;
            }
            // If the first character of the answer is N
            if (currentAnswer[0] == 'N')
            {
                // Populate new answers and process the answer as correct
                controller.Populate(true);
                // Update the score with the time remaining and signal correct answer
                controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, true);
                // Play right noise clip
                SwipeNoiseSource.PlayOneShot(RightNoiseClip);
                Debug.Log("right answer");
            }
            else
            {
                // Populate new answers and process the answer as incorrect
                controller.Populate(false);
                // Update the score with the time remaining and signal incorrect answer
                controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, false);
                // Play wrong noise clip
                SwipeNoiseSource.PlayOneShot(WrongNoiseClip);
                Debug.Log("wrong answer");
            }
        }
        // Reset the timer
        controller.timer.timeRemaining = controller.timer.maxTime;
        // Update it
        controller.timer.UpdateSlider();
        // Reset position and rotation
        transform.position = startPosition;
        transform.rotation = startRotation;
        // yield return new WaitForSeconds(respawnDelay);
        // Allow swiping
        canSwipe = true;
        // Reset distance moved
        distanceMoved = 0f;
        // Start the timer again
        controller.timer.timerIsRunning = true;
    }
    // Run when the accept button is pressed
    public void Accept()
    {
        SwipeNoiseSource.PlayOneShot(SwipeNoiseClip);
        controller.timer.timerIsRunning = false;
        // Starts the SlowAccept coroutine which moves the page slower through the motion
        StartCoroutine("SlowAccept");
    }
    // Slowly moves page to the right
    private IEnumerator SlowAccept()
    {
        // Establish elapsed time to zero
        float elapsedTime = 0.0f;
        // Iterate while elapsed time is less than animation length
        while (elapsedTime < animationLength)
        {
            // Update elapsed time
            elapsedTime += Time.deltaTime;
            // Determine angle to rotate and rotate
            float lerpedAngle = Mathf.Lerp(0, maxButtonRotation, easeOutCubic(elapsedTime / animationLength));
            float angleDelta = lerpedAngle - transform.rotation.eulerAngles.z;
            transform.RotateAround(transform.position, Vector3.forward, angleDelta);
            // Linearly interpolate page to desired position
            transform.position = Vector3.Lerp(startPosition, 
                new Vector3(finalX, transform.position.y), 
                easeOutCubic(elapsedTime / animationLength));
            // Pause until next frame
            yield return null;
        }
        // If the answer is correct process it as so
        if (currentAnswer[0] == 'Y')
        {
            SwipeNoiseSource.PlayOneShot(RightNoiseClip);
            controller.Populate(true);
            controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, true);
            Debug.Log("right answer");
        }
        // Otherwise process it as incorrect
        else
        {
            SwipeNoiseSource.PlayOneShot(WrongNoiseClip);
            controller.Populate(false);
            controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, false);
            Debug.Log("wrong answer");
        }
        // Reset timer
        controller.timer.timeRemaining = controller.timer.maxTime;
        controller.timer.UpdateSlider();
        // Reset page position
        transform.position = startPosition;
        transform.rotation = startRotation;
        // Allow swiping
        canSwipe = true;
        // Restart timer
        controller.timer.timerIsRunning = true;
    }
    // Run when the accept button is pressed
    public void Reject()
    {
        // Plays swiping sound
        SwipeNoiseSource.PlayOneShot(SwipeNoiseClip);
        // Stops the timer
        controller.timer.timerIsRunning = false;
        // Starts the SlowAccept coroutine which moves the page slower through the motion
        StartCoroutine("SlowReject");
    }
    // Smooths linear interpolations
    private float easeOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }
    // Slows rejection animation
    private IEnumerator SlowReject()
    {
        // Establish elapsed time to zero
        float elapsedTime = 0.0f;
        // Iterate while elapsed time is less than animation length
        while (elapsedTime < animationLength)
        {
            // Update elapsed time
            elapsedTime += Time.deltaTime;
            // Determine angle to rotate and rotate
            float lerpedAngle = Mathf.Lerp(0, -maxButtonRotation, easeOutCubic(elapsedTime / animationLength));
            float angleDelta = lerpedAngle - transform.rotation.eulerAngles.z;
            transform.RotateAround(transform.position, Vector3.forward, angleDelta);
            // Linearly interpolate page to desired position
            transform.position = Vector3.Lerp(startPosition, 
                new Vector3(-finalX, transform.position.y), 
                easeOutCubic(elapsedTime / animationLength));
            // Pause until next frame
            yield return null;
        }
        // If the answer is correct process it as so
        if (currentAnswer[0] == 'N')
        {
            // Play right noise clip
            SwipeNoiseSource.PlayOneShot(RightNoiseClip);
            controller.Populate(true);
            controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, true);
            Debug.Log("right answer");
        }
        // Otherwise process it as incorrect
        else
        {
            // Play wrong noise clip
            SwipeNoiseSource.PlayOneShot(WrongNoiseClip);
            controller.Populate(false);
            controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, false);
            Debug.Log("wrong answer");
        }
        // Reset timer
        controller.timer.timeRemaining = controller.timer.maxTime;
        controller.timer.UpdateSlider();
        // Reset page position
        transform.position = startPosition;
        transform.rotation = startRotation;
        // Allow swiping
        canSwipe = true;
        // Restart timer
        controller.timer.timerIsRunning = true;
    }
    // Move the page down and off the screen if the time runs out
    public IEnumerator TimeUp()
    {
        // Play wrong noise clip
        SwipeNoiseSource.PlayOneShot(WrongNoiseClip);
        // Calculate step size
        rejectPosStep = Mathf.Abs(transform.position.y - finalY) / rejectSteps;
        // While it is not at the desired position move it down and wait
        while (transform.position.y > finalY)
        {
            transform.position -= new Vector3(0, rejectPosStep);
            yield return new WaitForSeconds(rejectTimeGap);
        }

        // Populate the new conversions with false
        controller.Populate(false);
        // Call the scoring update with an incorrect answer
        controller.scoringSystem.ScoreUpdate(controller.timer.timeRemaining, false);
        // Reset timer
        controller.timer.timeRemaining = controller.timer.maxTime;
        controller.timer.UpdateSlider();
        // Reset page position
        transform.position = startPosition;
        transform.rotation = startRotation;
        // Allow swiping
        canSwipe = true;
        // Restart timer
        controller.timer.timerIsRunning = true;
    }
}
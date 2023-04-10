using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.iOS;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startRotationAngles;
    private Vector3 currentRotationAngles;
    private float rotationDifference;

    public bool canSwipe = true;
    public float maxRotation = 65f;
    public float acceptDistance = 350f;
    public float angleCap = 200f;
    public float rotationAdjustment = 10f;
    public float positionAdjustment = .004f;
    public float finalX = 5.75f;
    public float xStepSize = .4f;
    public float xTimeGap = .05f;
    public float respawnDelay = .1f;

    private float movementRatio;
    private float oldRatio;
    private float oldPosition;

    private Vector2 touchStart;
    private float distanceMoved;

    private bool establishStepSize = true;
    private float resetPosStep;
    private float resetRotStep;
    private bool reset = false;
    public float allowedGap = .01f;
    public float fixedSteps = 10f;

    private bool enterMovedPhase = false;
    private bool touchedUI = false;

    public float buttonSteps = 9f;
    public float buttonTimeGap = .05f;
    public float maxButtonRotation = 45f;
    private float buttonPosStep;
    private float buttonRotStep;

    public string currentAnswer;
    public GameController controller;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.localRotation;
        startRotationAngles = startRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        currentRotationAngles = transform.localRotation.eulerAngles;
        if (Input.touchCount > 0 && canSwipe && !reset)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = touch.position;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
                if (raycastResults.Count > 0)
                    foreach (var cast in raycastResults)
                    {
                        // Debug.Log(cast.gameObject.name);
                        if (cast.gameObject.name == "DisapproveBtn" || cast.gameObject.name == "ApproveBtn")
                        {
                            Debug.Log("Touched Buttons");
                            touchedUI = true;
                            return;
                        }
                    }
            }

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                distanceMoved = 0f;
                oldRatio = 0f;
                oldPosition = touch.position.x;
                enterMovedPhase = true;
            }
            else if (touch.phase == TouchPhase.Moved && enterMovedPhase)
            {
                distanceMoved = touch.position.x - touchStart.x;
                movementRatio = distanceMoved / acceptDistance;
                rotationDifference = Mathf.Abs(currentRotationAngles.z - startRotationAngles.z);
                // Account for counterclockwise rotation
                if (rotationDifference > angleCap)
                    rotationDifference = 360f - rotationDifference;

                if (rotationDifference < maxRotation)
                    transform.Rotate(transform.forward, (movementRatio - oldRatio) * rotationAdjustment);
                oldRatio = movementRatio;

                transform.position += new Vector3(positionAdjustment * (touch.position.x - oldPosition), 0f, 0f);
                oldPosition = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                canSwipe = false;
                if (Mathf.Abs(distanceMoved) > acceptDistance)
                {
                    StartCoroutine("AcceptOrReject");
                    Debug.Log("Good swipe");
                }
                else if (touchedUI)
                {
                    Debug.Log("Released UI");
                    touchedUI = false;
                }
                else
                {
                    reset = true;
                    Debug.Log("Swipe rejected");
                }
                enterMovedPhase = false;
            }
        }

        if (reset)
        {
            if (establishStepSize)
            {
                resetPosStep = Mathf.Abs(transform.position.x - startPosition.x) / fixedSteps;
                resetRotStep = rotationDifference / fixedSteps;
                establishStepSize = false;
            }

            if (distanceMoved > 0)
            {
                transform.RotateAround(transform.position, Vector3.forward, -resetRotStep);
                transform.position -= new Vector3(resetPosStep, 0);
            }
            else
            {
                transform.RotateAround(transform.position, Vector3.forward, resetRotStep);
                transform.position += new Vector3(resetPosStep, 0);
            }

            if (Mathf.Abs(transform.position.x - startPosition.x) <= allowedGap)
            {
                transform.position = startPosition; // Reset position
                transform.rotation = startRotation; // Reset orientation
                reset = false;
                establishStepSize = true;
                canSwipe = true;
                distanceMoved = 0f;
            }
        }
    }

    public IEnumerator AcceptOrReject()
    {
        // Swiped right
        if (distanceMoved > 0)
        {
            Debug.Log("Swiped RIGHT");
            while (transform.position.x < finalX)
            {
                transform.position += new Vector3(xStepSize, 0f, 0f);
                yield return new WaitForSeconds(xTimeGap);
            }
            if (currentAnswer == "yes")
                controller.Populate(true);
            else
                controller.Populate(false);
        }
        // Swiped left
        else
        {
            Debug.Log("Swiped LEFT");
            while (transform.position.x > -1 * finalX)
            {
                transform.position -= new Vector3(xStepSize, 0f, 0f);
                yield return new WaitForSeconds(xTimeGap);
            }
            if (currentAnswer == "no")
                controller.Populate(true);
            else
                controller.Populate(false);
        }
        transform.position = startPosition;
        transform.rotation = startRotation;
        // yield return new WaitForSeconds(respawnDelay);
        canSwipe = true;
        distanceMoved = 0f;
    }

    public void Accept()
    {
        buttonPosStep = finalX / buttonSteps;
        buttonRotStep = maxButtonRotation / buttonSteps;
        Debug.Log("Accepted");
        StartCoroutine("SlowAccept");
    }

    private IEnumerator SlowAccept()
    {
        while (transform.position.x < finalX)
        {
            transform.RotateAround(transform.position, Vector3.forward, buttonRotStep);
            transform.position += new Vector3(buttonPosStep, 0);
            yield return new WaitForSeconds(buttonTimeGap);
        }

        if (currentAnswer == "yes")
            controller.Populate(true);
        else
            controller.Populate(false);

        transform.position = startPosition;
        transform.rotation = startRotation;
        canSwipe = true;
    }

    public void Reject()
    {
        buttonPosStep = finalX / buttonSteps;
        buttonRotStep = maxButtonRotation / buttonSteps;
        Debug.Log("Rejected");
        StartCoroutine("SlowReject");
    }

    private IEnumerator SlowReject()
    {
        while (transform.position.x > -finalX)
        {
            transform.RotateAround(transform.position, Vector3.forward, -buttonRotStep);
            transform.position -= new Vector3(buttonPosStep, 0);
            yield return new WaitForSeconds(buttonTimeGap);
        }

        if (currentAnswer == "no")
            controller.Populate(true);
        else
            controller.Populate(false);

        transform.position = startPosition;
        transform.rotation = startRotation;
        canSwipe = true;
    }
}
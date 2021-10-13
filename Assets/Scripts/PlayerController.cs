using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SerialController
{
    private CharacterController controller;

    [Header("PlayerController specific variables")]
    [Space(15)]
    [SerializeField]
    private bool isMoving;
    public bool IsMoving
    {
        get { return isMoving; }
    }

    [SerializeField]
    private bool keyboardMovement; // Allows keyboard to be used for movement

    [Space(10)]
    [SerializeField]
    [Range(1, 15)]
    [Tooltip("Speed that character rotates to face the point that it is moving towards")]
    private float rotationSpeed;
    [SerializeField]
    [Range(1, 10)]
    private float movementSpeed;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        // Used for quickly testing out features on keyboard instead of using controller
        // Probably also could be used for joystick movement with slight modifications
        if (keyboardMovement)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 newPos = new Vector3(transform.position.x + horizontal, transform.position.y, transform.position.z + vertical);
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            Vector3 dir = newPos - transform.position;

            if (direction.magnitude >= 0.1f)
            {
                Quaternion rotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                controller.Move(direction * movementSpeed * Time.deltaTime);
            }
        }

        string message = (string)serialThread.ReadMessage();
        if (message == null)
            return;

        // Passes incoming messages from controller to SampleMessageListener, which then logs the message
        if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
            messageListener.SendMessage("OnConnectionEvent", true);
        else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
            messageListener.SendMessage("OnConnectionEvent", false);
        else
            messageListener.SendMessage("OnMessageArrived", message);

        // Checks that player is not moving before checking received message
        if (!isMoving)
            if (message == "UP" || message == "DOWN" || message == "RIGHT" || message == "LEFT")
                StartCoroutine(Move(message));

        switch (message)
        {
            case "BUTTON1":
                ButtonOne();
                break;
            case "BUTTON2":
                ButtonTwo();
                break;
            default:
                break;
        }
    }

    private IEnumerator Move(string direction)
    {
        isMoving = true;

        Vector3 targetPos = transform.position;

        // Calculates new position based on input
        switch (direction)
        {
            case "UPLEFT":
                targetPos.z += 1;
                targetPos.x -= 1;
                break;
            case "UPRIGHT":
                targetPos.z += 1;
                targetPos.x += 1;
                break;
            case "DOWNLEFT":
                targetPos.z -= 1;
                targetPos.x -= 1;
                break;
            case "DOWNRIGHT":
                targetPos.z -= 1;
                targetPos.x += 1;
                break;
            case "UP":
                targetPos.z += 1;
                break;
            case "DOWN":
                targetPos.z -= 1;
                break;
            case "RIGHT":
                targetPos.x += 1;
                break;
            case "LEFT":
                targetPos.x -= 1;
                break;
            default:
                // 
                break;
        }

        // Calculates direction and rotation that player is going to face when they start moving
        Vector3 dir = targetPos - transform.position;
        Quaternion rotation = Quaternion.LookRotation(dir);

        while (transform.position != targetPos)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10 * Time.deltaTime);
            float step = 4 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return new WaitForEndOfFrame();
        }

        isMoving = false;
    }

    private void ButtonOne()
    {
        throw new NotImplementedException();
    }

    private void ButtonTwo()
    {
        throw new NotImplementedException();
    }

    // Method not in use. Can be deleted if no other uses for it
    private Vector3 GetPosition(Transform transform)
    {
        return transform.position;
    }
}

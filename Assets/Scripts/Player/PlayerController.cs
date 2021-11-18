using System.Collections;
using UnityEngine;
public class PlayerController : SerialController
{
    private Animator animator;
    private CharacterController controller;
    private Player player;

    [Header("PlayerController specific variables")]
    [Space(15)]
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
    private float dashCooldown = 0;
    [SerializeField]
    private float gravity;

    private Quaternion canvasRotation;

    /// <summary>
    /// Horizontal input value received from serial monitor.
    /// </summary>
    private float horizontal = 0;
    /// <summary>
    /// Vertical input value received from serial monitor.
    /// </summary>
    private float vertical = 0;

    private void Awake()
    {
        if (messageListener == null)
            messageListener = GameObject.Find("MessageListener");
        if (controller == null)
            controller = gameObject.GetComponent<CharacterController>();
        if (player == null)
            player = gameObject.GetComponent<Player>();
        if (animator == null)
            animator = GetComponent<Animator>();
        canvasRotation = gameObject.transform.Find("Canvas").GetComponent<RectTransform>().rotation;
    }

    private enum MOVE
    {

        // HORIZONTALS X AXIS
        LEFT = 1,   // Starts movement
        STOPLEFT = 2,   // Stops movement
        RIGHT = 3,   // Starts movement
        STOPRIGHT = 4, // Stops movement
        //

        // VERTICAL Y AXIS
        DOWN = -1,  // Starts movement
        STOPDOWN = -2, // Stops movement
        UP = -3,    // Starts movement
        STOPUP = -4    // Stops movement
    }

    void Update()
    {
        if (!GameManager.Instance.GameStarted)
            return;

        if (dashCooldown > 0)
            dashCooldown -= Time.deltaTime;

        // Allows Player1 to pause by pressing escape on keyboard
        // even if keyboardMovement is set to false.
        if (Input.GetKeyDown(KeyCode.Escape) && player.GetPlayerNumber == 0)
            ButtonThree();

        string message = (string)serialThread.ReadMessage();

        // If keyboard is in use then calculates horizontal and vertical inputs from keyboard.
        if (keyboardMovement)
        {
            if (Input.GetKeyDown(KeyCode.E))
                ButtonOne();
            if (Input.GetKeyDown(KeyCode.F))
                ButtonTwo();

            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        // Calculates horizontal and vertical inputs from serial messages.
        else
        {
            switch (message)
            {
                case "1":   // LEFT
                    horizontal = -1;
                    break;
                case "2":   // STOPLEFT
                    horizontal = 0;
                    break;
                case "3":   // RIGHT
                    horizontal = 1;
                    break;
                case "4":   // STOPRIGHT
                    horizontal = 0;
                    break;
                case "-1":  // DOWN
                    if (GameManager.Instance.IsPaused)
                        PauseMenu.Instance.SelectButton();
                    else
                        vertical = -1;
                    break;
                case "-2":  // STOPDOWN
                    vertical = 0;
                    break;
                case "-3":  // UP
                    if (GameManager.Instance.IsPaused)
                        PauseMenu.Instance.SelectButton();
                    else
                        vertical = 1;
                    break;
                case "-4":  // STOPUP
                    vertical = 0;
                    break;
                // Button inputs.
                case "5":
                    ButtonOne();
                    break;
                case "6":
                    ButtonTwo();
                    break;
                case "7":
                    ButtonThree();
                    break;
                default:
                    break;
            }
        }

        if (GameManager.Instance.GameOver || GameManager.Instance.IsPaused)
            return;

        Vector3 newPos = new Vector3(transform.position.x + horizontal, transform.position.y, transform.position.z + vertical);
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 dir = newPos - transform.position;

        if (direction.magnitude >= 0.1f)
        {
            // Add gravity
            if (!controller.isGrounded)
                direction.y -= gravity * Time.deltaTime;

            if (animator.speed != movementSpeed / DefaultValues.defaultMovementSpeed)
                animator.speed = movementSpeed / DefaultValues.defaultMovementSpeed;

            if (animator.GetFloat("MoveSpeed") < 1)
                animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") + 0.08f);

            Quaternion rotation = Quaternion.LookRotation(dir);
            RectTransform canvasRect = gameObject.transform.Find("Canvas").GetComponent<RectTransform>();
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            canvasRect.rotation = canvasRotation;
            controller.Move(direction * movementSpeed * Time.deltaTime);
        }
        else if (animator.GetFloat("MoveSpeed") > 0)
        {
            if (animator.speed != 1)
                animator.speed = 1;

            animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") - 0.08f);
        }

        // Passes incoming messages from controller to SampleMessageListener, which then logs the message
        if (message != null)
        {
            if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
                messageListener.SendMessage("OnConnectionEvent", true);
            else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
            {
                messageListener.SendMessage("OnConnectionEvent", false);
            }
            else
                messageListener.SendMessage("OnMessageArrived", message);
        }
    }

    private void ButtonOne()
    {
        if (!GameManager.Instance.GameOver && !GameManager.Instance.IsPaused)
        {
            if (player.CloseIngredient != null)
            {
                player.CloseIngredient.TakeIngredient(player.CloseIngredient.IngredientScriptableObject, player);
                return;
            }

            if (player.ClosePizza != null && player.HeldIngredient != null && !player.ClosePizza.IsCooked)
            {
                player.ClosePizza.AddIngredient(player.HeldIngredient, player);
                return;
            }

            if (player.ClosePizza != null && player.ClosePizza.Ingredients.Count != 0 && player.HeldPizza == null)
            {
                player.PickUpPizza();
                return;
            }

            if (player.ClosePizza != null && player.ClosePizza.Ingredients.Count.Equals(0) && player.HeldPizza != null && player.HeldPizza.cookState != HeldPizzaSO.CookState.Burnt)
            {
                player.PutDownPizza();
                return;
            }

            // Checks that player is close pizza oven, player is holding pizza and pizza cookState is not either cooked or burnt.
            if (player.ClosePizzaOven != null && player.HeldPizza != null && player.HeldPizza.cookState != HeldPizzaSO.CookState.Cooked && player.HeldPizza.cookState != HeldPizzaSO.CookState.Burnt)
            {
                player.ClosePizzaOven.AddPizzaToOven(player.HeldPizza, player);
                return;
            }

            if (player.ClosePizzaOven != null && player.ClosePizzaOven.PizzaInOven && player.HeldPizza == null)
            {
                player.ClosePizzaOven.TakePizza(player);
                return;
            }

            if (player.CloseDeliveryPoint != null && player.HeldPizza != null && player.HeldPizza.cookState == HeldPizzaSO.CookState.Cooked)
            {
                player.CloseDeliveryPoint.DeliverPizza(player.HeldPizza, player);
            }

            if (player.CloseTrashBin != null && player.HeldPizza != null)
            {
                player.DiscardPizza();
                return;
            }
        }
        else if (GameManager.Instance.GameOver)
        {
            GameManager.Instance.MainMenu();
        }
        else if (GameManager.Instance.IsPaused)
        {
            PauseMenu.Instance.Click();
        }
    }

    private void ButtonTwo()
    {
        if (!GameManager.Instance.GameOver && !GameManager.Instance.IsPaused && dashCooldown <= 0)
            StartCoroutine("Dash");

        if (GameManager.Instance.GameOver)
            GameManager.Instance.Restart();
    }

    // Pause button
    private void ButtonThree()
    {
        if (!GameManager.Instance.IsPaused)
            GameManager.Instance.Pause();
        else
            GameManager.Instance.Unpause();
    }

    private IEnumerator Dash()
    {
        Debug.Log("DASH");
        float timer = 2;
        float wait = 0.1f;

        dashCooldown = DefaultValues.dashCooldownLength;
        movementSpeed += 2;

        while (timer > 0)
        {
            timer -= wait;
            yield return new WaitForSeconds(wait);
        }

        movementSpeed = DefaultValues.defaultMovementSpeed;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : SerialController
{
    [Header("PlayerController specific variables")] [Space(15)] [SerializeField]
    private GameObject dashEffect;

    [SerializeField] private bool keyboardMovement; // Allows keyboard to be used for movement

    [Space(10)]
    [SerializeField]
    [Range(1, 15)]
    [Tooltip("Speed that character rotates to face the point that it is moving towards")]
    private float rotationSpeed;

    [SerializeField] [Range(1, 10)] private float movementSpeed;

    [SerializeField] private float gravity;

    private Animator animator;

    // Used to keep attached canvas to face towards camera all the time.
    private Quaternion          canvasRotation;
    private CharacterController controller;
    private bool                cooldown;
    private float               dashCooldown;

    // Used to display dash cooldown and duration on UI.
    private Image dashProgressBar;

    /// <summary>
    ///     Horizontal input value received from serial monitor.
    /// </summary>
    private float horizontal;

    private Player player;

    /// <summary>
    ///     Vertical input value received from serial monitor.
    /// </summary>
    private float vertical;

    private void Awake()
    {
        if (messageListener == null)
        {
            messageListener = GameObject.Find("MessageListener");
        }

        if (controller == null)
        {
            controller = gameObject.GetComponent<CharacterController>();
        }

        if (player == null)
        {
            player = gameObject.GetComponent<Player>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Finds dash progress bars from scene gameobjects if it is null.
        if (dashProgressBar == null)
        {
            dashProgressBar = player.GetPlayerNumber switch
            {
                Player.PlayerNumber.Player1 => GameManager.Instance.P1DashProgressBar,
                Player.PlayerNumber.Player2 => GameManager.Instance.P2DashProgressBar,
                _                           => dashProgressBar
            };
        }

        canvasRotation = gameObject.transform.Find("Canvas").GetComponent<RectTransform>().rotation;
        dashCooldown   = DefaultValues.dashCooldownLength;
    }

    private void Update()
    {
        if (!GameManager.Instance.GameStarted)
        {
            return;
        }

        if (dashCooldown < DefaultValues.dashCooldownLength && cooldown)
        {
            dashCooldown += Time.deltaTime;
            var percent = dashCooldown / DefaultValues.dashCooldownLength;
            dashProgressBar.fillAmount = Mathf.Lerp(0, 1, percent);
        }

        // Allows Player1 to pause by pressing escape on keyboard
        // even if keyboardMovement is set to false.
        if (Input.GetKeyDown(KeyCode.Escape) && player.GetPlayerNumber == 0)
        {
            ButtonThree();
        }

        var message = (string) serialThread.ReadMessage();

        // If keyboard is in use then calculates horizontal and vertical inputs from keyboard.
        if (keyboardMovement)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ButtonOne();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ButtonTwo();
            }

            horizontal = Input.GetAxisRaw("Horizontal");
            vertical   = Input.GetAxisRaw("Vertical");
        }
        // Calculates horizontal and vertical inputs from serial messages.
        else
        {
            switch (message)
            {
                case "1": // LEFT
                    horizontal = -1;
                    break;
                case "2": // STOPLEFT
                    horizontal = 0;
                    break;
                case "3": // RIGHT
                    horizontal = 1;
                    break;
                case "4": // STOPRIGHT
                    horizontal = 0;
                    break;
                case "-1": // DOWN
                    if (GameManager.Instance.IsPaused)
                    {
                        PauseMenu.Instance.SelectButton();
                    }
                    else
                    {
                        vertical = -1;
                    }

                    break;
                case "-2": // STOPDOWN
                    vertical = 0;
                    break;
                case "-3": // UP
                    if (GameManager.Instance.IsPaused)
                    {
                        PauseMenu.Instance.SelectButton();
                    }
                    else
                    {
                        vertical = 1;
                    }

                    break;
                case "-4": // STOPUP
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
            }
        }

        if (GameManager.Instance.GameOver || GameManager.Instance.IsPaused)
        {
            return;
        }

        var newPos = new Vector3(transform.position.x + horizontal, transform.position.y,
                                 transform.position.z + vertical);
        var direction = new Vector3(horizontal, 0, vertical).normalized;
        var dir       = newPos - transform.position;

        if (direction.magnitude >= 0.1f)
        {
            // Add gravity
            if (!controller.isGrounded)
            {
                direction.y -= gravity * Time.deltaTime;
            }

            if (animator.speed != movementSpeed / DefaultValues.defaultMovementSpeed)
            {
                animator.speed = movementSpeed / DefaultValues.defaultMovementSpeed;
            }

            if (animator.GetFloat("MoveSpeed") < 1)
            {
                animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") + 0.08f);
            }

            var rotation   = Quaternion.LookRotation(dir);
            var canvasRect = gameObject.transform.Find("Canvas").GetComponent<RectTransform>();
            transform.rotation  = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            canvasRect.rotation = canvasRotation;
            controller.Move(direction * movementSpeed * Time.deltaTime);
        }
        else if (animator.GetFloat("MoveSpeed") > 0)
        {
            if (animator.speed != 1)
            {
                animator.speed = 1;
            }

            animator.SetFloat("MoveSpeed", animator.GetFloat("MoveSpeed") - 0.08f);
        }

        // Passes incoming messages from controller to SampleMessageListener, which then logs the message
        if (message != null)
        {
            if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
            {
                messageListener.SendMessage("OnConnectionEvent", true);
            }
            else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
            {
                messageListener.SendMessage("OnConnectionEvent", false);
            }
            else
            {
                messageListener.SendMessage("OnMessageArrived", message);
            }
        }
    }

    private void ButtonOne()
    {
        if (GameManager.Instance.GameOver || GameManager.Instance.IsPaused)
        {
            if (GameManager.Instance.GameOver)
            {
                GameManager.Instance.MainMenu();
            }
            else if (GameManager.Instance.IsPaused)
            {
                PauseMenu.Click();
            }
        }
        else
        {
            if (player.CloseIngredient != null && player.HeldPizza == null)
            {
                Ingredient.TakeIngredient(player.CloseIngredient.IngredientScriptableObject, player);
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

            if (player.ClosePizza != null && player.ClosePizza.Ingredients.Count.Equals(0) &&
                player.HeldPizza  != null && player.HeldPizza.cookState != HeldPizzaSO.CookState.Burnt)
            {
                player.PutDownPizza();
                return;
            }

            // Checks that player is close pizza oven, player is holding pizza and pizza cookState is not either cooked or burnt.
            if (player.ClosePizzaOven      != null && !player.ClosePizzaOven.PizzaInOven && player.HeldPizza != null &&
                player.HeldPizza.cookState != HeldPizzaSO.CookState.Cooked &&
                player.HeldPizza.cookState != HeldPizzaSO.CookState.Burnt)
            {
                player.ClosePizzaOven.AddPizzaToOven(player.HeldPizza, player);
                return;
            }

            if (player.ClosePizzaOven != null && player.ClosePizzaOven.PizzaInOven && player.HeldPizza == null)
            {
                player.ClosePizzaOven.TakePizza(player);
                return;
            }

            if (player.CloseDeliveryPoint  != null && player.HeldPizza != null &&
                player.HeldPizza.cookState == HeldPizzaSO.CookState.Cooked)
            {
                player.CloseDeliveryPoint.DeliverPizza(player.HeldPizza, player);
                return;
            }

            if (player.CloseTrashBin != null)
            {
                if (player.HeldPizza != null)
                {
                    player.DiscardPizza();
                    return;
                }

                if (player.HeldIngredient != null)
                {
                    player.HeldIngredient = null;
                    return;
                }

                return;
            }
        }
    }

    private void ButtonTwo()
    {
        switch (GameManager.Instance.GameOver)
        {
            case false when !GameManager.Instance.IsPaused && dashCooldown >= DefaultValues.dashCooldownLength:
                StartCoroutine("Dash");
                break;
            case true:
                GameManager.Instance.Restart();
                break;
        }
    }

    // Pause button
    private static void ButtonThree()
    {
        if (!GameManager.Instance.IsPaused)
        {
            GameManager.Instance.Pause();
        }
        else
        {
            GameManager.Instance.Unpause();
        }
    }

    private IEnumerator Dash()
    {
        cooldown = false;
        float timer   = 2;
        var   maxTime = timer;

        var go = Instantiate(dashEffect);
        go.transform.position = transform.position;
        var particle = go.GetComponent<ParticleSystem>();
        particle.Play();
        // Cooldown counts from 0 to value of DefaultValues.dashCooldownLength.
        dashCooldown  =  0;
        movementSpeed += 2;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            var percent = timer / maxTime;
            dashProgressBar.fillAmount = Mathf.Lerp(0, 1, percent);
            yield return new WaitForEndOfFrame();
        }

        cooldown = true;
        particle.Stop();
        Destroy(go);
        movementSpeed = DefaultValues.defaultMovementSpeed;
    }
}

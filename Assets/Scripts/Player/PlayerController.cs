using UnityEngine;
public class PlayerController : SerialController
{

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
        if (controller == null)
            controller = gameObject.GetComponent<CharacterController>();
        if (player == null)
            player = gameObject.GetComponent<Player>();
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
        int message;
        if (!keyboardMovement)
            message = (int)serialThread.ReadMessage();
        else
            message = 0;

        if (Input.GetKeyDown(KeyCode.E))
            ButtonOne();
        if (Input.GetKeyDown(KeyCode.F))
            ButtonTwo();

        // If keyboard is in use then calculates horizontal and vertical inputs from keyboard.
        if (keyboardMovement)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        // Calculates horizontal and vertical inputs from serial messages.
        else
        {
            switch (message)
            {
                case (int)MOVE.LEFT:
                    horizontal = -1;
                    break;
                case (int)MOVE.STOPLEFT:
                    horizontal = 0;
                    break;
                case (int)MOVE.RIGHT:
                    horizontal = 1;
                    break;
                case (int)MOVE.STOPRIGHT:
                    horizontal = 0;
                    break;
                case (int)MOVE.DOWN:
                    vertical = -1;
                    break;
                case (int)MOVE.STOPDOWN:
                    vertical = 0;
                    break;
                case (int)MOVE.UP:
                    vertical = 1;
                    break;
                case (int)MOVE.STOPUP:
                    vertical = 0;
                    break;
                case 0:
                    return;
                default:
                    break;
            }
        }

        Vector3 newPos = new Vector3(transform.position.x + horizontal, transform.position.y, transform.position.z + vertical);
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 dir = newPos - transform.position;

        if (direction.magnitude >= 0.1f)
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            RectTransform canvasRect = gameObject.transform.Find("Canvas").GetComponent<RectTransform>();
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            canvasRect.rotation = canvasRotation;
            controller.Move(direction * movementSpeed * Time.deltaTime);
        }




        // Passes incoming messages from controller to SampleMessageListener, which then logs the message
        if (message != 0)
        {
            if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
                messageListener.SendMessage("OnConnectionEvent", true);
            else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
                messageListener.SendMessage("OnConnectionEvent", false);
            else
                messageListener.SendMessage("OnMessageArrived", message);
        }
    }

    private void ButtonOne()
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

        if (player.ClosePizza != null && player.HeldPizza == null)
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

    private void ButtonTwo()
    {
        
    }
}
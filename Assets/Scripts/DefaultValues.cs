using UnityEngine;

/// <summary>
///     Contains all default values used throughout the game
/// </summary>
public static class DefaultValues
{
    /// <summary>
    ///     Minimum value for pizza timer to have.
    /// </summary>
    public static float minTimerValue = 90;

    /// <summary>
    ///     Maximum value for pizza timer to have.
    /// </summary>
    public static float maxTimerValue = 150;

    /// <summary>
    ///     Duration of the game.
    /// </summary>
    public static float gameDuration = 300;

    /// <summary>
    ///     Value that lerpTime will be increased each time.
    /// </summary>
    public static float lerpTimeIncrement = 0.0003f;

    /// <summary>
    ///     Amount of time until pizza is burnt
    /// </summary>
    //public static float burnTime = 25;

    /// <summary>
    ///     Amount of points that pizza is worth by default.
    /// </summary>
    public static int pizzaPointValue = 50;

    public static float fastDeliveryThreshold = 0.6f;

    /// <summary>
    ///     Amount of bonus points that are awarded on fast delivery.
    /// </summary>
    public static int fastDeliveryBonus = 10;

    /// <summary>
    ///     Default player movement speed.
    /// </summary>
    public static int defaultMovementSpeed = 4;

    /// <summary>
    ///     Default cooldown length of dash.
    /// </summary>
    public static float dashCooldownLength = 6;

    /// <summary>
    ///     Default scale of non selected ui buttons.
    /// </summary>
    public static Vector3 defaultButtonScale = new Vector3(1, 1, 1);

    /// <summary>
    ///     Scale value of selected ui buttons.
    /// </summary>
    public static Vector3 selectedButtonScale = new Vector3(1.1f, 1.1f, 1.1f);
}

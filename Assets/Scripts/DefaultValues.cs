using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains all default values used throughout the game
/// </summary>
public static class DefaultValues
{
    /// <summary>
    /// Minimum value for pizza timer to have.
    /// </summary>
    public static float minTimerValue = 90;
    /// <summary>
    /// Maximum value for pizza timer to have.
    /// </summary>
    public static float maxTimerValue = 150;
    /// <summary>
    /// Duration of the game.
    /// </summary>
    public static float gameDuration = 10;
    /// <summary>
    /// Value that lerpTime will be increased each time.
    /// </summary>
    public static float lerpTimeIncrement = 0.0003f;
    /// <summary>
    /// Amount of time until pizza is burnt
    /// </summary>
    public static float burnTime = 25;
    /// <summary>
    /// Amount of points that pizza is worth by default.
    /// </summary>
    public static int pizzaPointValue = 50;
    public static float fastDeliveryThreshold = 0.6f;
    /// <summary>
    /// Amount of bonus points that are awarded on fast delivery.
    /// </summary>
    public static int fastDeliveryBonus = 10;
    public static int defaultMovementSpeed = 4;
}

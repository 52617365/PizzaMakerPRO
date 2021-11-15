using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles player deliveries.
/// </summary>
public class DeliveryPoint : MonoBehaviour
{
    [SerializeField]
    private Color[] defaultColors;

    [HideInInspector]
    public Outline outline;
    private bool pizzaDelivered;

    public Material highlightMaterial { get; private set; }

    public Color[] TopMaterialColor { get { return defaultColors; } }

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        foreach (var material in renderer.materials)
        {
            if (material.name == "highlight (Instance)")
            {
                highlightMaterial = material;
                break;
            }
        }
    }

    public void DeliverPizza(HeldPizzaSO pizza, Player player)
    {
        int correctCount;
        float pointPercentage = 0;

        // Loops through all CurrentOrders.
        foreach (var order in GameManager.Instance.CurrentOrders)
        {
            // Int value that tracks amount of correct ingredients.
            correctCount = 0;

            // Loops through all ingredients of order.
            for (int i = 0; i < order.UIElement.Ingredients.Count; i++)
            {
                // Loops through ingredients of pizza that player is trying to deliver.
                for (int x = 0; x < pizza.ingredients.Count; x++)
                {
                    // Checks if ingredient equals to ingredient in order.
                    if (pizza.ingredients[x] == order.UIElement.Ingredients[i])
                        correctCount++;
                }
                // If all ingredients in delivered pizza matches to any of CurrentOrders this will then finish delivery.
                if (correctCount == pizza.ingredients.Count && correctCount == order.UIElement.Ingredients.Count)
                {
                    pointPercentage = order.UIElement.RemainingTime / order.UIElement.MaxTime;
                    GameManager.Instance.CurrentOrders.Remove(order);

                    GameManager.Instance.ClearOrder(order);

                    player.ClearActiveIcons();
                    player.HeldPizza.ingredients.Clear();
                    player.HeldPizza.cookState = HeldPizzaSO.CookState.Uncooked;
                    player.HeldPizza = null;
                    Destroy(player.instantiatedGameObject);
                    player.instantiatedGameObject = null;
                    player.GetComponent<Animator>().SetFloat("Holding", 0);
                    pizzaDelivered = true;
                    break;
                }
            }
            if (pizzaDelivered)
                break;
        }
        if (!pizzaDelivered)
            return;

        // TODO: Add score for delivering correct pizza within time limit.
        GetComponent<AudioSource>().Play();
        GameManager.UpdateScore(pointPercentage);
        pizzaDelivered = false;
    }
}

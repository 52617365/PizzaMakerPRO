using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
///     Handles player deliveries.
/// </summary>
public class DeliveryPoint : MonoBehaviour
{
    [SerializeField] private Color[] defaultColors;

    public GameObject errorIcon;
    public GameObject[] errorMessages; // 0 = Burnt pizza, 1 = Pizza not cooked, 2 = Ingredient mismatch.

    private Coroutine errorCoroutine;

    private bool pizzaDelivered;

    public Material HighlightMaterial { get; private set; }

    public Color[] TopMaterialColor => defaultColors;

    private void Awake()
    {
        var rendererComponent = GetComponent<Renderer>();

        foreach (Material material in rendererComponent.materials)
        {
            if (material.name == "highlight (Instance)")
            {
                HighlightMaterial = material;
                break;
            }
        }
    }

    public void DeliverPizza(HeldPizzaSO pizza, Player player)
    {
        float pointPercentage = 0;

        // Loops through all CurrentOrders.
        foreach (OrderSO order in GameManager.Instance.CurrentOrders)
        {
            // Int value that tracks amount of correct ingredients.
            var correctCount = 0;

            // Loops through all ingredients of order.
            foreach (IngredientSO t in order.UIElement.Ingredients)
            {
                // Loops through ingredients of pizza that player is trying to deliver.
                correctCount += pizza.ingredients.Count(t1 => t1 == t);

                // If all ingredients in delivered pizza matches to any of CurrentOrders this will then finish delivery.
                if (correctCount == pizza.ingredients.Count && correctCount == order.UIElement.Ingredients.Count)
                {
                    pointPercentage = order.UIElement.RemainingTime / order.UIElement.MaxTime;
                    GameManager.Instance.CurrentOrders.Remove(order);

                    GameManager.ClearOrder(order);

                    player.ClearActiveIcons();
                    player.HeldPizza.ingredients.Clear();
                    player.HeldPizza.cookState = HeldPizzaSO.CookState.Uncooked;
                    player.HeldPizza = null;
                    Destroy(player.InstantiatedGameObject);
                    player.InstantiatedGameObject = null;
                    player.GetComponent<Animator>().SetFloat("HoldingPizzaBox", 0);
                    pizzaDelivered = true;
                    break;
                }
            }

            if (pizzaDelivered)
            {
                break;
            }
        }

        if (!pizzaDelivered)
        {
            if (errorCoroutine != null)
            {
                StopCoroutine(errorCoroutine);
                foreach (GameObject go in errorMessages)
                {
                    if (go.activeSelf)
                    {
                        go.SetActive(false);
                    }
                }

                errorIcon.SetActive(false);
            }

            errorCoroutine = StartCoroutine(ShowDeliveryError(2));
            return;
        }

        // TODO: Add score for delivering correct pizza within time limit.
        GetComponent<AudioSource>().Play();
        GameManager.UpdateScore(pointPercentage);
        if (!GameManager.Instance.disablePlayerScoring)
        {
            player.UpdateScore(pointPercentage);
        }
        pizzaDelivered = false;
    }

    public void ShowBurntPizzaError()
    {
        if (errorCoroutine != null)
        {
            StopCoroutine(errorCoroutine);
            foreach (GameObject go in errorMessages)
            {
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }
            }

            errorIcon.SetActive(false);
        }

        errorCoroutine = StartCoroutine(ShowDeliveryError(0));
    }

    public void ShowNotCookedError()
    {
        if (errorCoroutine != null)
        {
            StopCoroutine(errorCoroutine);
            foreach (GameObject go in errorMessages)
            {
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }
            }

            errorIcon.SetActive(false);
        }

        errorCoroutine = StartCoroutine(ShowDeliveryError(1));
    }

    private IEnumerator ShowDeliveryError(int index)
    {
        errorMessages[index].SetActive(true);
        errorIcon.SetActive(true);
        yield return new WaitForSeconds(2f);
        errorMessages[index].SetActive(false);
        errorIcon.SetActive(false);
        errorCoroutine = null;
    }
}

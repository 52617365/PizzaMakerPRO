using UnityEngine;

[CreateAssetMenu(fileName = "OrderSO", menuName = "ScriptableObjects/Pizza/OrderSO", order = 3)]
public class OrderSO : ScriptableObject
{
    public bool isInUse;
    public PizzaUI UIElement;
}
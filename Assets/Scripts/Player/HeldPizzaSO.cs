using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeldPizzaSO", menuName = "ScriptableObjects/Pizza/HeldPizzaSO", order = 4)]
public class HeldPizzaSO : ScriptableObject
{
    public enum CookState { Uncooked, Cooked, Burnt };
    public CookState cookState;
    public List<IngredientSO> ingredients;
}

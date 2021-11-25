using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Scriptable object that is added into PizzaDatabase
/// </summary>
[CreateAssetMenu(fileName = "PizzaSO", menuName = "ScriptableObjects/Pizza/PizzaSO", order = 2)]
public class PizzaSO : ScriptableObject
{
    public string pizzaName;

    // List of scriptable object ingredients
    public List<IngredientSO> ingredients;
}
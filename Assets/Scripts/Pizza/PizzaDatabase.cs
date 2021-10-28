using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database of all pizzas from where game will
/// pick randomly pizzas for player to prepare.
/// </summary>
[CreateAssetMenu(fileName = "PizzaDatabase", menuName = "ScriptableObjects/Pizza/PizzaDatabase", order = 1)]
public class PizzaDatabase : ScriptableObject
{
    // List of ScriptableObjects
    public List<PizzaSO> pizzas;
}

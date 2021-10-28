using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class Ingredient : MonoBehaviour
{
    [HideInInspector]
    public Outline outline;
    /// <summary>
    /// Ingredient scriptable object
    /// </summary>
    [SerializeField]
    private IngredientSO ingredientScriptableObject;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private Image icon;

    public IngredientSO IngredientScriptableObject
    {
        get { return ingredientScriptableObject; }
    }

    private void Awake()
    {
        outline = GetComponent<Outline>();
        // Sets the icon to be the icon that is specified
        // in ingredientScriptableObject.
        if(icon != null && ingredientScriptableObject.icon != null)
            icon.sprite = ingredientScriptableObject.icon;
    }

    /// <summary>
    /// Handles the ingredient pick up system
    /// </summary>
    public void TakeIngredient(IngredientSO ingredientSO, Player player)
    {
        Debug.Log("Picked up " + ingredientSO);
        player.HeldIngredient = ingredientSO;
        player.IngredientIcon.sprite = ingredientSO.icon;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class Ingredient : MonoBehaviour
{
    [SerializeField]
    private Texture defaultTexture;
    [SerializeField]
    private Texture highlightTexture;
    private Material highlightMaterial;
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

    public Texture HighLightTexture { get { return highlightTexture; } }
    public Texture DefaultTexture { get { return defaultTexture; } }
    public Material HighLightMaterial { get { return highlightMaterial; } }
    public IngredientSO IngredientScriptableObject
    {
        get { return ingredientScriptableObject; }
    }

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();

        foreach (var material in renderer.materials)
        {
            if (material.name == "CheeseMaterial (Instance)" || material.name == "HamMaterial (Instance)"
                || material.name == "KebabMaterial (Instance)" || material.name == "PepperoniMaterial (Instance)"
                || material.name == "PineappleMaterial (Instance)" || material.name == "SalamiMaterial (Instance)"
                || material.name == "ShrimpMaterial (Instance)" || material.name == "TomatoMaterial (Instance)" 
                || material.name == "TunaMaterial (Instance)" || material.name == "Blue-CheeseMaterial (Instance)")
                highlightMaterial = material;
        }
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
        player.HeldIngredient = ingredientSO;
        player.IngredientIcon.sprite = ingredientSO.icon;
    }

}

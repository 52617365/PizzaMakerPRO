using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// </summary>
public class Ingredient : MonoBehaviour
{
    [SerializeField] private Texture defaultTexture;
    [SerializeField] private Texture highlightTexture;

    /// <summary>
    ///     Ingredient scriptable object
    /// </summary>
    [SerializeField] private IngredientSO ingredientScriptableObject;

    /// <summary>
    /// </summary>
    [SerializeField] private Image icon;

    public string[] materialNames =
    {
        "CheeseMaterial (Instance)",
        "HamMaterial (Instance)",
        "KebabMaterial (Instance)",
        "PepperoniMaterial (Instance)",
        "PineappleMaterial (Instance)",
        "SalamiMaterial (Instance)",
        "ShrimpMaterial (Instance)",
        "TomatoMaterial (Instance)",
        "TunaMaterial (Instance)",
        "Blue-CheeseMaterial (Instance)"
    };

    public Texture HighLightTexture => highlightTexture;
    public Texture DefaultTexture => defaultTexture;
    public Material HighLightMaterial { get; private set; }

    public IngredientSO IngredientScriptableObject => ingredientScriptableObject;

    private void Awake()
    {
        var rendererComponent = GetComponent<Renderer>();

        foreach (Material material in rendererComponent.materials)
            // Checking if materialNames array contains the material.name in iteration.
        {
            if (Array.Exists(materialNames, x => x == material.name))
            {
                HighLightMaterial = material;
            }
        }

        // Sets the icon to be the icon that is specified
        // in ingredientScriptableObject.
        if (icon != null && ingredientScriptableObject.icon != null)
        {
            icon.sprite = ingredientScriptableObject.icon;
        }
    }

    /// <summary>
    ///     Handles the ingredient pick up system
    /// </summary>
    public static void TakeIngredient(IngredientSO ingredientSO, Player player)
    {
        player.HeldIngredient = ingredientSO;
        player.IngredientIcon.sprite = ingredientSO.icon;
    }
}

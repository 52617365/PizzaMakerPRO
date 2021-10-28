using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test script for changing material color
/// </summary>
public class MaterialTest : MonoBehaviour
{
    [SerializeField]
    private Material lightMaterial;
    [SerializeField]
    private List<Color> defaultColors;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        foreach (var material in renderer.materials)
        {
            if (material.name == "Light (Instance)")
                lightMaterial = material;
        }
        lightMaterial.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            lightMaterial.color = defaultColors[1];
            lightMaterial.SetColor("_EmissionColor", Color.red);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerControl : MonoBehaviour
{
    [SerializeField]
    private bool isFlickering = false;
    [SerializeField]
    private float timeDelay;
    private Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    private void Update()
    {
        if (isFlickering == false)
            StartCoroutine(FlickeringLight());
    }

    private IEnumerator FlickeringLight()
    {
        isFlickering = true;
        Material material = renderer.materials[0];
        while (isFlickering)
        {
            material.SetColor("_EmissionColor", Color.black);
            timeDelay = Random.Range(1.3f, 3f);
            yield return new WaitForSeconds(timeDelay);
            material.SetColor("_EmissionColor", Color.white);
            timeDelay = Random.Range(0.4f, 2f);
            yield return new WaitForSeconds(timeDelay);
        }
    }
}

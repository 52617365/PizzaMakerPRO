using System.Collections;
using UnityEngine;

public class FlickerControl : MonoBehaviour
{
    [SerializeField] private bool isFlickering;

    [SerializeField] private float timeDelay;


    private void Update()
    {
        if (isFlickering == false)
        {
            StartCoroutine(FlickeringLight());
        }
    }

    private IEnumerator FlickeringLight()
    {
        isFlickering = true;
        Color color = Color.white * 4;
        Material material = GetComponent<Renderer>().materials[0];
        while (isFlickering)
        {
            material.SetColor("_EmissionColor", Color.black);
            timeDelay = Random.Range(1.3f, 3f);
            yield return new WaitForSeconds(timeDelay);
            material.SetColor("_EmissionColor", color);
            timeDelay = Random.Range(0.4f, 2f);
            yield return new WaitForSeconds(timeDelay);
        }
    }
}
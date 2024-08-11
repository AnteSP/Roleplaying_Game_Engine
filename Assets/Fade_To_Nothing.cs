using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade_To_Nothing : MonoBehaviour
{
    [SerializeField] AudioSource au;
    // The duration in seconds over which the object should disappear
    public float disappearDuration = 5.0f;

    // The initial and final alpha values for fading
    private float initialAlpha = 1.0f;
    private float finalAlpha = 0.0f;

    // The time at which the script was enabled
    private float startTime;

    // Reference to the Renderer component
    private Image objectRenderer;

    private void OnEnable()
    {
        au.Play();
        // Get the current time
        startTime = Time.time;

        // Get the Renderer component of the GameObject
        objectRenderer = GetComponent< Image>();

        if (objectRenderer == null)
        {
            Debug.LogError("Renderer component not found on the GameObject.");
            return;
        }

        // Set the initial alpha value
        SetAlpha(initialAlpha);
    }

    private void Update()
    {
        // Calculate the elapsed time
        float elapsedTime = Time.time - startTime;

        // Calculate the current alpha value based on the elapsed time
        float currentAlpha = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / disappearDuration);

        // Set the current alpha value
        SetAlpha(currentAlpha);

        // Check if the disappear duration has been reached
        if (elapsedTime >= disappearDuration)
        {
            // Disable the GameObject
            //gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        objectRenderer.color = new Color(objectRenderer.color.r, objectRenderer.color.g, objectRenderer.color.b, alpha);
    }
}

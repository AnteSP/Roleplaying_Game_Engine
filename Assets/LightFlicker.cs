using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D lightSource; // The light component to flicker
    public float minIntensity = 0.5f; // Minimum light intensity
    public float maxIntensity = 1.5f; // Maximum light intensity
    public float flickerSpeed = 0.1f; // Speed of flickering effect

    private float targetIntensity; // Target intensity to reach
    private float currentVelocity = 0f; // Velocity for smooth damping

    void Start()
    {
        if (lightSource == null)
        {
            lightSource = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        }
    }

    void Update()
    {
        // Randomly set the target intensity within the specified range
        targetIntensity = Random.Range(minIntensity, maxIntensity);

        // Smoothly transition to the target intensity
        lightSource.intensity = Mathf.SmoothDamp(lightSource.intensity, targetIntensity, ref currentVelocity, flickerSpeed);
    }
}

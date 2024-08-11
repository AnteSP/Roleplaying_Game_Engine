using UnityEngine;
using System.Collections;

public class CameraSway : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public CamZoom camTransform;

    // How long the object should shake for.
    public float swayFreq = 5f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float swayAmount = 0.7f;
    public float swayNoise = 0.7f;
    public float cumNoise = 0;
    public Vector2 sway = new Vector2(0,0);
    public float relativeX = 1;
    public float swayTime = 0;

    public Vector3 originalPos;

    void OnEnable()
    {
        if (camTransform == null) camTransform = GetComponent(typeof(CamZoom)) as CamZoom;
    }

    void Update()
    {
        camTransform.AddToFocusPoint = sway;

        swayTime += Time.deltaTime + cumNoise;
        cumNoise += Random.Range(-swayNoise/100, swayNoise/100);
        cumNoise *= 0.8f;

        sway.x = Mathf.Sin(swayTime*swayFreq*relativeX)*swayAmount;
        sway.y = Mathf.Cos(swayTime * swayFreq) * swayAmount;
    }
}
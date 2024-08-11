using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public CamZoom camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;
    public bool disableAll = false;
    public float SD = 0;

    public Vector3 originalPos;

    void OnEnable()
    {
        if (camTransform == null) camTransform = GetComponent(typeof(CamZoom)) as CamZoom;
        SD = shakeDuration;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.AddToFocusPoint += (Vector2)( Random.insideUnitSphere * shakeAmount*(shakeDuration/SD));

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.AddToFocusPoint = Vector2.zero;
            if (disableAll)
            {
                gameObject.SetActive(false);
                shakeDuration = SD;
            }
            else
            {
                shakeDuration = SD;
                this.enabled = false;
            }
            
        }
    }
}
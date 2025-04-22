using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TESTING : MonoBehaviour
{
    public Volume volume;
    public VolumeProfile fromProfile;
    public VolumeProfile toProfile;
    public float transitionDuration = 2f;

    private ColorAdjustments fromColorAdjustments, toColorAdjustments, currentColorAdjustments;
    private float transitionProgress;
    private bool isTransitioning;

    void Start()
    {
        // Ensure we have a current profile to modify
        if (volume.profile == null)
        {
            volume.profile = Instantiate(fromProfile);
        }

        // Get the current overrides
        volume.profile.TryGet(out currentColorAdjustments);
        fromProfile.TryGet(out fromColorAdjustments);
        toProfile.TryGet(out toColorAdjustments);
        StartTransition();
    }

    public void StartTransition()
    {
        transitionProgress = 0f;
        isTransitioning = true;
    }

    void Update()
    {
        if (!isTransitioning) return;

        transitionProgress += Time.deltaTime / transitionDuration;
        transitionProgress = Mathf.Clamp01(transitionProgress);

        // Interpolate each property
        currentColorAdjustments.contrast.Override(
            Mathf.Lerp(fromColorAdjustments.contrast.value,
                      toColorAdjustments.contrast.value,
                      transitionProgress));

        currentColorAdjustments.saturation.Override(
            Mathf.Lerp(fromColorAdjustments.saturation.value,
                      toColorAdjustments.saturation.value,
                      transitionProgress));

        currentColorAdjustments.postExposure.Override(
            Mathf.Lerp(fromColorAdjustments.postExposure.value,
                      toColorAdjustments.postExposure.value,
                      transitionProgress));

        currentColorAdjustments.colorFilter.Override(
            Color.Lerp(fromColorAdjustments.colorFilter.value,
                      toColorAdjustments.colorFilter.value,
                      transitionProgress));

        if (transitionProgress >= 1f)
        {
            isTransitioning = false;
        }
    }
}
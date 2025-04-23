using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayLightCycle : MonoBehaviour
{
    Light2D[] streetLights;
    [SerializeField] GameObject streetLightsParent;
    bool isday = true;

    [SerializeField] Volume camVol;

    [Tooltip("minute 0 = midnight. 1439 = 11:59pm")]
    [SerializeField] int nightEndM, middayStartM, middayEndM, nightStartM;

    [SerializeField] VolumeProfile nightProfile,dayProfile,sunriseProfile;

    private ColorAdjustments fromColorAdjustments, toColorAdjustments, currentColorAdjustments;
    private float transitionProgress,lightIntensity;
    [SerializeField] float maxLightIntensity = 2;

    // Start is called before the first frame update
    void Start()
    {
        if (streetLightsParent != null)
        {
            streetLights = streetLightsParent.GetComponentsInChildren<Light2D>();
            updateStreetLights(!isday, maxLightIntensity);
        }
        camVol.profile.TryGet(out currentColorAdjustments);
        sunriseProfile.TryGet(out fromColorAdjustments);
        dayProfile.TryGet(out toColorAdjustments);
    }

    bool holdingDay = false, holdingNight = false;

    void transitionSetup(VolumeProfile from, VolumeProfile to)
    {
        if (transitionProgress < 0.3f)//going to sunrise
        {
            from.TryGet(out fromColorAdjustments);
            sunriseProfile.TryGet(out toColorAdjustments);
            transitionProgress *= 10f/3f;
        }else if(transitionProgress < 0.7f)//holding sunrise
        {
            sunriseProfile.TryGet(out fromColorAdjustments);
            sunriseProfile.TryGet(out toColorAdjustments);
        }
        else//leaving sunrise
        {
            sunriseProfile.TryGet(out fromColorAdjustments);
            to.TryGet(out toColorAdjustments);
            transitionProgress -= 0.7f;
            transitionProgress *= 10f / 3f;
        }
    }

    //1440 minutes in a day
    public void setDaylightForMinute(int m)
    {
        if(m < 0 || m > 1440)
        {
            print("ERROR: minutes > 1440 or < 0 (" + m + ")");
            return;
        }
        

        if(m < nightEndM || m > nightStartM)//hold nighttime
        {
            if (holdingNight) return;
            sunriseProfile.TryGet(out fromColorAdjustments);
            nightProfile.TryGet(out toColorAdjustments);
            transitionProgress = 1;
            lightIntensity = maxLightIntensity;
            holdingDay = false;
            holdingNight = true;
        }
        else if(m < middayStartM)//transition into midday
        {
            transitionProgress = (float)(m - nightEndM) / (float)(middayStartM- nightEndM);
            isday = (transitionProgress > 0.2f);
            lightIntensity = (maxLightIntensity / 10f) + ((maxLightIntensity / 10f) * 9 * (1-(transitionProgress*4)));

            transitionSetup(nightProfile, dayProfile);
            
            holdingDay = false;
            holdingNight = false;
        }
        else if(m < middayEndM)//hold midday
        {
            if (holdingDay) return;
            sunriseProfile.TryGet(out fromColorAdjustments);
            dayProfile.TryGet(out toColorAdjustments);
            lightIntensity = maxLightIntensity / 10f;
            transitionProgress = 1;
            holdingDay = true;
            holdingNight = false;
        }
        else if(m <= nightStartM)//transition into night
        {
            transitionProgress = (float)(m - middayEndM) / (float)(nightStartM - middayEndM);
            isday = (transitionProgress < 0.8f);
            lightIntensity = (-15*maxLightIntensity / 10f) + (maxLightIntensity / 10f) * 25* transitionProgress;

            transitionSetup(dayProfile , nightProfile);
            
            holdingNight = false;
            holdingDay = false;
        }

        Stats.Debug(m + "  " + transitionProgress);
        updateStreetLights(!isday, lightIntensity);
        updateDayLight(transitionProgress);
    }

    void updateStreetLights(bool on,float light)
    {
        if (streetLights == null) return;
        foreach (Light2D l in streetLights)
        {
            l.enabled = on;
            l.intensity = light;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transitionProgress">must be between 0 and 1</param>
    void updateDayLight(float transitionProgress)
    {
        if (currentColorAdjustments == null) return;
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
    }
}

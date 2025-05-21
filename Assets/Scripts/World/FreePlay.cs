using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePlay : MonoBehaviour
{
    public static bool freePlayModeOn { get; private set; } = false;

    [SerializeField] GameObject[] disableOnFreePlay;
    [SerializeField] GameObject[] enableOnlyOnFreePlay;

    [SerializeField] UnityEngine.Rendering.VolumeProfile freePlayProfile = null;
    static UnityEngine.Rendering.VolumeProfile OGProfile;

    public static void StartFreePlay()
    {
        freePlayModeOn = true;
    }

    public static void resetFreePlay()
    {
        freePlayModeOn = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (freePlayModeOn)
        {
            foreach (GameObject g in disableOnFreePlay)
            {
                g.SetActive(false);
            }

            if(freePlayProfile != null)
            {
                OGProfile = Camera.main.GetComponent<UnityEngine.Rendering.Volume>().profile;
                Camera.main.GetComponent<UnityEngine.Rendering.Volume>().profile = freePlayProfile;
            }

            Stats.StartStopTime(true, "freeplay");
            //GameObject.FindGameObjectWithTag("SPECIAL").GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 0.3f;
        }
        foreach (GameObject g in enableOnlyOnFreePlay)
        {
            g.SetActive(freePlayModeOn);
        }
    }

    public static void resetCamProfile()
    {
        Camera.main.GetComponent<UnityEngine.Rendering.Volume>().profile = OGProfile;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

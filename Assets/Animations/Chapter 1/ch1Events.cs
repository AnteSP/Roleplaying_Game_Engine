using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ch1Events : MonoBehaviour
{

    [SerializeField] AudioSource gunSound;
    [SerializeField] Animator[] disable;

    // Start is called before the first frame update
    void Start()
    {
        float v = Progress.getFloat("Volume");
        if (float.IsNaN(v)) return;
        gunSound.volume *= v;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void gunShot()
    {
        gunSound.Play();
        CameraShake cs = Camera.main.GetComponent<CameraShake>();
        cs.shakeAmount = 2;
        cs.shakeDuration = 1.5f;
        cs.enabled = true;
        Stats.current.FilterColor(new Color(1, 0, 0,0.3f));

        foreach(Animator g in disable)
        {
            g.enabled = false;
        }
    }
}

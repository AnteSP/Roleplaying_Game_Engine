using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CO2 : Resource
{
    float min,max;

    // Start is called before the first frame update
    void Start()
    {
        min = transform.GetChild(0).GetChild(0).localRotation.eulerAngles.z;
    
        max = (min - 180)*2;
    }

    // Update is called once per frame
    void Update()
    {
        ProgressBar.DoRotate(transform.GetChild(0).GetChild(0), min, max, Current/(float)Max,true);




    }
}

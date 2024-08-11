using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mixer : Resource
{

    Vector2 InitialB;
    Transform Bar;

    // Start is called before the first frame update
    void Start()
    {

        Bar = transform.GetChild(0).GetChild(0);
        InitialB = Bar.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        ProgressBar.DoLine(Bar, InitialB, Current / (float)Max, false);




    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeTrans : MonoBehaviour
{
    [SerializeField] Image[] Images;
    [SerializeField] Text[] Words;

    Color[] ImOg,WOg;

    // Start is called before the first frame update
    void Start()
    {
        ImOg = new Color[Images.Length];
        WOg = new Color[Words.Length];

        getBaseColors();
    }

    public void getBaseColors()
    {

        for (int i = 0; i < Images.Length; i++)
        {
            ImOg[i] = Images[i].color;

        }

        for (int i = 0; i < Words.Length; i++)
        {
            WOg[i] = Words[i].color;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DoTrans()
    {

        for(int i = 0; i < Images.Length; i++)
        {
            Images[i].color = Color.clear;


        }

        for(int i = 0; i < Words.Length; i++)
        {
            Words[i].color = Color.clear;


        }


    }

    public void UndoTrans()
    {

        for (int i = 0; i < Images.Length; i++)
        {
            Images[i].color = ImOg[i];


        }

        for (int i = 0; i < Words.Length; i++)
        {
            Words[i].color = WOg[i];


        }
    }
}

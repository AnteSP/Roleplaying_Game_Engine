using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakeTrans : MonoBehaviour
{
    [SerializeField] Image[] Images;
    [SerializeField] Text[] Words;

    Color[] ImOg,WOg,LbOg;

    // Start is called before the first frame update
    void Start()
    {
        ImOg = new Color[Images.Length];
        WOg = new Color[Words.Length];

        int count = 0;
        foreach(Image i in Images)
            foreach (TextMeshProUGUI t in i.GetComponentsInChildren<TextMeshProUGUI>())
            {
                count++;
            }
        LbOg = new Color[count];


        getBaseColors();
    }

    public void getBaseColors()
    {
        int j = 0;
        for (int i = 0; i < Images.Length; i++)
        {
            ImOg[i] = Images[i].color;
            foreach (TextMeshProUGUI t in Images[i].GetComponentsInChildren<TextMeshProUGUI>())
            {
                LbOg[j++] = t.color;
            }
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
        int j = 0;
        for(int i = 0; i < Images.Length; i++)
        {
            Images[i].color = Color.clear;

            foreach (TextMeshProUGUI t in Images[i].GetComponentsInChildren<TextMeshProUGUI>())
            {
                t.color = Color.clear;
            }
        }

        for(int i = 0; i < Words.Length; i++)
        {
            Words[i].color = Color.clear;


        }


    }

    public void UndoTrans()
    {
        int j = 0;
        for (int i = 0; i < Images.Length; i++)
        {
            Images[i].color = ImOg[i];

            foreach (TextMeshProUGUI t in Images[i].GetComponentsInChildren<TextMeshProUGUI>())
            {
                t.color = LbOg[j++];
            }
        }

        for (int i = 0; i < Words.Length; i++)
        {
            Words[i].color = WOg[i];


        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakeTrans : MonoBehaviour
{
    [SerializeField] Image[] Images;
    [SerializeField] Text[] Words;
    List<TextMeshProUGUI> childTexts = new List<TextMeshProUGUI>();

    Color[] ImOg,WOg,LbOg;

    public void forceStart()
    {
        Start();
    }

    // Start is called before the first frame update
    void Start()
    {
        ImOg = new Color[Images.Length];
        WOg = new Color[Words.Length];

        foreach(Image i in Images)
            foreach (TextMeshProUGUI t in i.GetComponentsInChildren<TextMeshProUGUI>())
            {
                childTexts.Add(t);
            }
        LbOg = new Color[childTexts.Count];

        getBaseColors();
    }

    public void getBaseColors()
    {
        for (int i = 0; i < Images.Length; i++) ImOg[i] = Images[i].color;

        for(int i = 0; i < childTexts.Count; i++) LbOg[i] = childTexts[i].color;

        for (int i = 0; i < Words.Length; i++) WOg[i] = Words[i].color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DoTrans()
    {
        for (int i = 0; i < Images.Length; i++) Images[i].color = Color.clear; ;

        for (int i = 0; i < childTexts.Count; i++) childTexts[i].color = Color.clear; ;

        for (int i = 0; i < Words.Length; i++) Words[i].color = Color.clear;
    }

    public void UndoTrans()
    {
        for (int i = 0; i < Images.Length; i++) Images[i].color = ImOg[i];

        for (int i = 0; i < childTexts.Count; i++) childTexts[i].color = LbOg[i];

        for (int i = 0; i < Words.Length; i++) Words[i].color = WOg[i];
    }
}

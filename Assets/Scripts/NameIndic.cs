using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameIndic : MonoBehaviour
{
    static TextMeshProUGUI T;
    static Image i;

    static float iniX,iniY;
    // Start is called before the first frame update
    void Start()
    {
        T = GetComponent<TextMeshProUGUI>();
        iniX = transform.parent.localPosition.x;
        iniY = transform.parent.localPosition.y;
        i = transform.parent.GetComponent<Image>();
    }

    private void Update()
    {
        transform.parent.parent.position = Input.mousePosition;
    }

    public static void Indicate(string S)
    {
        if(i == null) return;

        if (S == "")
        {
            i.enabled = false;
        }
        else
        {
            i.enabled = true;
        }
        
        T.text = S;

        //T.transform.localPosition = new Vector3((Screen.width / 2) - Input.mousePosition.x < 0 ? -iniX : iniX, T.transform.localPosition.y, T.transform.localPosition.z);
        i.transform.localPosition = new Vector3(
            (Screen.width / 2) - Input.mousePosition.x < 0 ? -iniX : iniX,
            (Screen.height / 2) - Input.mousePosition.y < 0 ? -iniY : iniY, 
            T.transform.localPosition.z);
    }
}

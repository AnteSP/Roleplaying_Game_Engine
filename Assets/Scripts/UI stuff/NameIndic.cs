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

    private void Awake()
    {
        T = GetComponent<TextMeshProUGUI>();
        iniX = transform.parent.localPosition.x;
        iniY = transform.parent.localPosition.y;
        //print(transform.parent.gameObject.name);
        i = transform.parent.GetComponent<Image>();
    }

    public static void turnOff()
    {
        i.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (i == null) return;
        //print((Input.mousePosition.x - Screen.width / 2) + i.rectTransform.rect.width - Screen.width  );
        if (i.canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            transform.parent.parent.localPosition = new Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height / 2,0) ;
            /*
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                i.canvas.GetComponent<RectTransform>(),
                Input.mousePosition,
                Camera.main,
                out Vector2 localPoint);

            transform.parent.parent.position = localPoint;*/
        }
        else
            transform.parent.parent.position = Input.mousePosition;
    }

    public static void Indicate(string S)
    {
        //print("CALLED INDICATE "  + (i==null));
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

        if(i.canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            
            
            //T.transform.localPosition = new Vector3((Screen.width / 2) - Input.mousePosition.x < 0 ? -iniX : iniX, T.transform.localPosition.y, T.transform.localPosition.z);
            i.transform.localPosition = new Vector3(
                Screen.width - (Input.mousePosition.x + i.rectTransform.rect.width) < 0 ? -iniX : iniX,
                Screen.height - (Input.mousePosition.y + i.rectTransform.rect.height) < 0 ? iniY : -iniY,
                T.transform.localPosition.z);
        }
        else
        {
            //T.transform.localPosition = new Vector3((Screen.width / 2) - Input.mousePosition.x < 0 ? -iniX : iniX, T.transform.localPosition.y, T.transform.localPosition.z);
            i.transform.localPosition = new Vector3(
                (Screen.width / 2) - Input.mousePosition.x < 0 ? -iniX : iniX,
                (Screen.height / 2) - Input.mousePosition.y < 0 ? -iniY : iniY,
                T.transform.localPosition.z);
        }


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    SpriteRenderer Spr, SprP;

    Transform TP;

    float first;

    bool done;


    // Start is called before the first frame update
    void Start()
    {
        TP = transform.parent;
        Spr = GetComponent<SpriteRenderer>();
        SprP = transform.parent.GetComponent<SpriteRenderer>();

        first = transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        float Percent = 100;

        transform.localPosition = new Vector3(first + ((0 - first)*Percent), transform.localPosition.y, transform.localPosition.z);
        transform.localScale = new Vector3(Percent, transform.localScale.y, transform.localScale.z);

    }

    static public void DoLine(Transform T,Vector2 Initial,float Percent,bool DoX)
    {

        Vector3 temp = DoX ? new Vector3(Initial.x + ((0 - Initial.x) * Percent), T.localPosition.y, T.localPosition.z) : new Vector3(T.localPosition.x, Initial.y + ((0 - Initial.y) * Percent), T.localPosition.z);

        T.localPosition = temp;
        T.localScale = DoX ? new Vector3(Percent, T.localScale.y, T.localScale.z) : new Vector3(T.localScale.x, Percent, T.localScale.z);


    }

    static public void DoLine(RectTransform T, Vector2 Initial,float EndScalePoint, float Percent, bool DoX)
    {

        Vector3 temp = DoX ? new Vector3(Initial.x + ((0 - Initial.x) * Percent), T.localPosition.y, T.localPosition.z) : new Vector3(T.localPosition.x, Initial.y + ((0 - Initial.y) * Percent), T.localPosition.z);
        T.localPosition = temp;
        T.sizeDelta = DoX ? new Vector2(Percent* EndScalePoint, T.sizeDelta.y) : new Vector2(T.sizeDelta.x, Percent* EndScalePoint);

        //T.localScale = DoX ? new Vector3(Percent, T.localScale.y, T.localScale.z) : new Vector3(T.localScale.x, Percent, T.localScale.z);


    }

    static public void DoRotate(Transform T, float min, float max, float Percent,bool LongWay)
    {

        min = LongWay ? min - 360 : min;

        T.localRotation = Quaternion.Euler(new Vector3(0, 0, min + ((max - min) * Percent)));

        float Radius = Vector2.Distance(T.localPosition, Vector2.zero);
        T.localPosition = new Vector3(Mathf.Sin((T.localRotation.eulerAngles.z/360 + 0.5f) * Mathf.PI * 2)*Radius, -Mathf.Cos((T.localRotation.eulerAngles.z/360 + 0.5f) * Mathf.PI * 2)*Radius, T.localPosition.z);



    }
}

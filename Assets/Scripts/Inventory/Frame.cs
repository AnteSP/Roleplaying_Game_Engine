using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Frame : MonoBehaviour
{

    RectTransform Rect;

    public int ID;

    public static int Target = -1;

    Image I1;
    Image TB;
    Text T;

    static Color Norm1, High, Pressed, NormT, NormTB;

    int Heir;

    Vector3 GoodPos;

    // Start is called before the first frame update
    void Start()
    {
        Rect = GetComponent<RectTransform>();
        I1 = transform.GetChild(0).GetComponent<Image>();
        TB = transform.GetChild(1).GetComponent<Image>();
        T = transform.GetChild(2).GetComponent<Text>();

        Heir = transform.GetSiblingIndex();

        if (Norm1 == Color.clear)
        {

            Norm1 = I1.color;

            High = new Color(0.7f, 0.7f, 0.7f, 1) ;
            Pressed = new Color(0.3f, 0.3f, 0.3f, 1);

            NormT = T.color;
            NormTB = TB.color;
        }

        
        
    }

    public void Click()
    {
        
        NewInv.MakeGrid(false);
        I1.color = Pressed;

        GoodPos = Rect.position;
        
    }

    public void Drag()
    {
        transform.SetAsLastSibling();
        Rect.position = Input.mousePosition;
        GetComponent<Image>().raycastTarget = false;
        I1.raycastTarget = false;
        TB.raycastTarget = false;

    }

    public void UnClick()
    {
        
        if (Target != -1)
        {
            print("UNCLICK " + gameObject.name);
            //print("OLD: ITEMS[Target]" + Items.ITEMS[Target] + " ITEMS[ID]" + Items.ITEMS[ID] + " ID" + ID + " Target" + Target);
            //Keep track of original target values so they are not lost
            int temp = Items.ITEMS[Target];
            int Qtemp = Items.ITEMQUANTITY[Target];
            //Set Target values
            Items.ITEMS[Target] = Items.ITEMS[ID];
            Items.ITEMQUANTITY[Target] = Items.ITEMQUANTITY[ID];
            //Set Held frame's values
            Items.ITEMQUANTITY[ID] = Qtemp;
            Items.ITEMS[ID] = temp;

            //Target = ID;
            Items.UpdatePics();
            //print("NEW: ITEMS[Target]" + Items.ITEMS[Target] + " ITEMS[ID]" + Items.ITEMS[ID] + " ID" + ID + " Target" + Target);

        }


        NewInv.MakeGrid(true);
        transform.SetSiblingIndex(Heir);
        GetComponent<Image>().raycastTarget = true;
        I1.raycastTarget = true;
        TB.raycastTarget = true;
        I1.color = High;
        if (!MInside(Rect,GoodPos))
        {
            GoOut();
        }
        else
        {
            TB.color = Color.clear;
        }

    }

    public void GoIn()
    {
        Items.SELECT(Items.ITEMS[ID]);
        Target = ID;
        print(Target + " is new Target");

        I1.color = High;
        T.color = Color.clear;
        TB.color = Color.clear;

    }

    public void GoOut()
    {
       // print("Go out triggered" + T.text);


        I1.color= Norm1;
        T.color = NormT;
        
        if (T.text != "")
        {
            TB.color = NormTB;
        }
        

    }

    public static bool MInside(RectTransform R,Vector3 P)
    {
        Vector3 temp = Input.mousePosition;
        Vector2 size = R.sizeDelta * 0.3f;


        return (Mathf.Clamp(temp.x,P.x - size.x, P.x + size.x) == temp.x && Mathf.Clamp(temp.y, P.y - size.y, P.y + size.y) == temp.y);

    }

    
}

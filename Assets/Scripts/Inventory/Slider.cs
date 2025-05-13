using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slider : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    //RectTransform This;

    [SerializeField] RectTransform EndPoint;

    [SerializeField] AudioSource BumpSound;

    float start,altStart;

    bool Play;

    bool Out;

    [SerializeField] Color Selected;

    [SerializeField] Sprite OutImage;
    Sprite InImage;

    Color Base;

    Image I;

    [SerializeField] RectTransform ObjToMove;

    static List<RectTransform> Handles = new List<RectTransform>();
    static List<float> Starts = new List<float>();

    [SerializeField] Sprite Retracted, Detracted;

    public bool stopTime = true;

    RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        Out = true;
        rt = GetComponent<RectTransform>();
        start = rt.position.x;

        altStart = ObjToMove.position.x;

        I = GetComponent<Image>();
        InImage = I.sprite;
        Base = I.color;

        Handles.Add(rt);
        Starts.Add(start);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData e)
    {

        I.color = new Color((Base.r + Selected.r) / 2, (Base.g + Selected.g) / 2, (Base.b + Selected.b) / 2, (Base.a + Selected.a) / 2);


    }

    void IPointerExitHandler.OnPointerExit(PointerEventData e)
    {

        I.color = Base;


    }

    void IPointerDownHandler.OnPointerDown(PointerEventData e)
    {

        /*

        I.color = Selected;

        bool Check = This.position.x - start < 1;
        This.position = new Vector3(Check ? EndPoint.position.x : start, This.position.y, This.position.z);
        I.sprite = Check ? OutImage : InImage;
        BumpSound.Play();

        */

        I.color = Selected;
        NewForcePosition(Out = !Out);
        I.sprite = Out ? Retracted : Detracted;
        if (stopTime)
        {
            SocialCell.refreshSocial();
            Stats.StartStopTime(Out, "Slider_" + ObjToMove.name);
            Stats.StartStopPlayerMovement(Out, "Slider_" + ObjToMove.name);
        }

    }
    /*
    public void Going()
    {
        bool CheckLeft = Input.mousePosition.x > start;
        bool CheckRight = Input.mousePosition.x < EndPoint.position.x;
        bool WithinBounds = CheckLeft && CheckRight;
        This.position = new Vector3(WithinBounds ? Input.mousePosition.x : This.position.x,This.position.y,This.position.z);

        

        if (!WithinBounds && Play)
        {
            This.position = new Vector3(CheckLeft ? EndPoint.position.x : start, This.position.y, This.position.z);
            BumpSound.Play();
        }

        Play = WithinBounds;

    }
    
    public void MSelected()
    {
        I.color = Selected;

        bool Check = This.position.x - start  < 1;
        This.position = new Vector3(Check ? EndPoint.position.x : start, This.position.y, This.position.z);
        I.sprite = Check ? OutImage : InImage;
        BumpSound.Play();
    }

    public void LetGo()
    {
        I.color = Base;

    }

    public void Hover()
    {

        I.color = new Color((Base.r + Selected.r) / 2, (Base.g + Selected.g) / 2, (Base.b + Selected.b) / 2, (Base.a + Selected.a) / 2);
    }

    public void MOff()
    {

        I.color = Base;
    }
    */
    public void ForcePosition(bool gotostart)
    {

        rt.position = new Vector3(!gotostart ? EndPoint.position.x : start, rt.position.y, rt.position.z);
        BumpSound.Play();
        
    }

    public static void ForceBack()
    {
        for(int i = 0; i < Handles.Count; i++)
        {
            //print(Starts[i] + " but " + Handles[i].position.x + " buuut " + Handles[i].anchoredPosition.x + " aaaand ");
            //Handles[i].position = new Vector3(Starts[i], Handles[i].position.y, Handles[i].position.z);
            
            Slider s = Handles[i].GetComponent<Slider>();
            s.ObjToMove.position = new Vector3(s.altStart, s.ObjToMove.position.y, s.ObjToMove.position.z);
            s.Out = true;
        }

    }

    public void NewForcePosition(bool gotostart)
    {

        ObjToMove.position = new Vector3(!gotostart ? EndPoint.position.x : altStart, ObjToMove.position.y, ObjToMove.position.z);
        BumpSound.Play();
        outfit.checkOutfits();
    }
    
    public static void EmptyList()
    {
        Handles.Clear();
        Starts.Clear();
    }
    
}

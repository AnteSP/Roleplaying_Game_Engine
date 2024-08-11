using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamZoom : MonoBehaviour
{

    [SerializeField] Camera C;

    [SerializeField] BoxCollider2D Col;
    Rigidbody2D Crb;

    [SerializeField] float Max = 20;
    float maxX, minX,minY,maxY;

    [SerializeField] float MTracker = 0;

    public Vector2 FocusPoint;
    public Vector2 AddToFocusPoint;
    static public CamZoom cz;

    public bool allowZooming = true;
    CameraShake cs;

    static Vector2 heldZoomPos = new Vector2(999,999);

    public void ChangeMax(int New)
    {
        MTracker = New;
        Max = New;
        Start();
        LateUpdate();
    }

    public void SetSize(int S)
    {
        C.orthographicSize = 1;
        Col.size = new Vector2(minX, minY);
        MTracker = 1;

        for(int i = 0; i < S; i++)
        {
            Col.size += new Vector2(2 * Camera.main.aspect, 2);
            C.orthographicSize = ++MTracker;
        }
    }

    private void Start()
    {
        MTracker = Camera.main.orthographicSize;

        maxX = 2 * Camera.main.aspect * (Max - MTracker);
        minX = Col.size.x - (2 * Camera.main.aspect * (MTracker - 1));

        maxX += Col.size.x;

        minY = Col.size.y - (2 * (MTracker - 1));
        maxY = (2 * (Max));
        Crb = C.GetComponent<Rigidbody2D>();
        cz = this;
        cs = C.GetComponent<CameraShake>();
        if (heldZoomPos.x != 999)
        {
            setFocusPoint(heldZoomPos);
            heldZoomPos = new Vector2(999, 999);
        }
    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (allowZooming && IsMouseOverGameWindow)//camera zooming
        {
            MTracker -= Input.mouseScrollDelta.y;
            MTracker = (MTracker > Max) ? Max : ((MTracker < 1) ? 1 : MTracker);
            C.orthographicSize = MTracker;
            Col.size = new Vector2(Camera.main.aspect, 1f) * 2f * MTracker;
        }

        cs.originalPos = FocusPoint;
        //transform.position = new Vector3(FocusPoint.x, FocusPoint.y, transform.position.z);
        //Crb.position = FocusPoint + AddToFocusPoint;
        Crb.MovePosition(FocusPoint + AddToFocusPoint);
        AddToFocusPoint = Vector2.zero;
    }

    bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }

    public static void setFocusPoint(Vector2 fp, int hsecs = 0, bool forceGoodToGo = false, bool ignorePhysics = false)
    {
        //if(Vector2.Distance(fp,))
        //cz.FocusPoint = fp;
        //cz.Crb.MovePosition(cz.FocusPoint + cz.AddToFocusPoint);
        if(hsecs == 0)
        {
            if (cz == null)
            {
                heldZoomPos = fp;
                return;
            }

            if (ignorePhysics)
            {
                Camera.main.transform.position = new Vector3(fp.x, fp.y, Camera.main.transform.position.z);
                //print("Go to " + fp.x + " " + fp.y);
            }

            cz.FocusPoint = fp;
        }
        else
        {
            cz.StartCoroutine(moveToPoint(fp, hsecs, forceGoodToGo));
        }
        
    }

    public static void applyOffset(Vector2 offset)
    {
        cz.AddToFocusPoint += offset;
    }

    static IEnumerator moveToPoint(Vector2 fp,int hsecs,bool forceGoodToGo = false)
    {
        Vector2 OGfp = cz.FocusPoint;
        for(int i = 0; i < hsecs; i++)
        {
            cz.FocusPoint = Vector2.Lerp(OGfp, fp, (float)i/(float)hsecs );
            yield return new WaitForSeconds(0.01f);
        }
        cz.FocusPoint = fp;
        if(forceGoodToGo) Dialogue.forceGoodToGo(true);
    }

    //private void OnDrawGizmos() { Gizmos.DrawSphere(FocusPoint, 1); }
}



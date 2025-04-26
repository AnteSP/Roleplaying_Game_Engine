using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    Rigidbody2D rb,Crb;

    [SerializeField] float Speed = 0.25f;
    [SerializeField] float SlowDownCoefficent = 10;
    [SerializeField] float AnimSpeed = 4;
    [SerializeField] GameObject FObj;
    bool fObjActivated = false;
    //[SerializeField] Vector2 CameraBoundsPos, CameraBoundsNeg;
    float RSpeed;

    public float SlowDown = 1;

    Animator An;

    Vector2 StartPos;

    public Vector2 FocusPoint;

    Vector2 TooFar = new Vector2(0.5f, 0.5f);

    public bool UseMouse = false;

    Vector3 LastPos;
    float timer = 0;
    SpriteRenderer spr;

    int turningFreq = 0;
    int spinObjTriggers = 0;
    [SerializeField] GameObject SpinObj;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        An = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        Crb = Camera.main.GetComponent<Rigidbody2D>();
        StartPos = transform.position;
        FocusPoint = transform.position;
        if (FObj == null) fObjActivated = true;
    }

    // Start is called before the first frame update
    void Start()
    {



    }

    public void handleCameraStuff(int Hor, int Ver, Vector2 nextPos,bool noMouseForce = false)
    {
        //print("GONE THRU " + noMouseForce);
        bool rememberMouse = UseMouse;
        if (noMouseForce) UseMouse = false;
        Vector2 temp = Offset();
        Vector2 mOffset = temp * temp * temp * 40;
        //FocusPoint = (Vector2)transform.position + (UseMouse? mOffset : Vector2.zero);
        FocusPoint = nextPos + (UseMouse ? mOffset : Vector2.zero);

        if ((Hor == 0 && Ver == 0) && Input.mousePosition != LastPos)
        {
            Face((Vector2)transform.position + mOffset);
        }
        LastPos = Input.mousePosition;

        if (IsMouseOverGameWindow || noMouseForce)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                CamZoom.setFocusPoint(nextPos);
            }
            else
            {
                CamZoom.setFocusPoint(FocusPoint);
            }

        }
        if (noMouseForce) UseMouse = rememberMouse;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (turningFreq > 0) turningFreq -= 1;
        if (turningFreq > 800)
        {
            if(SpinObj != null) handleSpinEvents();
        }
        //movement stuff
        int Hor = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        int Ver = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        RSpeed = Input.GetKey(KeyCode.LeftShift) ? Speed / SlowDownCoefficent : Speed;
        RSpeed = Input.GetKey(KeyCode.Space) ? 0 : RSpeed;

        RSpeed *= (1 / SlowDown) < 1 ? (1 / SlowDown) : 1;

        Vector2 nextPos = rb.position + new Vector2(Hor, Ver).normalized * RSpeed;
        rb.MovePosition(nextPos);
        //rb.MovePosition((Vector2)transform.localPosition + new Vector2(Hor, Ver).normalized * RSpeed);

        //Animation stuff
        An.SetInteger("Horizontal", Hor);
        An.SetInteger("Vertical", Hor == 0 ? Ver : 0);

        An.SetFloat("Speed", RSpeed*AnimSpeed);

        handleCameraStuff(Hor,Ver,nextPos);


        if (!fObjActivated && Input.GetKey(KeyCode.F))
        {
            fObjActivated = true;
            spr.enabled = false;
            FObj.SetActive(true);
            StartCoroutine(disableObjIn(0.8f,FObj));
        }
    }

    void handleSpinEvents()
    {
        SpinObj.SetActive(true);
        StartCoroutine(disableObjIn(2, SpinObj));
        turningFreq = 0;
        spinObjTriggers++;

        if (spinObjTriggers == 3)
        {
            Stats.DisplayMessage("Ok dude we fucking get it, haha funi explosion, chill");
        }
        else if (spinObjTriggers == 4)
        {
            Stats.DisplayMessage("Bro istfg if you keep doing that something's gonna break");
        }
        else if (spinObjTriggers == 5)
        {
            SpinObj.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (spinObjTriggers == 6)
        {
            SpinObj.GetComponent<SpriteRenderer>().color = Color.white;
            Stats.DisplayMessage("The explosions blue now. ITS FUCKING BLUE. Do you even know how bad that is!??!?!?");
        }
        else if (spinObjTriggers == 7)
        {
            rb.gravityScale = 30;
            Stats.DisplayMessage("Fuck you, gravity");
        }
        else if (spinObjTriggers == 8)
        {
            rb.gravityScale = -30;
            spr.flipY = true;
            Stats.DisplayMessage("Isn't your hand getting tired?");
        }
        else if (spinObjTriggers == 9)
        {
            rb.gravityScale = 0;
            spr.flipY = false;
            Stats.DisplayMessage("If you do this one more time, shit is really going to get weird. I'm warning you (Like, game breakingly bad. Save first before going any farther)");
        }
        else if (spinObjTriggers >= 10)
        {
            GameObject.Instantiate(gameObject);
            GameObject.Instantiate(gameObject);
            GameObject.Instantiate(gameObject);
        }
    }

    IEnumerator disableObjIn(float s,GameObject g)
    {
        yield return new WaitForSeconds(s);
        g.SetActive(false);
        spr.enabled = true;
    }

    bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }

    Vector2 Offset()
    {
        return new Vector2(  (Input.mousePosition.x - ((float)Screen.width) /2f) / ((float)Screen.width), (Input.mousePosition.y- ((float)Screen.height) /2f) / ((float)Screen.height) );
    }

    public void ShutDown()
    {
        if(An != null)
        {
            An.SetInteger("Horizontal", 0);
            An.SetInteger("Vertical", 0);
        }
        if(rb != null) rb.velocity = Vector2.zero;
        this.enabled = false;
    }

    public void ToggleMouse()
    {
        UseMouse = !UseMouse;
    }

    public void Respawn()
    {
        transform.position = StartPos;
    }

    string lastAnim = "";
    public void Face(Vector2 a)
    {
        string comp = "";
        a = a - rb.position;

        An.SetInteger("Horizontal", 0);
        An.SetInteger("Vertical", 0);

        An.GetBool("Drunk");

        if (Mathf.Abs(a.x) > Mathf.Abs(a.y))
        {
            comp = (a.x < 0) ? "Idle_Walk_Left" : "Idle_Walk_Right";
            An.Play((a.x < 0) ? "Idle_Walk_Left" : "Idle_Walk_Right");
            An.Play((a.x < 0) ? "Idle left" : "Idle right");
        }
        else
        {
            comp = (a.x < 0) ? "Idle_Walk_Down" : "Idle_Walk_Up";
            An.Play((a.y < 0) ? (An.GetBool("Drunk") ? "Idle Drunk Down" : "Idle down") : "Idle up");
            An.Play((a.y < 0) ? "Idle_Walk_Down" : "Idle_Walk_Up");
        }

        if (comp != lastAnim) turningFreq += 10;
        lastAnim = comp;

    }

    public void changeSpeed(float to)
    {
        Speed = to;
    }

    public void teleportTo(Transform to)
    {
        transform.position = to.position;
        CamZoom.setFocusPoint(to.position, ignorePhysics: true);
    }

}

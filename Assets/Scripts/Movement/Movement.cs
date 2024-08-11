using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    Rigidbody2D rb,Crb;

    [SerializeField] float Speed = 0.25f;
    [SerializeField] float SlowDownCoefficent = 10;
    [SerializeField] float AnimSpeed = 4;
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

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        An = GetComponent<Animator>();
        Crb = Camera.main.GetComponent<Rigidbody2D>();
        StartPos = transform.position;
        FocusPoint = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
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

        Vector2 temp = Offset();
        Vector2 mOffset = temp * temp * temp * 40;
        //FocusPoint = (Vector2)transform.position + (UseMouse? mOffset : Vector2.zero);
        FocusPoint = nextPos + (UseMouse ? mOffset : Vector2.zero);

        if ((Hor==0 && Ver == 0) && Input.mousePosition != LastPos)
        {
            Face((Vector2)transform.position + mOffset);
        }
        LastPos = Input.mousePosition;

        if (IsMouseOverGameWindow)
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

    public void Face(Vector2 a)
    {
        a = a - rb.position;

        An.SetInteger("Horizontal", 0);
        An.SetInteger("Vertical", 0);

        An.GetBool("Drunk");

        if (Mathf.Abs(a.x) > Mathf.Abs(a.y))
            An.Play((a.x < 0) ? "Idle left" : "Idle right");
        else
            An.Play((a.y < 0) ? (An.GetBool("Drunk") ? "Idle Drunk Down" : "Idle down") : "Idle up");
    }

}

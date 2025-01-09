using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCMovement : MonoBehaviour
{
    [NonSerialized] public Rigidbody2D rb;

    public float Speed = 8;//0.25f;
    public float AnimSpeed = 0.1f;//4
    [NonSerialized] public int animX, animY;

    [NonSerialized] public Animator An;

    [NonSerialized] public float Hor = 0, Ver = 0, mult = 0;
    [NonSerialized] public bool Hgreater = false;

    public bool ResetAfterTalk = false;
    public bool Facing = false;
    public bool walkThruWalls = false;

    public AudioSource OscilatingSound = null;
    float OscVol = 0;
    float OscInt = 0;
    public bool OscUp = true;
    public string overwriteResetAnim = "";


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        An = GetComponent<Animator>();
        if(OscilatingSound != null)
        {
            float f = Progress.getFloat("Volume");
            OscVol = OscilatingSound.volume * (float.IsNaN(f) ? 1 : f);

            OscilatingSound.volume = 0;
        }
        ResetAn();
    }

    public void HandleAnimVals()
    {
        Hgreater = Mathf.Abs(Hor) > Mathf.Abs(Ver);
        mult = (Hgreater) ? 1f / Mathf.Abs(Hor) : 1f / Mathf.Abs(Ver);
        mult = (float.IsNaN(mult) ? 1 : mult);

        Hor *= mult;
        Ver *= mult;

        animX = Hgreater ? (Hor > 0 ? 1 : -1) : 0;
        animY = Hgreater ? 0 : (Ver > 0 ? 1 : -1);
    }
    /// <summary>
    /// basically just gets a new OscInt (Oscilating audio interval) value
    /// </summary>
    /// <param name="APos"></param>
    public void OscPrep(Vector2 APos)
    {
        Hor = 0; Ver = 0; mult = 0;

        Hor = APos.x - rb.position.x;
        Ver = APos.y - rb.position.y;

        Vector2 temp = new Vector2(Hor, Ver);
        temp = temp / temp.magnitude;

        Vector2 newPos = rb.position + temp * Speed * 0.01f;

        OscInt = Vector2.Distance(rb.position, newPos) / (Vector2.Distance(rb.position, newPos) + Vector2.Distance(newPos, APos));
        OscInt = OscInt * OscVol;
    }

    public void OscAction()
    {
        OscilatingSound.volume += (OscUp ? 1 : -1) * OscInt;
    }

    public void Face(Vector2 a)
    {
        if (Facing)
        {
            if (!An.enabled) An.enabled = true;
            a = a - rb.position;

            An.SetInteger("Horizontal", 0);
            An.SetInteger("Vertical", 0);

            if (Mathf.Abs(a.x) > Mathf.Abs(a.y))
            {
                An.Play((a.x < 0) ? "Idle left" : "Idle right");
                An.Play((a.x < 0) ? "Idle_Walk_Left" : "Idle_Walk_Right");
            }
            else
            {
                An.Play((a.y < 0) ? "Idle down" : "Idle up");
                An.Play((a.y < 0) ? "Idle_Walk_Down" : "Idle_Walk_Up");
            }
        }
        
    }

    public void Face(string leftrightdownup)
    {
        print("FACE S");
        if (Facing)
        {
            if(!An.enabled)An.enabled = true;
            An.Play("Idle " + leftrightdownup);
            An.Play("Idle_Walk_" + leftrightdownup);
        }
    }

    public void ResetAn()
    {
        if (ResetAfterTalk) {
            //print("GOT HERE" + overwriteResetAnim + (overwriteResetAnim != ""));
            if(overwriteResetAnim != "")
                An.Play(overwriteResetAnim);
            else
                An.Play("Idle down");
        }
    }

    public void alt()
    {
        An.Play("Alt");
    }

    public void ShutDown()
    {
        An.SetInteger("Horizontal", 0);
        An.SetInteger("Vertical", 0);
        rb.velocity = Vector2.zero;
        this.enabled = false;
    }
}

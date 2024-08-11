using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWander : NPCMovement
{
    bool WMove;
    [Tooltip("Higher number = less likely to stop/go")] [SerializeField] float WanderStop=0.5f, WanderGo=3;
    Vector2 StoredW;

    


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        An = GetComponent<Animator>();
    }

    private void OnWillRenderObject()
    {

        if (Random.Range(1f, (1f / Time.deltaTime) * (WMove ? WanderStop : WanderGo)) < 2)
        {
            WMove = !WMove;
            StoredW = new Vector2(Random.Range(1, 3) * ((Random.Range(1, 3) == 1) ? -1 : 1), Random.Range(1, 3) * ((Random.Range(1, 3) == 1) ? -1 : 1));
        }

        An.SetInteger("Horizontal", 0);
        An.SetInteger("Vertical", 0);
        if (WMove)
        {
            Hor = StoredW.x;
            Ver = StoredW.y;
            HandleAnimVals();
            rb.MovePosition(rb.position + new Vector2(Hor * Speed, Ver * Speed));
            An.SetInteger("Horizontal", Hgreater ? (Hor > 0 ? 1 : -1) : 0);
            An.SetInteger("Vertical", Hgreater ? 0 : (Ver > 0 ? 1 : -1));
        }
        //Animation stuff
        An.SetFloat("Speed", Speed * AnimSpeed);

        rb.velocity = Vector2.zero;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPath : NPCMovement
{
    [SerializeField] Transform Path;
    List<Vector2> PPoints = new List<Vector2>();
    int index;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        An = GetComponent<Animator>();
        try
        {
            for (int i = 0; i < Path.childCount; i++)
            {
                PPoints.Add(Path.GetChild(i).transform.position);
            }
        }
        catch (System.Exception e)
        {

            print(e);
        }

        Hor = PPoints[index].x - transform.position.x;
        Ver = PPoints[index].y - transform.position.y;

        Hgreater = false;
        animX = 0;
        animY = 0;

        index = 0;

        HandleAnimVals();

        //Animation stuff
        An.SetInteger("Horizontal", animX);
        An.SetInteger("Vertical", animY);
        An.SetFloat("Speed", Speed * AnimSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Hor = 0;
        Ver = 0;
        mult = 0;

        Hor = PPoints[index].x - transform.position.x;
        Ver = PPoints[index].y - transform.position.y;
        Vector2 Dir = new Vector2(Hor, Ver);
        Dir /= Mathf.Sqrt(Vector2.SqrMagnitude(Dir));

        rb.MovePosition(rb.position + (Dir*Speed*0.01f));

        if (Vector2.Distance(transform.position, PPoints[index]) < 0.2f)
        {
            Hgreater = false;
            animX = 0;
            animY = 0;

            index = (index == (PPoints.Count - 1) ? 0 : index + 1);
            Hor = PPoints[index].x - transform.position.x;
            Ver = PPoints[index].y - transform.position.y;

            HandleAnimVals();

            //Animation stuff
            An.SetInteger("Horizontal", animX);
            An.SetInteger("Vertical", animY);
            An.SetFloat("Speed", Speed * AnimSpeed);
        }

        rb.velocity = Vector2.zero;
    }

    private void OnEnable()
    {
        An = GetComponent<Animator>();
        An.SetInteger("Horizontal", animX);
        An.SetInteger("Vertical", animY);
        An.SetFloat("Speed", Speed * AnimSpeed);
    }
}

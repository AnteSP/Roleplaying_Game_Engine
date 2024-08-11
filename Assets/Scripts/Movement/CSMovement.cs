using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSMovement : MonoBehaviour
{
    [SerializeField] List<Transform> APos;
    [SerializeField] List<NPCMovement> Actors;

    CutSceneTalker Dad;
    bool n = false;

    public void ensureAnimasAreReady()
    {
        Animator an = null;
        foreach (NPCMovement n in Actors)
        {
            an = n.GetComponent<Animator>();
            if (an != null) an.enabled = true;//make sure animators are on and walking is correctly animated
            //n.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void Awake()
    {
        Dad = GetComponent<CutSceneTalker>();
    }

    public void movementPrep()
    {
        for (int i = 0; i < APos.Count; i++)
        {

            if (Actors[i].OscilatingSound != null)
            {
                Actors[i].OscPrep(APos[i].position);
            }


        }
    }
    /// <summary>
    /// true if n false if N
    /// </summary>
    /// <param name="n"></param>
    public void nOrN(bool n)
    {
        this.n = n;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < APos.Count; i++)
        {
            NPCMovement N = Actors[i];

            N.Hor = 0; N.Ver = 0; N.mult = 0;

            N.Hor = APos[i].position.x - N.rb.position.x;
            N.Ver = APos[i].position.y - N.rb.position.y;

            Vector2 temp = new Vector2(N.Hor, N.Ver);
            temp = temp / temp.magnitude;

            if (N.walkThruWalls)
            {
                temp = N.rb.position + temp * (N.Speed/1.5f) * 0.01f;
                N.transform.position = temp;
            }
            else
            {
                temp = N.rb.position + temp * N.Speed * 0.01f;
                N.rb.MovePosition(temp);
            }

            if(N.OscilatingSound != null)N.OscAction();

            if (N.An != null)
            {
                N.Hgreater = false;
                N.animX = 0;
                N.animY = 0;

                N.HandleAnimVals();

                //Animation stuff
                N.An.SetInteger("Horizontal", N.Hgreater ? (N.Hor > 0 ? 1 : -1) : 0);
                N.An.SetInteger("Vertical", N.Hgreater ? 0 : (N.Ver > 0 ? 1 : -1));
                N.An.SetFloat("Speed", N.Speed * N.AnimSpeed);
            }

            doEndCheck(i, N);

            N.rb.velocity = Vector2.zero;
        }

        if (APos.Count == 0)
        {
            if(!n)Dad.goodtoGo = true;
            this.enabled = false;
        }
    }

    void doEndCheck(int i, NPCMovement N)
    {
        if (Vector2.Distance(N.rb.position, APos[i].position) >= 0.2f) return;

        if(N.An != null)
        {
            N.An.SetFloat("Speed", 0);
            N.An.SetInteger("Horizontal", 0);
            N.An.SetInteger("Vertical", 0);
        }

        N.OscUp = !N.OscUp;
        APos.RemoveAt(i);
        Actors.RemoveAt(i);
    }
}

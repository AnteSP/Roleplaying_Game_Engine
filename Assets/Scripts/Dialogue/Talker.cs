using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talker : Resource
{
    [SerializeField] Dialogue D;
    [SerializeField] string[] sentences;

    NPCMovement N;
    bool NE = false;

    int Index = 0;

    [SerializeField] AudioSource TypeNoise;

    [SerializeField] bool talkOnAwake = false;
    public GameObject specialObj = null;

    public void overWriteSentences(string[] inp)
    {
        sentences = inp;
    }

    public void resetIndex()
    {
        Index = 0;
    }

    private void Start()
    {
        if(TryGetComponent<NPCMovement>(out NPCMovement a))
        {
            N = a;
            NE = true;
        }
        if (talkOnAwake) Use(0);
    }

    public override void Use(float Amount)
    {
        if (Index == 0) Stats.setLockedInObject(this);
        GameObject P = Stats.current.Player;
        P.GetComponent<Movement>().ShutDown();
        Camera.main.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        D.gameObject.SetActive(true);
        D.transform.localPosition = new Vector3(D.transform.localPosition.x, (Camera.main.transform.position.y > P.transform.position.y) ? Mathf.Abs(D.transform.localPosition.y) : -Mathf.Abs(D.transform.localPosition.y), D.transform.localPosition.z);
        D.TypeNoise = TypeNoise;


        D.Current = sentences[Index++ % sentences.Length];
        if (NE)
        {
            N.enabled = false;
            N.Face(P.GetComponent<Rigidbody2D>().position);
            N.ShutDown();
            D.NPC = N;
        }
        D.talker = this;

        D.NextSentence();

/*        try
        {
            D.Current = sentences[Index++%sentences.Length];
            if (NE)
            {
                N.enabled = false;
                N.Face(P.GetComponent<Rigidbody2D>().position);
                N.ShutDown();
                D.NPC = N;
            }
            D.talker = this;

            D.NextSentence();

        } catch(System.Exception e)
        {
            print("TALKER ERROR: " + e);
            D.EndConvo();

            Index = 0;
        }
*/
        

        

    }
}

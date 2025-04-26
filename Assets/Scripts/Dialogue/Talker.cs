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

    public void changeSentence(int ind,string changeTo)
    {
        if(ind < 0)//prepend sentences
        {
            string[] updatedSentences = new string[sentences.Length + 1];
            updatedSentences[0] = changeTo;
            sentences.CopyTo(updatedSentences, 1);
            sentences = updatedSentences;
        }
        else
            sentences[ind] = changeTo;
    }

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

        if (FUNMax != FUNMin && !Progress.checkFUN(FUNMin, FUNMax)) Destroy(gameObject);
    }

    public override void Use(float Amount)
    {
        if (Index == 0)
        {
            Stats.setLockedInObject(this);
            Stats.StartStopTime(false, "Talker");
        }
        GameObject P = Stats.current.Player;
        Stats.StartStopPlayerMovement(false,"Talker");
        Camera.main.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        D.gameObject.SetActive(true);
        D.DisplayOnTop((Camera.main.transform.position.y > P.transform.position.y));
        D.SetTypeNoise(TypeNoise);


        D.Current = sentences[Index++ % sentences.Length];

        if (NE)
        {
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

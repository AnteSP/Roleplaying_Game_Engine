using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talker1 : Resource
{
    [SerializeField] Dialogue D;
    [SerializeField] string[] sentences;

    [SerializeField] string[] Altsentences;

    NPCMovement N;
    bool NE = false;

    int Index = 0;

    [SerializeField] AudioSource TypeNoise;

    private void Start()
    {
        if(TryGetComponent<NPCMovement>(out NPCMovement a))
        {
            N = a;
            NE = true;
        }
    }

    public override void Use(float Amount)
    {



        GameObject P = Stats.current.Player;
        P.GetComponent<Movement>().ShutDown();
        Camera.main.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        D.gameObject.SetActive(true);
        D.DisplayOnTop();
        D.SetTypeNoise(TypeNoise);
        try
        {
            D.Current = sentences[Index++];

            if (NE)
            {
                N.enabled = false;
                N.Face(P.GetComponent<Rigidbody2D>().position);
                N.ShutDown();
                D.NPC = N;
            }

            D.NextSentence();

        } catch(System.Exception e)
        {
            D.EndConvo();

            Index = (!(sentences[sentences.Length - 1][1] == 'K' && sentences[sentences.Length - 1][0] == '%')) ? 0 : Index;
        }

        

        

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talker2 : Resource
{
    [SerializeField] Dialogue D;
    [SerializeField] string[] sentences;

    [SerializeField] string[] Altsentences1;
    [SerializeField] string[] Altsentences2;
    string[] backup;

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
        backup = sentences;
    }

    public override void Use(float Amount)
    {
        //3,7,11,15
        if(Index == 0)
        {
            if (Items.Add(15,-1))
            {
                Items.Add(7, -1);
                Items.Add(11, -1);
                Items.Add(3, -1);
                sentences = Altsentences2;
            } else if(Items.Add(7,-1) || Items.Add(12, -1)|| Items.Add(3, -1))
            {
                sentences = Altsentences1;
            }
            else
            {
                sentences = backup;
            }
        }

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

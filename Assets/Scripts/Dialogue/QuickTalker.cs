using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTalker : Resource
{
    [SerializeField] Dialogue D;
    [SerializeField] string sentence;

    NPCMovement N;
    bool NE = false;

    [SerializeField] AudioSource TypeNoise;
    Collider2D col;
    public int messageTime = 5;
    [SerializeField] bool stopPlayer = false;
    [SerializeField] int stopPlayerTime = 0;

    static public bool ONLYACTIVETALKER = false;
    static QuickTalker currentlyWaiting = null;
    public enum DialogueBoxPosition
    {
        AlwaysTop, Dynamic, AlwaysBottom
    }

    [SerializeField] DialogueBoxPosition dPosition = DialogueBoxPosition.Dynamic;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        if(TryGetComponent<NPCMovement>(out NPCMovement a))
        {
            N = a;
            NE = true;
        }
    }

    public override void Use(float Amount)
    {
        D.gameObject.SetActive(true);

        if (stopPlayer) Stats.StartStopPlayerMovement(false);

        if (dPosition == DialogueBoxPosition.Dynamic) D.DisplayOnTop();
        else D.DisplayOnTop(dPosition == DialogueBoxPosition.AlwaysTop);
        D.SetTypeNoise(TypeNoise);
        D.Current = sentence;

        if (NE)
        {
            N.enabled = false;
            N.Face(Stats.current.Player.GetComponent<Rigidbody2D>().position);
            N.ShutDown();
            D.NPC = N;
        }

        if (currentlyWaiting != null)
        {
            print("STOPPING FOR " + currentlyWaiting.gameObject.name);
            currentlyWaiting.StopAllCoroutines();
            Destroy(currentlyWaiting.gameObject);
        }
        currentlyWaiting = this;
        D.NextSentence();
        ONLYACTIVETALKER = true;

        try
        {

        } catch(System.Exception e)
        {
            D.EndConvo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player") return;
        col.enabled = false;
        
        Use(0);
        StartCoroutine(ExampleCoroutine());
        if(stopPlayerTime > 0)
        {
            StartCoroutine(PlayerMovementCoroutine());
            
        }
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(messageTime);

        if(ONLYACTIVETALKER) D.EndConvo();
        currentlyWaiting = null;

        Destroy(gameObject);
        
    }

    IEnumerator PlayerMovementCoroutine()
    {
        yield return new WaitForSeconds(stopPlayerTime);

        Stats.StartStopPlayerMovement(true);

    }
}

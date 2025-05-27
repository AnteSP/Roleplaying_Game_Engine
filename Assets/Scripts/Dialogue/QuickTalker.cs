using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTalker : Resource
{
    [SerializeField] Dialogue D;
    public string sentence;

    NPCMovement N;
    bool NE = false;

    [SerializeField] AudioSource TypeNoise;
    public int messageTime = 5;
    [SerializeField] bool stopPlayer = false;
    [SerializeField] bool justDisableComponent = false;
    [SerializeField] int stopPlayerTime = 0;
    public GameObject specialObject;

    static public bool ONLYACTIVETALKER = false;
    static QuickTalker currentlyWaiting = null;
    public enum DialogueBoxPosition
    {
        AlwaysTop, Dynamic, AlwaysBottom
    }

    [SerializeField] DialogueBoxPosition dPosition = DialogueBoxPosition.Dynamic;

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
        D.gameObject.SetActive(true);

        if (stopPlayer)
        {
            Stats.StartStopPlayerMovement(false, "QuickTalker");
            Stats.StartStopTime(false, "QuickTalker");
        }

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
        D.quickTalker = this;

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
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        
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

        if (justDisableComponent) this.enabled = false;
        else Destroy(gameObject);



    }

    IEnumerator PlayerMovementCoroutine()
    {
        yield return new WaitForSeconds(stopPlayerTime);

        Stats.StartStopPlayerMovement(true,"QuickTalker");
        Stats.StartStopTime(true, "QuickTalker");
    }
}

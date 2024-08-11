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
        D.transform.localPosition = new Vector3(D.transform.localPosition.x, (Camera.main.transform.position.y > Stats.current.Player.transform.position.y) ? Mathf.Abs(D.transform.localPosition.y) : -Mathf.Abs(D.transform.localPosition.y), D.transform.localPosition.z);
        D.TypeNoise = TypeNoise;
        D.Current = sentence;

        if (NE)
        {
            N.enabled = false;
            N.Face(Stats.current.Player.GetComponent<Rigidbody2D>().position);
            N.ShutDown();
            D.NPC = N;
        }

        D.NextSentence();

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


    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(messageTime);
        D.EndConvo();
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ch2Events : MonoBehaviour
{

    [SerializeField] AudioSource metalBang;
    [SerializeField] AudioSource jump;
    [SerializeField] AudioSource scratch;
    Animation an;
    public string inp;
    [SerializeField] Animator playerThrown;
    [SerializeField] List<SpriteRenderer> boysToTurn;
    [SerializeField] NPCMovement PlayerActor;
    public bool TurnTimeOff = false;
    [SerializeField] Sprite BeerSprite;

    // Start is called before the first frame update
    void Start()
    {
        if (TurnTimeOff)
        {
            Stats.StartStopTime(false, "Ch2 Opening");
        }
        an = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && gameObject.name == "Truck Parent")
        {
            //GetComponent<Animation>().Play(inp);
        }
    }

    public void MetalBang()
    {
        metalBang.Play();
    }

    public void Jump()
    {
        jump.Play();
    }

    public void Scratch()
    {
        scratch.Play();
    }

    public void PGetSM()
    {
        playerThrown.enabled = false;
        Items.Add(17, 1);
    }

    public void TurnBoys()
    {
        foreach (SpriteRenderer s in boysToTurn) s.flipX = true;
    }

    public void deleteSelf()
    {
        Destroy(gameObject);
    }

    public void PGetBeer()
    {
        Items.ShiftAnim(BeerSprite, "BEER!", "Finally no more withdrawal symptoms");
        //Items.Add(25, 1);
        PlayerActor.Face("down");
    }
}

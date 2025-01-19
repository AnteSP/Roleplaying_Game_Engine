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
    [SerializeField] Transform playerTPSpot;
    [SerializeField] GameObject playerSleeping;
    [SerializeField] StoreMenu openOnStart;
    [SerializeField] List<Talker> prependToSent;
    [SerializeField] QuickTalker FredSecret;

    // Start is called before the first frame update
    void Start()
    {
        if (TurnTimeOff)
        {
            Stats.StartStopTime(false, "Ch2 Opening");
        }
        an = GetComponent<Animation>();

        if (prependToSent.Count > 0 && Items.Contains(31))
        {
            foreach(Talker t in prependToSent)
            {
                t.changeSentence(-1, "%{31,1}  ");
                t.changeSentence(-1, "%#CHey... Soda Lad... Wait, you 'ave that record my nan wanted? Ah thanks mate, appreciate that");
                t.changeSentence(2, "%[FredF]Anyway...");
            }
            
        }
        
    }

    public int calcFredCost()
    {
        switch (Progress.getInt("FredF"))
        {
            case 0: return 20000;
            case 1: return 10000;//max day 1   dec1
            case 2: return 5000;//max day 2    dec1 + dec2
            case 3: return 2000;
            case 4: return 1000;//max day 3    dec1 + dec2 + task + dec3
            case 5: return 500;//max day 4    dec1 + dec2 + task + dec3 + dec4
            case 6: return 0;
            default: return 0;
        }
    }

    private void OnEnable()
    {
        if (openOnStart != null)
        {
            openOnStart.Use(0);
            gameObject.SetActive(false);
        }

        if (FredSecret != null)
        {
            if (calcFredCost() > Stats.current.Money) FredSecret.sentence = "%[GameOver]Fred (disguised): I'm sorry lad, you don't have enough cash. Not worth the risk";
            else FredSecret.sentence = "%[Ch2Final]Fred (disguised): Oi! Sh! Don't look, it's me. I see you got the money... Restroom door... Climb out the window... Boot o' my car";
        }
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

    public void takePlayerToTPSpot()
    {
        Stats.current.Player.GetComponent<SpriteRenderer>().enabled = false;
        
        Stats.current.Player.transform.position = playerTPSpot.transform.position;
        Stats.current.Player.GetComponent<Movement>().handleCameraStuff(0, 0, playerTPSpot.transform.position, true);
        Stats.StartStopPlayerMovement(false,"Ch2WakeUp");
        Stats.SkipToNextTime(9,0);
    }

    public void bringBackPlayerAfterWaking()
    {
        Stats.current.Player.GetComponent<SpriteRenderer>().enabled = true;
        Stats.StartStopPlayerMovement(true, "Ch2WakeUp");
        playerSleeping.SetActive(false);
        switch (Stats.getCurrentDay())
        {
            case 2:
                Stats.StartDeadline('B');
                break;
            case 3:
                Stats.StartDeadline('C');
                break;
            case 4:
                Stats.StartDeadline('D');
                break;
            case 5:
                Stats.StartDeadline('E');
                break;

        }
        Progress.switchInPlay("Ch2WakeUp", false);
    }
}

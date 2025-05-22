using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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

    [SerializeField] List<GameObject> GGOptions,DannyOptions;

    [SerializeField] AudioSource drugTakeNoise;

    public static Ch2Events current;
    [SerializeField] PlayableDirector drug3;
    [SerializeField] List<GameObject> EnableOnDrug3, DisableOnDrug3;
    [SerializeField] SellSpot HQSellSpot;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
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

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Ch2-HQ")
        {
            bool DannyEventPossible = Stats.allTimeInGame > Stats.dayHourToTime(5, 18) && Stats.allTimeInGame < Stats.dayHourToTime(5, 22);
            //print("ALLTIME: " + )

            foreach (GameObject g in DannyOptions) if (!DannyEventPossible) Destroy(g);
            foreach (GameObject g in GGOptions) if (DannyEventPossible) Destroy(g);
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

        if(HQSellSpot != null)
        {
            if (Progress.getInt("MRespect") >= 0)
            {
                HQSellSpot.CostToPrep = 0;
                if (!Progress.getBool("Ch2HQSpotFree"))
                {
                    Stats.DisplayMessage("Your friendship with the mafia is no longer negative! That means they're no longer actively resentful of you hooray!\n\nThe sell spot by the mafia HQ is now FREE! Go set up there");
                    Progress.switchInPlay("Ch2HQSpotFree", true);
                }
            }


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
                Progress.saveData(toFile: "saveArchiveCh2LastDay");
                break;

        }
        Progress.switchInPlay("Ch2WakeUp", false);
    }

    public void startDrug1()
    {
        StartCoroutine(drug1());
    }

    public IEnumerator drug1()
    {
        UnityEngine.Rendering.Universal.FilmGrain fg;
        UnityEngine.Rendering.Universal.ChromaticAberration ca;
        Camera.main.GetComponent<UnityEngine.Rendering.Volume>().profile.TryGet(out fg);
        Camera.main.GetComponent<UnityEngine.Rendering.Volume>().profile.TryGet(out ca);
        Movement m = Stats.current.Player.GetComponent<Movement>();
        drugTakeNoise.Play();
        Stats.current.GetComponent<AudioSource>().pitch = 2;

        float perc;
        for (int i = 0; i < 20; i++)
        {
            perc = (float)i / 20f;
            fg.intensity.Override( Mathf.Lerp(0.2f, 1, perc) );
            ca.intensity.Override(Mathf.Lerp(0.14f, 1, perc) );
            m.changeSpeed(0.15f + (0.15f*perc*2));
            yield return new WaitForSeconds(1f/20f);
        }
        
    }

    public void startDrug2()
    {
        StartCoroutine(drug2());
    }

    public IEnumerator drug2()
    {
        UnityEngine.Rendering.Universal.FilmGrain fg;
        UnityEngine.Rendering.Universal.ChromaticAberration ca;
        UnityEngine.Rendering.Universal.PaniniProjection pp;
        UnityEngine.Rendering.Universal.Vignette v;
        UnityEngine.Rendering.Volume camvol = Camera.main.GetComponent<UnityEngine.Rendering.Volume>();
        camvol.profile.TryGet(out fg);
        camvol.profile.TryGet(out ca);
        camvol.profile.TryGet(out pp);
        camvol.profile.TryGet(out v);
        Movement m = Stats.current.Player.GetComponent<Movement>();
        drugTakeNoise.Play();
        v.active = true;
        v.color.Override(Color.cyan);
        v.intensity.Override(0.2f);
        Stats.current.GetComponent<AudioSource>().pitch = -3;
        Camera.main.orthographic = false;

        float perc;
        for (int i = 0; i < 20; i++)
        {
            perc = (float)i / 20f;
            fg.intensity.Override(Mathf.Lerp(0.2f, 1, perc));
            ca.intensity.Override(Mathf.Lerp(0.14f, 1, perc));
            pp.cropToFit.Override(Mathf.Lerp(1, 0, perc));
            m.changeSpeed(0.15f + (0.15f * perc * 5));
            yield return new WaitForSeconds(1f / 20f);
        }

    }

    public void startDrug3()
    {
        drug3.Play();
        Stats.current.AllowSelecting = false;
        Stats.changeBackgroundMusic(null,true);
        Stats.StartStopTime(false,"DOIN DRUGS");
    }

    public void startDrug3_Part2()
    {
        UnityEngine.Rendering.Universal.FilmGrain fg;
        UnityEngine.Rendering.Universal.ChromaticAberration ca;
        UnityEngine.Rendering.Universal.PaniniProjection pp;
        UnityEngine.Rendering.Universal.Vignette v;
        UnityEngine.Rendering.Volume camvol = Camera.main.GetComponent<UnityEngine.Rendering.Volume>();
        camvol.profile.TryGet(out fg);
        camvol.profile.TryGet(out ca);
        camvol.profile.TryGet(out pp);
        camvol.profile.TryGet(out v);

        drugTakeNoise.Play();
        v.active = true;
        v.color.Override(Color.cyan);
        v.intensity.Override(0.6f);
        Camera.main.orthographic = false;

        fg.intensity.Override(1);
        ca.intensity.Override(1);
        pp.cropToFit.Override(0);

        UnityEngine.Rendering.Universal.SplitToning st = (UnityEngine.Rendering.Universal.SplitToning)camvol.profile.Add(typeof(UnityEngine.Rendering.Universal.SplitToning));
        st.shadows.Override(Color.red);
        st.highlights.Override(Color.blue);

        UnityEngine.Rendering.Universal.LensDistortion ld = (UnityEngine.Rendering.Universal.LensDistortion)camvol.profile.Add(typeof(UnityEngine.Rendering.Universal.LensDistortion));
        ld.intensity.Override(-0.5f);
        ld.scale.Override(0.4f);

        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "Arena") StartCoroutine(rotateArena(obj.transform,1));
            if (obj.name == "Buildings") StartCoroutine(rotateArena(obj.transform,-2));
        }

        
    }

    public IEnumerator rotateArena(Transform arena,float increm)
    {
        for (int i = 0; i < 900; i++)
        {
            arena.Rotate(new Vector3(0, increm, 0));
            yield return new WaitForSeconds(1f/60f);
        }

    }

    public void startDrug3_Part3()
    {
        UnityEngine.Rendering.Volume camvol = Camera.main.GetComponent<UnityEngine.Rendering.Volume>();
        camvol.enabled = false;
        Camera.main.orthographic = true;
        Camera.main.GetComponent<CamZoom>().SetSize(4);
        Camera.main.backgroundColor = new Color(0.05f, 0.05f, 0.05f);
        Stats.current.Player.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f,0.5f);
        Stats.current.AllowSelecting = true;
        Stats.current.Player.GetComponent<Movement>().changeSpeed(0.02f);
        StartCoroutine(FinishDrugSequence());
    }

    public IEnumerator FinishDrugSequence()
    {
        yield return new WaitForSeconds(15);
        Stats.DisplayMessage("Your connection to this world loosens... You will soon leave. But you feel as though a piece of this will remain with you");
        yield return new WaitForSeconds(10);
        Progress.saveOnlyThis(38, 1, "Ch2HatmanVisited", 0);
        Application.Quit();
        print("APP QUITTED");
    }
}

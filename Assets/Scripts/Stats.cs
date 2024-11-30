using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    [SerializeField] Text MONEYTEXT;
    TextMeshProUGUI MoneyAdd;
    [SerializeField] Text TIMETEXT;
    TextMeshProUGUI TimeAdd;
    [SerializeField] Text DAYTEXT;

    public int Money;
    //public uint TimeFromMidNight = 0;
    
    uint Time = 480;
    uint Day = 1;

    int Digits;

    float Timer;

    [SerializeField] Animator BLACKBAR;
    [SerializeField] GameObject DEADLINE;
    [SerializeField] GameObject LOADING;

    public CutSceneTalker CurrentCS;
    [SerializeField] CutSceneTalker DefaultCS;

    [SerializeField] GameObject MESSAGE;
    [SerializeField] GameObject GOMESSAGE;
    [SerializeField] AudioSource ThereGoesMyHero;

    public bool CanGoUp;
    //public bool ChoosingSells;

    public bool AllowSelecting = true;
    public bool PassTime = true;

    [SerializeField] public AudioSource KACHING;

    public float timePassage = 1;

    [SerializeField] Text DEBUGT;

    static readonly string[] tips = { "I did nazi those Nazis coming",
    " |  ||\n||  |_",
    "Maybe the mafia could help...",
    "who is skeptic?",
    "Make money\nBuy supplies\nMake more money\nrepeat",
    "Fuck Conker Koola bruh",
    "",
    "This is sodapressing",
    "Loading...",
    "loading...",
    "Rendering...",
    "CUT! great job",
    "when does it end",
    "End of cutscene",
    "Black bar transition",
    "...",
    ":D",
    "Is it pop or soda?",
    "Is there something\n you should be\n doing right now?"};

    static readonly string[] startCSTips = { "Starting cutscene...",
    "Epic cutscene starting...",
    "here we go...",
    "Loading cutscene...",
    "Oh shit is this a cutscene!?"};

    static readonly string[] endCSTips = { "Ending cutscene...",
    "Epic cutscene ending...",
    "Back to regular gameplay...",
    "Unpacking cutscene...",
    "Leaving the cutscene..."};

    static readonly string[] transCSTips = { "Some time later...",
    "Hours later...",
    "A while later..."};

    static readonly string[] nextDayCSTips = { "The next morning...",
    "You leave for bed\nand wake up the next day...",
    "You sleep\nand wake up in the morning..."};


    [SerializeField] GameObject Sounds;

    public GameObject Player;
    Movement playerMovement;

    public static Stats current;

    int NazisDed = 0;
    [SerializeField] GameObject NaziNotice;
    [SerializeField] CutSceneTalker EndFail, EndSucceed;
    [SerializeField] public Image Filter;

    [SerializeField] GameObject SignRemind;

    [SerializeField] bool StartWithoutInv = false;

    Animator space = null;

    public Resource lockedInConvoWith = null;

    [SerializeField] AudioSource specialSound = null;

    public static float allTime = 0;
    public static int allTimeInGame = 600;//Starts at 10:00am on ch2
    public static bool freePlay = false;
    public List<SellSodas> sodasSelling = new List<SellSodas>();

    [SerializeField] GameObject[] disableOnFreePlay;
    [SerializeField] GameObject[] enableOnlyOnFreePlay;

    public bool doDayLight = false;
    static readonly float noon = 0, midnight = 0.17f;
    UnityEngine.Rendering.Universal.Light2D[] streetLights;
    [SerializeField] GameObject streetLightsParent;
    bool isday = true;

    UnityEngine.Rendering.Volume camVol;
    float suncoefficient = 0;

    [SerializeField] AudioSource timeSkipSound;
    Vector3 OGTimeTextScale;
    Vector3 TargTimeTextScale;

    static string teleportPoint = "";
    [SerializeField] Transform TeleportPointsParent;

    [HideInInspector] public List<Image> stickyNotes = new List<Image>();
    List<Deadline> Deds = new List<Deadline>();
    [SerializeField] GameObject DeadlineNotification;
    [SerializeField] GameObject SocialNotification;

    private void OnEnable()
    {
        suncoefficient = Mathf.Asin(1) * 4;
        camVol = Camera.main.GetComponent<UnityEngine.Rendering.Volume>();
        if (freePlay)
        {
            foreach (GameObject g in disableOnFreePlay)
            {
                g.SetActive(false);
            }

            camVol.weight = 0;

            GameObject.FindGameObjectWithTag("SPECIAL").GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 0.3f;
        }
        foreach (GameObject g in enableOnlyOnFreePlay)
        {
            g.SetActive(freePlay);
        }

        QualitySettings.vSyncCount = 1;
        current = this;
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null) playerMovement = Player.GetComponent<Movement>();
        if( teleportPoint != "" && TeleportPointsParent != null)
        {
            Vector2 targ = TeleportPointsParent.GetComponentsInChildren<Transform>().Where(a => a.name == teleportPoint).First().position;
            Player.GetComponentInParent<Transform>().position = new Vector3(targ.x, targ.y, 0);
        }
        Digits = MONEYTEXT.text.Length;
        CurrentCS = DefaultCS;

        Transform overlay = DAYTEXT.transform.parent;
        overlay.GetComponent<MakeTrans>().forceStart();
        for(int i = 0; i < 99; i++)
        {
            Transform t = overlay.Find("Deadline (" + i + ")");
            if (t == null) break;
            stickyNotes.Add(t.GetComponent<Image>());
            t.gameObject.SetActive(false);
        }
        Deds = GetComponents<Deadline>().ToList();

        //CreateDeadline("Pay 1000 for the soda machine", 7*24*60, -1000, EndFail,EndSucceed);
        //DEADLINE.SetActive(false);

        if (StartWithoutInv)
        {
            Transform invO = GameObject.FindGameObjectWithTag("Inventory").transform;
            invO.position = new Vector3(invO.position.x - (250 * invO.lossyScale.x), invO.position.y, invO.position.z);
        }
        //print("load please");
        if (!Progress.wasDataLoaded()) Progress.loadData(excludeItems:true);
        //print("did it tho?");

        AudioSource aS = Stats.current.GetComponent<AudioSource>();
        if (aS != null)
        {
            StartCoroutine(StartSong(aS, 0.02f, 0.5f));
            OGVolume.Add("Stats", aS.volume);
        }

        if (ObjectDepth.Space != null) space = ObjectDepth.Space.GetComponent<Animator>();
        
        Transform card = Camera.main.transform.Find("Card");
        if (card != null) OGVolume.Add("Card", card.GetComponent<AudioSource>().volume);

        foreach (AudioSource i in Sounds.GetComponentsInChildren<AudioSource>())
        {
            OGVolume.Add(i.gameObject.name, i.volume);
        }

        SetVolume(Progress.getFloat("Volume"),false);

        OGTimeTextScale = current.TIMETEXT.transform.localScale;
        TargTimeTextScale = OGTimeTextScale * 1.4f;

        if (streetLightsParent != null)
        {
            streetLights = streetLightsParent.GetComponentsInChildren<UnityEngine.Rendering.Universal.Light2D>();

            float dayperc = ((float)allTimeInGame % (24f * 60f)) / (24f * 60f);
            dayperc = (Mathf.Cos(dayperc * current.suncoefficient - 0.5f) / 2) + 0.5f;

            isday = dayperc < 0.6f;

            foreach (UnityEngine.Rendering.Universal.Light2D l in current.streetLights) l.enabled = !current.isday;
        }

        //ChangeTime(TimeFromMidNight);

        MoneyAdd = MONEYTEXT.GetComponentInChildren<TextMeshProUGUI>();
        TimeAdd = TIMETEXT.GetComponentInChildren<TextMeshProUGUI>();
        MoneyAdd.gameObject.SetActive(false);
        TimeAdd.gameObject.SetActive(false);

        ChangeTime(0);
        ChangeMoney(0);

        if (!current.PassTime) StartStopTime(false, "Scene itself");

        foreach (Deadline.DeadlineData dd in Deadline.activeDeadlineData.Where(a=> a != null))
        {
            bool foundOne = false;
            foreach(Deadline ded in Deds.Where(a=> a.isSameEventAs(dd)))
            {
                ded.enabled = true;
                foundOne = true;
            }

            if (!foundOne )
            {
                if (current.PassTime)
                {
                    throw new Exception("HEY DUMBASS! There's no corresponding deadline script on the stats object for [" + dd.Description + "]");
                }
                else
                {
                    Deadline newDed = gameObject.AddComponent<Deadline>();
                    //at this point, newDed is (enabled = false) because of the OnEnable() Deadline function
                    newDed = dd.cloneVariaiblesTo(newDed);
                    newDed.enabled = true;
                    Deds.Add(newDed);
                }
                
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    public static float secSin = 1;
    public static uint delayedTimeAnim = 0;
    void Update()
    {
        if (AllowSelecting && !processingSelectable)
            StartCoroutine(UpdateSelectableIcon());

        if(ObjectDepth.Space != null && space == null) space = ObjectDepth.Space.GetComponent<Animator>();

        if (Input.GetKeyDown(KeyCode.Space) && !processingSelectable && space != null) 
            space.SetBool("Pressed", true);
        else if (Input.GetKeyUp(KeyCode.Space) && space != null)
            space.SetBool("Pressed", false);

        if (PassTime)
        {
            Timer += UnityEngine.Time.deltaTime;
            bool temp = Timer > 1f/(float)timePassage;
            //ChangeTime(temp ? (uint)timePassage : 0);
            ChangeTime(temp ? 1u : 0u);
            Timer = temp ? 0 : Timer;
        }
        allTime += UnityEngine.Time.deltaTime;

        secSin = Mathf.Sin(allTime*2);

        if (Input.GetKeyDown(KeyCode.X))
        {
            CloseMessage();
        }

        Debug("TIME: " + Time);
    }

    public uint getTime() => this.Time;

    public void SetupShop()
    {
        if (ChangeMoney(-SellSpot.current.CostToPrep))
        {
            KACHING.Play();
            //ChooseSellSoda.example
            //SellSpot.current.enabled = false;
            //SellSpot.current.transform.parent
            SellSpot.current.turnIntoMarket();
            
            //SellSpot.current.gameObject.AddComponent<ChooseSellSoda>(ChooseSellSoda.example);
        }
        else
        {
            Stats.DisplayMessage("You don't have enough money to setup shop here");
        }
        Stats.StartStopTime(true,"SellSpot");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="source">Set this if you only want time to start again if it gets started again by your source string</param>
    public static void StartStopTime(bool start,string source="")
    {
        //print("START " + start + " source: " + source);
        int comp = 0;
        if(source != "")
        {
            if (!start && !timeSources.Contains(source))//if stopping
            {
                timeSources.Add(source);
                comp = 1;
            }
            else//if starting
            {
                if(timeSources.Contains(source))
                    timeSources.Remove(source);
            }
        }

        if(timeSources.Count == comp)
        {
            current.PassTime = start;
            //current.TIMETEXT.color = start ? Color.black : Color.gray;
            UpdateTimeColor();
            NameIndic.Indicate("");
        }
        else
        {
            if (start) print("Time START rejected due to " + timeSources[0] + " and " + (timeSources.Count-1) + " others");
            else print("Time STOP rejected due to " + timeSources[0] + " and " + (timeSources.Count - 1) + " others");
        }

    }
    static List<string> timeSources = new List<string>();

    public static void UpdateTimeColor()
    {
        current.TIMETEXT.color = current.PassTime ? Color.black : Color.gray;
    }

    public static void EnableInventory()
    {
        if (!Stats.current.StartWithoutInv) return;
        Stats.current.StartWithoutInv = false;
        Transform inv = GameObject.FindGameObjectWithTag("Inventory").transform;
        inv.position = new Vector3(inv.position.x + (250* inv.lossyScale.x), inv.position.y, inv.position.z);
        print("ENABLING INVENTORY");
    }

    public static void KillNazi()
    {
        current.NazisDed++;
        print(current.NazisDed + "dead");
        if(current.NazisDed == 5)
        {
            current.NaziNotice.SetActive(false);
            DisplayMessage("ALL THE NAZI'S ARE DEAD! You can finally use the lemonade stand in front of the shop. Make sure you make $1000 by the time your sister returns");
            current.SignRemind.SetActive(true);
        }
    }

    public static void Debug(string h)
    {
        current.DEBUGT.text = h;
    }

    public static void PlaySpecialSound()
    {
        Stats.current.specialSound.Play();
    }

    public static void doSelecting(bool b)
    {
        current.AllowSelecting = b;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">0 = Start CS, 1 = End CS, 2 = Transition</param>
    public static void Transition(int type = 0)
    {
        if (current.CurrentCS == null) return;
        Dialogue.d.showDisplay(false);
        current.AllowSelecting = false;

        Stats.StartStopPlayerMovement(false,"Transition");

        if (!current.CurrentCS.gameObject.activeInHierarchy)
        {
            //current.PassTime = false;
        }
        current.BLACKBAR.Play("BlackBarsDown");

        string text = "";
        switch (type)
        {
            case 0:
                text = startCSTips[UnityEngine.Random.Range(0, startCSTips.Length)];
                break;
            case 1:
                text = endCSTips[UnityEngine.Random.Range(0, endCSTips.Length)];
                break;
            case 2:
                text = transCSTips[UnityEngine.Random.Range(0, transCSTips.Length)];
                break;
            case 3:
                text = nextDayCSTips[UnityEngine.Random.Range(0, nextDayCSTips.Length)];
                break;
            case 4:
                text = "";
                break;
        }

        current.BLACKBAR.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = text;
    }

    public static void AccelerateTime(float a)
    {
        //current.timePassage = ((++a) * a * a * a * a);//t = (x+1)^5
        current.timePassage = a*a*a;
    }

    bool processingSelectable = false;
    IEnumerator UpdateSelectableIcon()
    {
        bool UseResource = Input.GetKeyUp(KeyCode.Space) && ObjectDepth.Selected.name != "Player" && ObjectDepth.Space.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TimeGone");
        processingSelectable = true;
        SpriteRenderer spaceSpr = ObjectDepth.Space.GetComponent<SpriteRenderer>();
        Color ogc = spaceSpr.color;
        spaceSpr.color = new Color(1, 1, 1, 0.2f);

        if (UseResource || (lockedInConvoWith != null && Input.GetKeyUp(KeyCode.Space) ))
        {
            ObjectDepth.Space.GetComponent<Animator>().SetBool("Pressed", true);

            Animator temp = ObjectDepth.Space.GetComponent<Animator>();
            temp = ObjectDepth.Space.GetChild(0).GetComponent<Animator>();
            temp.SetTrigger("Go");
            temp.SetFloat("Speed", 1f / (ObjectDepth.Selected.GetComponent<Resource>().CollectTime + 0.01f));
            yield return new WaitForSeconds(ObjectDepth.Selected.GetComponent<Resource>().CollectTime * 1.3f);

            if(lockedInConvoWith != null)
            {
                lockedInConvoWith.Use(1);//use locked in object
            }
            else
            {
                switch (ObjectDepth.Selected.tag)
                {
                    case "Machine":
                        ObjectDepth.Selected.GetComponent<Resource>().Use(1);
                        break;
                    case "SodaMachine":
                        ObjectDepth.Selected.GetComponent<SodaMachine>().MakeActiveSM();
                        SodaMachine.ToggleMenu();
                        break;
                }
            }
        }

        ObjectDepth.Space.GetComponent<SpriteRenderer>().color = ogc;
        CanGoUp = Input.GetKeyUp(KeyCode.Space) ? true : CanGoUp;
        processingSelectable = false;
    }

    public static void releaseLockedInObject(bool removeFromNearby = true)
    {
        if (current.lockedInConvoWith == null) return;
        //print("Released locked in");
        GameObject lockedInObjectPointer = current.lockedInConvoWith.gameObject;
        current.lockedInConvoWith = null;
        if(removeFromNearby)
            ObjectDepth.forceRemoveFromNearby(lockedInObjectPointer);
        
    }

    public static void setLockedInObject(Resource r)
    {
        //print("Locked in " + r.gameObject.name);
        current.lockedInConvoWith = r;
    }
    /*
    void UpdateSelectableIcon()
    {
        bool UseResource = Input.GetKeyUp(KeyCode.Space) && ObjectDepth.Selected.name != "Player" && ObjectDepth.Space.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TimeGone");

        if (UseResource)
        {
            ObjectDepth.Space.GetComponent<Animator>().SetBool("Pressed", true);

            Animator temp = ObjectDepth.Space.GetComponent<Animator>();
            temp = ObjectDepth.Space.GetChild(0).GetComponent<Animator>();
            temp.SetTrigger("Go");
            temp.SetFloat("Speed", 1f / (ObjectDepth.Selected.GetComponent<Resource>().CollectTime + 0.01f));

            if (ObjectDepth.Selected.tag == "Machine")
            {
                ObjectDepth.Selected.GetComponent<Resource>().Use(1);
            }
            else if (ObjectDepth.Selected.tag == "SodaMachine")
            {
                ObjectDepth.Selected.GetComponent<SodaMachine>().ToggleMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ObjectDepth.Space.GetComponent<Animator>().SetBool("Pressed", true);
        }else if (Input.GetKeyUp(KeyCode.Space))
        {
            ObjectDepth.Space.GetComponent<Animator>().SetBool("Pressed", false);
        }

        CanGoUp = Input.GetKeyUp(KeyCode.Space) ? true : CanGoUp;
    }*/

    public static bool ChangeMoney(int Amount)
    {

        bool temp = current.Money + Amount < 0;

        if (!temp)
        {

            current.Money = temp ? 0 : current.Money + Amount;

            string Temp = "";
            int overflow = 9;

            for (int i = 1; i < current.Digits; i++)
            {
                if (current.Money >= Mathf.Pow(10, i))
                {
                }
                else
                {
                    Temp += " ";
                }
                overflow += 9 * Mathf.RoundToInt(Mathf.Pow(10, i));
            }

            Temp += current.Money;

            bool Overflown = Temp.Length > current.Digits;

            current.MONEYTEXT.text = Overflown ? overflow + "" : Temp;
            current.Money = Overflown ? overflow : current.Money;

            current.MONEYTEXT.text = current.Money + "";

            if (Amount != 0 && current.MoneyAdd != null)
            {
                current.MoneyAdd.gameObject.SetActive(false);
                if (Amount > 0)
                {
                    current.MoneyAdd.text = "+ " + Amount;
                    current.MoneyAdd.color = Color.green;
                }
                else
                {
                    current.MoneyAdd.text = "- " + -Amount;
                    current.MoneyAdd.color = Color.red;
                }
                current.MoneyAdd.gameObject.SetActive(true);
            }

        }

        return (!temp);
    }
    /*
    public static bool ChangeEnergy(float Amount)
    {

        bool temp = Energy + Amount < 0;

        Energy = temp ? 0 : Energy + Amount;

        Energy = Energy + Amount > EnergyLimit ? EnergyLimit : Energy;
    
        ProgressBar.DoLine(ENERGYBAR.rectTransform, Initial, 142, Energy / EnergyLimit, true);

        ENERGYBAR.transform.parent.Find("Text").GetComponent<Text>().text = Energy + "/" + EnergyLimit;
        
        return (!temp);
    }
    */

    public static string allTimeInGameToString(int t)
    {
        t = t % (24 * 60);//remove day information
        int mins = t % 60;
        t = t / 60;//get to the hour
        return (t < 10? "0" + t : t )  + ":" + (mins < 10 ? "0" + mins : mins);
    }
    static bool timeAnimating = false;

    public static void ChangeTimeAnim(int Amount, int frames = 40) => current.ChangeTimeAnimLocal(Amount, frames);
    public void ChangeTimeAnimLocal(int Amount, int frames = 40)
    {
        if(Amount < 0)
        {
            Stats.DisplayMessage("CANNOT GO BACK IN TIME");
            return;
        }

        if (Amount > 1)
        {
            current.TimeAdd.gameObject.SetActive(false);
            current.TimeAdd.text = "+ " + allTimeInGameToString((int)Amount);
            current.TimeAdd.gameObject.SetActive(true);
        }

        if (timeAnimating)
        {
            timeSkipSound.Play();
            ChangeTime((uint)Amount);
        }
        else
        {
            StartCoroutine(current.ChangeTimeAnim((uint)Amount, frames));
        }
       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Amount"></param>
    /// <param name="frames">at 50 fps (1 sec = 50 frames)</param>
    /// <returns></returns>
    IEnumerator ChangeTimeAnim(uint Amount,int frames)
    {
        //if (timeAnimating) frames = 0;//if we're already animating, don't do any animation
        timeAnimating = true;
        timeSkipSound.Play();
        
        if (frames >= 20)//enough time to do animations
        {
            int mpf = (int)Amount / frames;
            int leftover = (int)Amount % frames;
            float temp = 0;

            for(int i = 0; i < frames; i++)
            {

                if(i < 10)
                {
                    temp = (float)i / 10f;
                    //print(temp);
                    TIMETEXT.color = new Color(temp, temp, temp);
                    TIMETEXT.transform.localScale = Vector3.Lerp(OGTimeTextScale, TargTimeTextScale, temp);
                }
                else if(i > frames - 10)
                {
                    temp = (float)(frames - i-1)/10f;
                    //print(temp);
                    TIMETEXT.color = new Color(temp, temp, temp);
                    TIMETEXT.transform.localScale = Vector3.Lerp(OGTimeTextScale, TargTimeTextScale, temp);
                }

                ChangeTime((uint)(mpf + ((i < leftover) ? 1 : 0) ));
                yield return new WaitForSeconds(0.02f);
            }
        }
        else
        {
            ChangeTime(Amount);
        }
        timeAnimating = false;
        UpdateTimeColor();
    }

    public static void ChangeTime(uint Amount)
    {
        //print("Changed " + Amount);
        allTimeInGame += (int)Amount;
        current.Day = (uint)allTimeInGame/(24*60) + 1;

        DateTime resultDate = new DateTime(1979, 6, 1).AddDays(current.Day);
        current.DAYTEXT.text = resultDate.ToString("ddd MMM dd, yyyy");

        current.Time -= ( 24*60 * (uint)Mathf.FloorToInt((float)(current.Time + Amount) / (24f * 60f)));
        current.Time += Amount;

        //+ 605 since we begin at 10am in ch2
        current.TIMETEXT.text = allTimeInGameToString(allTimeInGame);

        CheckDeadlines();

        if (current.doDayLight)
        {
            float dayperc = ((float)allTimeInGame % (24f * 60f)) / (24f * 60f);
            //Debug(dayperc + " ");
            //dayperc += dayperc + 0.5f;
            //dayperc = dayperc % 1;
            dayperc = ( Mathf.Cos(dayperc * current.suncoefficient - 0.5f) /2) + 0.5f;
            
            current.camVol.weight = Mathf.Lerp(noon, midnight, dayperc);

            if ((dayperc < 0.6f) != (current.isday))//if isday does not match whether it's actually day or not
            {
                current.isday = !current.isday;
                foreach (UnityEngine.Rendering.Universal.Light2D l in current.streetLights) l.enabled = !current.isday;
            }

        }
    }

    public static void SkipToNextTime(uint hour,uint min)
    {
        uint rawTime = Stats.current.Time % (24 * 60);

        uint targTime = ((hour * 60) + min) - 120; //120 offset is needed for some reason? (it works tho. Not sure why. But it works)

        //print("RAW " + rawTime + " TARG " + targTime);

        ChangeTime((targTime - rawTime) + (uint)(targTime < rawTime ? (24 * 60) : 0));
    }
    public static uint getCurrentDay() => current.Day;
    /*
    public static void CreateAction(string Title,string Description,int Changemoney,float ChangeEnergy,uint ChangeTime)
    {
        GameObject Action = Instantiate(ACT, ACT.transform.parent);
        Action.transform.Find("Title").GetComponent<Text>().text = Title;
        Action.transform.Find("Description").GetComponent<Text>().text = Description;
        Action temp = Action.transform.GetComponentInChildren<Action>();
        temp.Money = Changemoney;
        temp.Energy = ChangeEnergy;
        temp.Time = ChangeTime;

    }
    */

    public static int GetStickyNote()
    {
        for(int i = 0; i < current.stickyNotes.Count; i++)
        {
            if (current.stickyNotes[i].gameObject.activeInHierarchy) continue;
            current.stickyNotes[i].gameObject.SetActive(true);
            return i;
        }
        return -1;
    }

    static void CheckDeadlines()
    {
        foreach(Deadline d in current.Deds.Where(a=> a.enabled))
        {
            d.Refresh();
        }
    }

    public static void StartDeadline(char ID)
    {
        Deadline d = current.Deds.Where(a => a.ID == ID).First();
        if(d == null)
        {
            throw new Exception("NO DEADLINE FOUND OF ID " + ID);
        }
        else
        {
            current.DeadlineNotification.gameObject.SetActive(true);
            d.enabled = true;
        }
    }

    static public void GameOver()
    {

        print("GAME OVER");
        current.GOMESSAGE.SetActive(true);
        current.GOMESSAGE.transform.Find("Text").GetComponent<Text>().text = "YOU FUCKING DIED\n\nconsider this a gameover... But feel free to still explore around";
        StartStopPlayerMovement(false);
        current.ThereGoesMyHero.Play();

    }

    static public void StartStopPlayerMovement(bool start, string source="")
    {
        if (current == null) return;

        if(current.Player == null)
        {
            GameObject g = GameObject.FindGameObjectWithTag("Player");
            if (g != null)
            {
                current.Player = g;
                current.playerMovement = g.GetComponent<Movement>();
            }
            else return;
        }

        //print("M-START " + start + " SOURCE " + source);
        int comp = 0;
        if(source != "")
        {
            if (!start && !moveSources.Contains(source))//stopping
            {
                moveSources.Add(source);
                comp = 1;
            }
            else//starting
            {
                if (moveSources.Contains(source))
                    moveSources.Remove(source);
            }
        }

        if(moveSources.Count == comp)//accept action
        {
            if (start)
                current.playerMovement.enabled = true;
            else
                current.playerMovement.ShutDown();
        }
        else
        {
            if (start) print("Movement START rejected due to " + moveSources[0] + " and " + (moveSources.Count - 1) + " others");
            else print("Movement STOP rejected due to " + moveSources[0] + " and " + (moveSources.Count - 1) + " others");
        }


    }
    static List<string> moveSources = new List<string>();

    /// <summary>
    /// Display a message instantly with the text box thing
    /// </summary>
    /// <param name="text"></param>
    static public void DisplayMessage(string text,bool stopTime = false)
    {
        if (stopTime)
        {
            Stats.StartStopTime(false,"Message");
        }
        current.MESSAGE.SetActive(true);
        current.MESSAGE.transform.Find("Text").GetComponent<Text>().text = text;
        StartStopPlayerMovement(false,"Message");
    }

    public void CloseMessage()
    {
        StartStopTime(true, "Message");
        StartStopPlayerMovement(true, "Message");
        NameIndic.Indicate("");
        current.MESSAGE.SetActive(false);
    }

    static public void PendMessage(string text)
    {
        ForceHandlesBack.PendingMessage = text;
    }

    public void MuteSound(bool t)
    {
        foreach(AudioSource i in Sounds.GetComponentsInChildren<AudioSource>())
        {
            i.mute = t;
        }
    }

    Dictionary<string, float> OGVolume = new Dictionary<string, float>();
    public void SetVolume(float a, bool changeData)
    {
        if (float.IsNaN(a)) return;
        if (changeData) Progress.setFloat("Volume", a);

        AudioSource StatsA = Stats.current.GetComponent<AudioSource>();
        if(StatsA != null) StatsA.volume = a * 0.5f;

        Transform card = Camera.main.transform.Find("Card");
        if(card != null) card.GetComponent<AudioSource>().volume = a* OGVolume["Card"];

        foreach (AudioSource i in Sounds.GetComponentsInChildren<AudioSource>())
        {
            i.volume = a * OGVolume[i.gameObject.name];
        }
    }

    public void SetVolume(float a)
    {
        SetVolume(a, true);
    }

    public static void changeBackgroundMusic(AudioClip a)
    {
        AudioSource aS = Stats.current.GetComponent<AudioSource>();
        aS.clip = a;
        aS.mute = false;
        aS.Play();
    }

    IEnumerator StartSong(AudioSource a,float time,float toVol)
    {
        float v = Progress.getFloat("Volume");
        if (!float.IsNaN(v)) toVol *= v;
        a.volume = 0;
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(time);
            a.volume += 0.01f*toVol;
        }
    }

    public void SceneChange(string r)
    {
        LOADING.SetActive(true);
        Player.SetActive(false);
        timeSources.Clear();
        SellUpgrade.FlushUpgrades();
        Progress.saveData();
        Slider.EmptyList();
        Stats.current = null;
        teleportPoint = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(r);
        
        Stats.current = GameObject.FindGameObjectWithTag("STATS").GetComponent<Stats>();

        print("UNLOADING DATA");
        Progress.markDataAsUnloaded();
    }

    public void FilterColor(Color c)
    {
        Animator a = Filter.GetComponent<Animator>();
        if (a != null) a.enabled = false;
        Filter.color = c;
    }

    public IEnumerator fadeFilterColor(Color C1,Color C2, float secs, Sprite img = null)
    {
        Filter.GetComponent<Animator>().enabled = false;
        Filter.sprite = img;
        Filter.color = C1;
        //print("GOT HERE");
        for (int i = 0; i < 100; i++)
        {
            //print("GOT HERE2 " + (secs/100f));
            yield return new WaitForSeconds(secs/100f);
            //print("GOT HERE3");
            //Filter.color = (C1*(100-i)) + (C2*(i));
            float I = (float)i / 100f;
            Filter.color = new Color( 
                (C1.r * (1 - I)) + (C2.r * I) , 
                (C1.g * (1 - I)) + (C2.g * I), 
                (C1.b * (1 - I)) + (C2.b * I), 
                (C1.a * (1 - I)) + (C2.a * I));
            //Stats.Debug("" + Filter.color.g + " -- " + ((C1.g * (1 - I)) + (C2.g * I)) + " -- " + I + " -- " + (1-I) + " -- " + (C1.g * (1 - I)) + " -- " + "");
            //print(Filter.color.r + "  --  " + Filter.color.b);
            //print(Filter.color);
        }
        Filter.color = C2;
        Filter.sprite = null;
        Filter.GetComponent<Animator>().enabled = true;
    }

    public void setCurrentCSGoodToGo(bool b)
    {
        if (CurrentCS == null) return;
        CurrentCS.setGoodToGoOnly(b);
    }

    //SocialNotification
    public static void changeFriendship(string Id,int amount)
    {
        print("GOT HERE BLEGH");
        current.SocialNotification.transform.parent.gameObject.SetActive(true);//The parent should be the social menu handle
        current.SocialNotification.gameObject.SetActive(false);

        if (Progress.doesFieldExist(Id))
        {
            Progress.setInt(Id, Progress.getInt(Id) + amount);
        }
        else
        {
            Progress.setInt(Id, amount);
        }

        current.SocialNotification.gameObject.SetActive(true);
        current.SocialNotification.GetComponent<TextMeshProUGUI>().text = "Friendship " + (amount < 0 ? "" : "+") + amount;

        
    }

}

public class Recipe
{

    public int[] Ingredients;
    public string Name;
    public int BasePrice;
    public Sprite Pic;
    public int ItemID;

    public Recipe(int[] ingredients, int itemID)
    {
        Items.EnsureItemsAreInstantiated();
        Item item = Items.ITEMS_DB[itemID];
        ItemID = itemID;
        Ingredients = ingredients;
        Name = item.Name;
        BasePrice = (int)Items.SodaInfo[Items.IndexOfXinY(ItemID, Items.Sodas)].PriceChange;
        Pic = item.icon;
        //MonoBehaviour.print("CREATED "  + Pic.name + " " + Name + " " + itemID);
        //MonoBehaviour.print("CREATED " + Items.ITEMS_DB[3].icon.name + " " + Items.ITEMS_DB[3].Name + " " + 3);
    }
}

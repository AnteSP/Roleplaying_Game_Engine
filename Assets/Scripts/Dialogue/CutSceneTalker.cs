using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Playables;

public class CutSceneTalker : MonoBehaviour
{
    [SerializeField] Dialogue D;
    [SerializeField] bool DBoxOnTop = false;
    [SerializeField] string[] Sentences;
    [SerializeField] AudioSource[] As;
    public int Index = 0;

    [SerializeField] GameObject[] Disable;
    [SerializeField] GameObject[] Enable;

    [SerializeField] Transform[] CamPos;
    Vector3 CamFocus;
    int cpI = 0;

    [SerializeField] CSMovement[] Movements;
    int mpI = 0;
    public CutSceneTalker alt = null;

    public bool goodtoGo = true;
    bool UseMouse = true;

    Rigidbody2D Crb;

    [SerializeField] int CamSize = 7;

    Animator Anim;
    public List<PlayableDirector> anims = new List<PlayableDirector>();

    public List<AudioSource> musicsQueue = new List<AudioSource>();

    public List<GameObject> destroyAfter = new List<GameObject>();

    public bool Ending = false;
    public bool skipPacking = false;
    /// <summary>
    /// Set this cutscene as Stats.currentcs then this cutscenetalker gets disabled until it is triggered by Stats or %S
    /// </summary>
    public bool setAsCurrent = false;
    public bool setAsCurrent_InstaPlay = false;
    public bool EndingChapter = false;
    public bool stopMusicOnStart = false;

    PlayableDirector pd;

    public string switchWhenDone = "";
    public int itemRequiredToStart = -1;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b">Is a cutscene ending? False if entering. True if leaving</param>
    public void PackUp(bool b)
    {
        if (!b && itemRequiredToStart != -1)
        {
            if (Items.ITEMS_DB.Length == 1)
            {//force Items to start
                Stats.current.GetComponent<Items>().enabled = false;
                Stats.current.GetComponent<Items>().enabled = true;
            }

            if (Items.Contains(itemRequiredToStart))
            {
                Items.AddNoAnim(31, -1);
            }
            else
            {
                this.enabled = false;
                print("Did not have item to start. Stopiing cs " + gameObject.name);
                return;
            }
        }

        Crb = Camera.main.GetComponent<Rigidbody2D>();
        Anim = D.GetComponent<Animator>();

        Slider.ForceBack();
        Stats.current.AllowSelecting = b;

        Stats.StartStopTime(b, "Cutscene");
        //Stats.current.PassTime = b;

        Camera.main.GetComponent<CamZoom>().SetSize(CamSize);
        Camera.main.GetComponent<CamZoom>().allowZooming = b;

        if(GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().color = Color.white;

        foreach(GameObject i in Disable)
        {
            //print("1for obj " + i.name + " and " + i.activeSelf);
            i.SetActive(b);
            //print("2for obj " + i.name + " and " + i.activeSelf);
        }
        foreach (GameObject i in Enable)
        {
            i.SetActive(!b);
        }
        if(Stats.current.Player == null)Stats.current.Player = GameObject.FindGameObjectWithTag("Player");

        if (Stats.current.Player != null)
        {
            Camera.main.transform.position = new Vector3(Stats.current.Player.transform.position.x, Stats.current.Player.transform.position.y, Camera.main.transform.position.z);
        }

        if (b)
        {
            CamZoom.setFocusPoint(Stats.current.Player.transform.position);
        }
        else
        {
            CamFocus = CamPos[0].position;
            CamZoom.setFocusPoint(CamFocus,ignorePhysics:true);
            if (stopMusicOnStart) Stats.changeBackgroundMusic(null);
        }


        UseMouse = b? UseMouse: Stats.current.Player.GetComponent<Movement>().UseMouse;

        if(Stats.current.Filter != null) Stats.current.Filter.GetComponent<Animator>().enabled = !b;

        this.enabled = b ? false : this.enabled; 

        D.gameObject.SetActive(!b);
        this.enabled = !b;
        print(b ? "Packed up and finished cutscene" : "Packed up and ready to do cutscene");
        if (!b)//if entering
        {
            //ObjectDepth.yeetSpaceBar();
            D.DisplayOnTop(DBoxOnTop);
            Stats.current.CurrentCS = this;
            D.CS = this;
        }

        Stats.releaseLockedInObject();
        ObjectDepth.yeetSpaceBar();

        if (Dialogue.d != null)
        {
            Dialogue.d.showDisplay(true);
            if (!b) Dialogue.d.ForceToDefaultText();
        }

        if (b)//if exiting
        {
            this.tag = "Untagged";
            foreach (GameObject g in destroyAfter) Destroy(g);
            if(switchWhenDone != "")
            {
                Progress.switchInPlay(switchWhenDone, true);
            }

            if(Index < As.Length)
            {
                print("AUDIO STOPPING " + As[Index].name);
                As[Index].Stop();
            }

        }
        else if(setAsCurrent_InstaPlay)//if entering && instaplaying
        {
            D.noInnactiveOnStart = true;
        }

        
        
    }

    private void OnEnable()
    {
        if (Stats.current == null)
        {
            Stats.current = GameObject.FindGameObjectWithTag("STATS").GetComponent<Stats>();
            Stats.current.Player = GameObject.FindGameObjectWithTag("Player");
        }
        if (setAsCurrent)
        {
            Stats.current.CurrentCS = this;
            setAsCurrent = false;
            if (!setAsCurrent_InstaPlay)
            {
                this.enabled = false;
                return;
            }

        }
        if(!skipPacking)PackUp(false);
        /*
        Points = points;
        if (TryGetComponent<NPCMovement>(out NPCMovement a))
        {
            N = a;
            NE = true;
        }

        N.rb = GetComponent<Rigidbody2D>();
        N.An = GetComponent<Animator>();

        GameObject P = GameObject.FindGameObjectWithTag("Player");
        P.GetComponent<Movement>().ShutDown();
        Camera.main.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        */
    }

    bool skipping = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && goodtoGo)
        {
            Next();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !Ending && alt == null && goodtoGo)
        {
            if (Sentences.Last().StartsWith("%#")) EndingChapter = true;
            Ending = true;
            skipping = true;
            D.EndCutScene(this);
            goodtoGo = false;
        }
        Vector2 temp = Offset();
        //if (UseMouse) CamFocus = (Vector2)CamPos[cpI].position + temp * temp * temp * 40;
        //Crb.position = Vector2.Lerp(Crb.position, CamPos[cpI].position, Vector2.Distance(Crb.position, CamPos[cpI].position)/10);
        //CamZoom.setFocusPoint(CamFocus,false);
        if (UseMouse) CamZoom.applyOffset(temp * temp * temp *40);

        if (Input.GetKeyDown(KeyCode.Mouse0) && goodtoGo)
        {
            if (Anim == null) Anim = D.GetComponent<Animator>();
            Anim.Play("Flip");
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && goodtoGo)
        {
            if (Anim == null) Anim = D.GetComponent<Animator>();
            Anim.Play("UnFlip");
        }
    }

    Vector2 Offset()
    {
        return new Vector2(Mathf.Clamp((Input.mousePosition.x - Screen.width / 2) / Screen.width, -0.5f, 0.5f), Mathf.Clamp((Input.mousePosition.y - Screen.height / 2) / Screen.height, -0.5f, 0.5f));
    }

    public bool GoToNextScene()
    {
        string lastSent = Sentences.Last();
        print("CHECKING SCENE" + lastSent);
        if (lastSent.StartsWith("%S"))
        {
            if(switchWhenDone != "")
            {
                Progress.switchInPlay(switchWhenDone, true);
            }

            if(lastSent.StartsWith("%S>")) 
                Stats.current.SceneChange(Sentences.Last().Substring(3));
            else
                Stats.current.SceneChange(Sentences.Last().Substring(2));
            return true;
        }
        return false;
    }

    public void Next()
    {
        D.gameObject.SetActive(true);

        D.Current = Sentences[Index];
        D.CS = this;

        if (As.Length <= Index) print("ERROR: MAKE SURE THERE'S AS MANY TYPE NOISE SHITS AS THERE ARE DIALOGUE BOXES DICKHEAD");
        D.SetTypeNoise(As[Index++]);
        D.NextSentence();
        try
        {
            
        }
        catch(System.Exception e)
        {
            D.EndCutScene(this);
            print("CUTSCENE ENDED DUE TO ERROR: " + e.Message + " @ " + e.Source);
        }
        
    }

    public bool isDone()
    {
        return Index == Sentences.Length - 1 || Index == Sentences.Length || skipping;
    }

    public void NextCamPos(int time = 200)
    {
        CamFocus = CamPos[++cpI].position;
        if(time == 0)
        {
            CamZoom.setFocusPoint(CamFocus, time, true,true);
        }
        else
        {
            CamZoom.setFocusPoint(CamFocus, time, true);
        }
        
    }

    public void NextMusicInQueue()
    {
        if (musicsQueue.Count == 0) return;
        AudioSource a = musicsQueue[0];
        musicsQueue.RemoveAt(0);
        Stats.changeBackgroundMusic(a.clip);
    }

    public void NextNPCMove(bool n)
    {
        Movements[mpI].ensureAnimasAreReady();
        Movements[mpI].movementPrep();
        Movements[mpI].nOrN(n);
        Movements[mpI].enabled = true;
        mpI++;
    }

    public void NextAnim()
    {
        foreach(PlayableDirector a in anims)
        {
            if (!a.name.EndsWith("[DONE_ANIM]"))
            {
                a.Play();
                a.name += "[DONE_ANIM]";
                goodtoGo = false;
                break;
            }
        }
    }

    public void setGoodToGo(bool b)
    {
        goodtoGo = b;
        if (b)
            Next();
    }

    public void setGoodToGoOnly(bool b)
    {
        goodtoGo = b;
    }
}
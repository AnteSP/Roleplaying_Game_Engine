using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI textDisplay;

    public float TypeSpeed;

    public NPCMovement NPC = null;
    public Talker talker = null; 
    public CutSceneTalker CS = null;
    Image im = null;

    public string Current = "";

    bool End = false;

    public AudioSource TypeNoise;
    public static Dialogue d;

    public void showDisplay(bool b)
    {
        im.enabled = b;
    }

    private void Start()
    {
        d = this;
        im = GetComponent<Image>();
    }

    IEnumerator Type()
    {
        bool TN = TypeNoise != null;
        if (TN)
        {
            TypeNoise.Play();
        }

        foreach(char letter in Current)
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(TypeSpeed);
        }

        if (TN)
        {
            TypeNoise.Stop();
        }

    }

    public void NextSentence()
    {
        textDisplay.text = "";
        StopAllCoroutines();
        End = false;
        textDisplay.enableWordWrapping = true;

        if (CS == null) ProcessPerc(); else ProcessPercCS();
        
        if (!End) StartCoroutine(Type());
    }

    public void EndConvo()
    {
        textDisplay.text = "";
        gameObject.SetActive(false);
        if (Stats.current.Player != null)
        {
            Stats.current.Player.GetComponent<Movement>().enabled = true;
        }
        End = true;
        try
        {
            NPC.enabled = true;
            NPC.ResetAn();
        } catch(System.Exception e){ }
        forceStopSounds();
        Stats.releaseLockedInObject();
    }

    public void EndCutScene(CutSceneTalker cs)
    {
        print("GOT HERE 2");
        cs.Ending = true;
        CS = cs;
        cs.goodtoGo = false;
        textDisplay.text = "";
        Stats.Transition(1);
        print("GOT HERE 3");
        gameObject.SetActive(false);
        forceStopSounds();

        //cs.PackUp(true);
        //cs.gameObject.SetActive(false);
        End = true;

        CS = null;
    }

    public void forceStopSounds()
    {
        StopCoroutine(Type());
        if (TypeNoise != null)
        {
            TypeNoise.Stop();
        }
    }

    void ProcessPerc()
    {

        if (Current[0] == '%')
        {
            char c = Current[1];
            print("Triggered Normal Percent Event " + c);
            Current = Current.Remove(0, 2);

            switch (c)
            {
                case 'L':
                    NPC.Face("left");
                    break;
                case 'R':
                    NPC.Face("right");
                    break;
                case 'U':
                    NPC.Face("up");
                    break;
                case 'D':
                    NPC.Face("down");
                    break;
                case 'P':
                    NPC.Face(GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().position);
                    break;
                case 'C':
                    NPC.GetComponent<Movement>().FocusPoint = NPC.transform.position;
                    break;
                case 'S'://Start cutscene
                    Stats.current.CurrentCS = talker.GetComponent<CutSceneTalker>();
                    Stats.current.AllowSelecting = false;
                    talker.gameObject.tag = "Untagged";
                    talker = null;
                    Stats.releaseLockedInObject();
                    Stats.Transition(0);
                    //Stats.current.CurrentCS.enabled = true;
                    break;
                case 's'://save
                    Progress.saveData();
                    break;
                case 'l'://load
                    Progress.loadData();
                    break;
                case '['://expected format     %[switchId]
                    Progress.switchInPlay( Current.Substring(0,Current.IndexOf(']')) , true);
                    Current = Current.Substring(Current.IndexOf(']')+1);
                    break;
                case '-'://expected format     %-[switchId]
                    Progress.switchInPlay(Current.Substring(1, Current.IndexOf(']')), false);
                    Current = Current.Substring(Current.IndexOf(']')+1);
                    break;
                case 'Q'://Quantity Query. How many of these switches are on? expected format     %Q[switchId1][switchId2][switchId3][switchId4]
                    int count = 0;
                    //Progress.readData();
                    while (Current[0] == '[')
                    {
                        if (Progress.getBool( Current.Substring(1, Current.IndexOf(']')-1) )) count++;
                        Current = Current.Substring(Current.IndexOf(']') + 1);
                    }
                    Current = count + Current;
                    break;
                case 'c':
                    Progress.checkProgress();
                    break;
                case 'Z':
                    Destroy(NPC.gameObject);
                    textDisplay.text = "";
                    gameObject.SetActive(false);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>().enabled = true;
                    End = true;
                    break;
                case 'K':
                    NPC.An.Play("Die");
                    NPC.enabled = false;
                    if(NPC.gameObject.name == "Nazi")
                    {
                        Stats.KillNazi();
                        NPC.gameObject.name = "Dead Nazi";
                    }
                    NPC = null;
                    break;
                case 'A':
                    NPC.alt();
                    break;
                case 'O':
                    textDisplay.enableWordWrapping = false;
                    break;
                case '4':
                    Current = "Would that be amusing for you, " + System.Environment.UserName+"?";
                    break;
                case '{'://format: {4,2}{70,1} [INSERT MESSAGE HERE]    for requiring two item 4s and one item 70

                    bool missingItems = false;

                    List<int> amounts = new List<int>();
                    List<int> items = new List<int>();

                    while(c == '{')
                    {
                        print(Current.Substring(0, Current.IndexOf(',')));
                        int item = int.Parse(Current.Substring(0, Current.IndexOf(',')));
                        print(Current.Substring(Current.IndexOf(',')+1, Current.IndexOf('}') - Current.IndexOf(',')-1));
                        int amount = int.Parse(Current.Substring( Current.IndexOf(',')+1 ,  Current.IndexOf('}') - Current.IndexOf(',')-1));
                        amounts.Add(amount);
                        items.Add(item);
                        if (!Items.Contains(item, amount)) {
                            print("NOT FOUND " + item + " x" + amount);
                            missingItems = true;
                        }
                        

                        c = Current[Current.IndexOf('}')+1];
                        Current = Current.Remove(0, Current.IndexOf('}')+2);
                    }

                    if (!missingItems)
                    {
                        Current = " * You have the required items to continue!";

                        for(int i = 0; i < amounts.Count; i++)
                            Items.Add(items[i], -amounts[i]);
                    }
                    else
                    {
                        print("ERROR FROM LACK OF ITEM");
                        Stats.DisplayMessage(Current);
                        throw new System.Exception();
                    }

                    break;
                case 'r'://give recipe. Use: %rX    X is the char in dic(below). Go to SodaMachine to find the index of recipe you want to add
                    Dictionary<char, int> dic = new Dictionary<char, int> { { 'a', 2 }, };
                    int ind = dic[Current[0]];
                    Current = Current.Remove(0, 1);
                    if ( SodaMachine.CreateRecipe(SodaMachine.Recipes[ind]) ) Items.ShiftAnim(SodaMachine.Recipes[ind].Pic, "NEW RECIPE", SodaMachine.Recipes[ind].Name);

                    break;
                case 'w'://wait
                    Stats.current.AllowSelecting = false;
                    StartCoroutine(wait( float.Parse(Current) ));
                    break;
                case 'a'://appear
                    talker.GetComponent<SpriteRenderer>().enabled = true;
                    break;
                case '~'://camera shake
                    Camera.main.GetComponent<CameraShake>().enabled = true;
                    break;
                case 'M'://mystery dissapearence
                    Destroy(talker.gameObject);
                    Stats.PlaySpecialSound();
                    Stats.current.Filter.GetComponent<Animator>().Play("ScreenFlash");
                    //StartCoroutine( Stats.current.fadeFilterColor(Color.cyan, new Color(1, 1, 1, 0), 1) );
                    EndConvo();
                    break;
                case 'm'://main menu
                    Stats.current.SceneChange("Main Menu");
                    break;
                case 'T'://Time skip
                    Stats.ChangeTimeAnim(90, 50);
                    break;
                case 'X':
                    Destroy(talker.gameObject);
                    EndConvo();
                    break;
                case '#'://Enable special object
                    if(Current.Length > 0)
                    {
                        char mode = Current.ToCharArray()[0];
                        if(mode == 'A')//swap object activeness + Play sound on special
                        {
                            //Stats.current.AllowSelecting = false;
                            //GetComponent<Image>().enabled = false;
                            talker.specialObj.SetActive(true);
                            talker.specialObj.GetComponent<AudioSource>().Play();
                            talker.gameObject.SetActive(false);
                            Stats.releaseLockedInObject();
                            EndConvo();
                        }
                        else if(mode == 'B')//Just make the object active and then end convo
                        {
                            if (talker.specialObj.activeSelf)
                            {
                                talker.specialObj.SetActive(false);
                            }
                            talker.specialObj.SetActive(true);
                            talker.resetIndex();
                            EndConvo();
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        Stats.current.AllowSelecting = false;
                        GetComponent<Image>().enabled = false;
                        talker.specialObj.SetActive(true);
                    }

                    break;
                case ':'://Change Scene     :SCENENAME
                    Stats.current.SceneChange(Current);
                    EndConvo();
                    break;
                case '%':
                    EndConvo();
                    break;
            }

        }

    }



    void ProcessPercCS()
    {

        if (Current[0] == '%')
        {
            char c = Current[1];
            print("Triggered CSPercent Event " + c);
            Current = Current.Remove(0, 2);

            switch (c)
            {
                case 'C':
                    CS.goodtoGo = false;
                    CS.NextCamPos();
                    break;
                case 'B':
                    CS.NextCamPos();
                    Camera.main.GetComponent<CamZoom>().SetSize(10);
                    break;
                case 'N':
                    CS.NextNPCMove(false);
                    CS.goodtoGo = false;
                    break;
                case 'n':
                    CS.NextNPCMove(true);
                    break;
                case '%':
                    EndCutScene(CS);
                    EndConvo();
                    break;
                case 'F':
                    Stats.current.Filter.GetComponent<Animator>().Play("Fade");
                    Stats.current.MuteSound(true);
                    break;
                case 'E':
                    Stats.current.SceneChange("Main Menu");
                    break;
                case 'A'://dont put 2 of these next to eachother
                    CS.NextAnim();
                    break;
                case 's':
                    Camera.main.GetComponent<CameraShake>().enabled = true;
                    break;
                case 'm':
                    AudioSource a = CS.musicsQueue[0];
                    CS.musicsQueue.RemoveAt(0);
                    Stats.changeBackgroundMusic(a.clip);
                    break;
                case 'i'://card intro seq. Do once to trigger. Once to end

                    GameObject cardObj = Camera.main.transform.Find("Card").gameObject;

                    if (cardObj.activeSelf)//clean up
                    {
                        cardObj.SetActive(false);
                        End = false;
                    }
                    else//start anim
                    {
                        cardObj.SetActive(true);
                        forceGoodToGo(false);
                        Dialogue.d.gameObject.SetActive(false);
                        End = true;
                    }

                    break;
                case 't'://transition
                    forceGoodToGo(false);
                    Stats.Transition(2);
                    
                    break;
                case 'w'://sWay
                    CameraSway Csw = Camera.main.GetComponent<CameraSway>();
                    Csw.enabled = !Csw.enabled;
                    break;
                case 'D'://Dark
                    Camera.main.GetComponent<UnityEngine.Rendering.Volume>().weight = 0.25f;
                    Stats.current.FilterColor(new Color(0, 0, 0,0));

                    break;
                case 'd'://decision
                    GameObject cb = textDisplay.transform.parent.parent.Find("Choice Box").gameObject;
                    cb.SetActive(true);
                    card[] cards = cb.GetComponentsInChildren<card>();
                    forceGoodToGo(false);
                    foreach(card card in cards)
                    {
                        card.launch();
                    }

                    break;
                case '#':
                    forceGoodToGo(false);
                    Stats.Transition(3);
                    Stats.current.CurrentCS.EndingChapter = true;
                    break;
                case 'S': //Current sentene will be used to switch to next scene
                    EndCutScene(CS);
                    EndConvo();
                    break;
                case 'I': //Information
                    Stats.DisplayMessage(Current);
                    Stats.current.CurrentCS.Next();
                    break;
            }

        }

    }

    IEnumerator wait(float secs)
    {
        Current = "";
        yield return new WaitForSeconds(secs);
        talker.Use(0);
        Stats.current.AllowSelecting = true;
    }

    static public void forceGoodToGo(bool val)
    {
        //print("SET");
        Dialogue.d.CS.goodtoGo = val;
    }
}

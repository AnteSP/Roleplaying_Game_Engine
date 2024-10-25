using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    AudioSource TypeNoise;
    public static Dialogue d;

    [SerializeField] Animator DialogueChoiceNotif;

    string delayedPercEvent = null;

    public void showDisplay(bool b)
    {
        textDisplay.enabled = b;
        im.enabled = b;
    }

    public void DisplayOnTop(bool b)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, b ? Mathf.Abs(transform.localPosition.y) : -Mathf.Abs(transform.localPosition.y), transform.localPosition.z);
    }

    public void DisplayOnTop()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, (Camera.main.transform.position.y > Stats.current.Player.transform.position.y) ? Mathf.Abs(transform.localPosition.y) : -Mathf.Abs(transform.localPosition.y), transform.localPosition.z);
    }

    private void Start()
    {
        d = this;
        im = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    public void SetTypeNoise(AudioSource ntn)
    {
        forceStopSounds();
        TypeNoise = ntn;
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

        if(delayedPercEvent != null)
        {
            Current = delayedPercEvent;
            if (CS == null) ProcessPerc(); else ProcessPercCS();
            delayedPercEvent = null;
        }

        if (TN)
        {
            TypeNoise.Stop();
        }

    }

    public void NextSentence()
    {
        QuickTalker.ONLYACTIVETALKER = false;
        textDisplay.text = "";
        StopAllCoroutines();
        if(TypeNoise != null) TypeNoise.Stop();
        End = false;
        textDisplay.enableWordWrapping = true;

        if (CS == null) ProcessPerc(); else ProcessPercCS();
        
        if (!End) StartCoroutine(Type());
    }

    public void EndConvo()
    {
        textDisplay.text = "";
        gameObject.SetActive(false);
        Stats.StartStopPlayerMovement(true, "Talker");

        End = true;
        try
        {
            NPC.enabled = true;
            NPC.ResetAn();
        } catch(System.Exception e){ }
        forceStopSounds();
        
        Stats.releaseLockedInObject(talker.unfocusAfterUse);
    }

    public void EndCutScene(CutSceneTalker cs)
    {
        cs.Ending = true;
        CS = cs;
        cs.goodtoGo = false;
        textDisplay.text = "";
        Stats.Transition(1);
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
                        Stats.DisplayMessage(Current,true);
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
                        else if(mode == 'C')//alternate special object's activeness
                        {
                            Current = Current.Remove(0, 1);
                            talker.specialObj.SetActive(!talker.specialObj.activeSelf);
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
                case 'd'://Start deadline
                    /*
                    if (delayedPercEvent == null)
                    {
                        delayedPercEvent = "%d" + Current;
                    }
                    else
                    {
                        char ID = Current[0];
                        Stats.StartDeadline(ID);
                    }*/
                    char ID = Current[0];
                    Stats.StartDeadline(ID);
                    Current = Current.Remove(0, 1);

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
                    //EndConvo();
                    break;
                case 'I': //Information
                    Stats.DisplayMessage(Current,true);
                    Stats.current.CurrentCS.Next();
                    Stats.current.CurrentCS.setGoodToGo(false);
                    break;
                case 'V': //Variable Line. Syntax Example:    %V[A]. blah blah blah       (Everything in [A] will be replaced by the variable line of ID A. If we want to decide between multiple, list multiple. First takes priority)

                    int closingBracketIndex = Current.IndexOf(']');

                    string insideBrackets = Current.Substring(1, closingBracketIndex - 1);
                    Current = Current.Substring(closingBracketIndex + 1);

                    CutSceneVarDia[] vards = Stats.current.CurrentCS.GetComponents<CutSceneVarDia>();

                    string fallback = "";
                    bool foundGoodLine = false;
                    foreach (char ch in insideBrackets)//Iterate over each ID
                    {
                        CutSceneVarDia v = vards.Where(a => a.ID == ch).FirstOrDefault();

                        if (v.isValid())
                        {
                            Current += v.Line;
                            foundGoodLine = true;
                        }
                        else
                        {
                            if (fallback == "") fallback = v.LineIfFalseAndFirstListed;
                        }
                    }

                    if (!foundGoodLine) Current += fallback;
                    DialogueChoiceNotif.SetTrigger("GoDec");

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

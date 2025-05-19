using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject Content;
    public TextMeshProUGUI version;
    [SerializeField] GameObject catBoyEasteregg;
    static MainMenu m = null;
    [SerializeField] UnityEngine.UI.Slider volSlider;
    static public float VOLUME = 1;
    public PlayableDirector tvTurnOn;
    [SerializeField] AudioSource clickNoise;
    static readonly int FINAL_CHAPTER = 2;

    [SerializeField] AudioMixer audioMixer;

    private void Start()
    {
        m = this;
        Progress.readData();
        VOLUME = Progress.getFloat("Volume");
        if (float.IsNaN(VOLUME)) VOLUME = 1;
        volSlider.value = VOLUME;

        int ch = Progress.getInt("Chapter");
        int i = 1;
        for(i = 1; i < Content.transform.childCount && i <= ch; i++)
        {
            GameObject g = Content.transform.GetChild(i).gameObject;
            if (!g.name.StartsWith("Chapter ")) continue;

            Transform play = g.transform.Find("Play Back").Find("Play");
            Transform fPlay = g.transform.Find("Play Back").Find("Free Play");

            play.GetComponent<Button>().interactable = false;
            fPlay.GetComponent<Button>().interactable = true;

            play.GetComponent<Tooltip>().tooltip = "Play Story (Erase save data to play again)";
            fPlay.GetComponent<Tooltip>().tooltip = "Free Play";
        }

        bool nextChp = true;
        for(;i < Content.transform.childCount && i <= FINAL_CHAPTER; i++)
        {
            GameObject g = Content.transform.GetChild(i).gameObject;
            if (!g.name.StartsWith("Chapter ")) continue;

            Transform play = g.transform.Find("Play Back").Find("Play");
            Transform fPlay = g.transform.Find("Play Back").Find("Free Play");

            play.GetComponent<Button>().interactable = nextChp;
            fPlay.GetComponent<Button>().interactable = false;
            play.GetComponent<Tooltip>().tooltip = nextChp ? "Play Story" : "Play Story (Locked)";
            nextChp = false;
        }
        int fun = Progress.getInt("FUN");
        string start = version.text.Substring(0, version.text.Length - 4);
        string funCode = "";

        funCode += (fun % 2 == 0 ? 'E' : 'O');
        switch (fun / 25)
        {
            case 0: funCode += 'B';break;
            case 1: funCode += 'D'; break;
            case 2: funCode += 'F'; break;
            case 3: funCode += 'G'; break;
            default: funCode += 'X';break;
        }
        switch (fun % 10)//right-most digit
        {
            case 0: case 1: funCode += 'A'; break;
            case 2: case 3: funCode += 'E'; break;
            case 4: case 5: funCode += 'I'; break;
            case 6: case 7: funCode += 'O'; break;
            case 8: case 9: funCode += 'U'; break;
        }
        switch (fun / 10)//2nd digit
        {
            case 0: funCode += 'J'; break;
            case 1: funCode += 'K'; break;
            case 2: funCode += 'L'; break;
            case 3: funCode += 'M'; break;
            case 4: funCode += 'N'; break;
            case 5: funCode += 'P'; break;
            case 6: funCode += 'R'; break;
            case 7: funCode += 'S'; break;
            case 8: funCode += 'T'; break;
            case 9: funCode += 'V'; break;
            default: funCode += 'X'; break;
        }

        version.text = start + funCode;

        if (Progress.getBool("ch1Catboy"))
        {
            catBoyEasteregg.SetActive(true);
        }
    }

    string sceneToLoad;
    bool startedAnim = false;
    public void LoadScene(string s)
    {
        if(!startedAnim) tvTurnOn.Play();
        LoadSceneInternal(s, false);
    }
    public void LoadSceneFreePlay(string s)
    {
        if (!startedAnim) tvTurnOn.Play();
        LoadSceneInternal(s, true);
    }

    void LoadSceneInternal(string s, bool doFreePlay)
    {
        sceneToLoad = s;
        //Stats.freePlay = doFreePlay;
        FreePlay.StartFreePlay();
        clickNoise.Play();
        NameIndic.turnOff();
    }


    public void ActuallyStartGame()
    {
        Stats.allTime = 0;
        Progress.saveData();
        SceneManager.LoadScene(sceneToLoad);
    }


    public void QuitGame()
    {
        UnityEngine.Application.Quit();


    }

    public void DeleteData()
    {
        Progress.deleteData();
        for (int i = 0; i < 1; i++)//HARDCODED CHAPTER VALUE HERE = 1
        {
            GameObject g = Content.transform.GetChild(i).gameObject;
            if (!g.name.StartsWith("Chapter ")) continue;

            Transform play = g.transform.Find("Play Back").Find("Play");
            Transform fPlay = g.transform.Find("Play Back").Find("Free Play");

            play.GetComponent<Button>().interactable = true;
            fPlay.GetComponent<Button>().interactable = false;

            play.GetComponent<Tooltip>().tooltip = "Play Story";
            fPlay.GetComponent<Tooltip>().tooltip = "Free Play (Locked)";
        }
    }

    public static void ForceStart()
    {
        m.Start();
    }

    float volSpamCounter = 0;
    bool isZero = false,evilSliderUp = false;

    [SerializeField] GameObject normalVolSlider, evilVolSlider;

    public void ChangeVolume()
    {
        VOLUME = volSlider.value;
        Progress.setFloat("Volume", VOLUME);
        //volSlider.value;

        if (!evilSliderUp)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(VOLUME == 0 ? -0.1f : VOLUME) * 20);

            if (volSlider.value == 0 && !isZero)
            {
                volSpamCounter++;
                isZero = true;
            }
            else if (volSlider.value == 1 && isZero)
            {
                isZero = false;
                volSpamCounter++;
            }

            if (volSpamCounter > 6 && VOLUME > 0.5f)
            {
                print("FUCK MY AAAASS");
                evilSliderUp = true;
                normalVolSlider.SetActive(false);
                evilVolSlider.SetActive(true);
                volSlider = evilVolSlider.GetComponentInChildren<UnityEngine.UI.Slider>();
            }
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(VOLUME == 0 ? -0.1f : VOLUME) * 20);
        }



    }

    private void Update()
    {
        if(volSpamCounter > 0)
        {
            volSpamCounter -= 0.05f;
        }
        
        
    }
}

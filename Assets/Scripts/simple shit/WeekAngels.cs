using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeekAngels : MonoBehaviour
{
    public bool OnAngel = true;
    public string[] Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,final;
    public Sprite[] sprites;
    SpriteRenderer spr;
    Talker t;
    static WeekAngels anim;

    [SerializeField] AudioSource intro, theRest;


    private void OnEnable()
    {
        if(gameObject.name == "Urim and Thummim" && !Progress.doesPickUpExist("*Ch1/Urim and Thummim"))
        {
            anim.GetComponent<Animator>().enabled = true;
        }
    }
    //0 = sunday
    //Michael (Sunday), Gabriel (Monday), Uriel (Tuesday), Raphael (Wednesday), Selaphiel (Thursday), Jegudiel (Friday), and Barachiel (Saturday)
    // Start is called before the first frame update
    void Start()
    {
        if (!OnAngel)
        {
            if (gameObject.name == "park") anim = this;
            return;
        }

        if(Progress.doesPickUpExist("*Ch1/Urim and Thummim"))
        {
            Destroy(this.gameObject);
        }
        
        int dow = (int)DateTime.Now.DayOfWeek;
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spr.sprite = sprites[dow];
        t = GetComponent<Talker>();

        switch (dow)
        {
            case 0:
                t.overWriteSentences(Sunday);
                break;
            case 1:
                t.overWriteSentences(Monday);
                break;
            case 2:
                t.overWriteSentences(Tuesday);
                break;
            case 3:
                t.overWriteSentences(Wednesday);
                break;
            case 4:
                t.overWriteSentences(Thursday);
                break;
            case 5:
                t.overWriteSentences(Friday);
                break;
            case 6:
                t.overWriteSentences(Saturday);
                break;
        }

        for(int i = 0; i < 7;i++)
        {
            if (!Progress.getBool("WeekAngel" + i)) return;//stop if any of these is false
        }

        t.overWriteSentences(final);
    }

    public void animEvent()
    {
        intro.Play();
        Stats.current.GetComponent<AudioSource>().mute = true;
        //Camera.main.GetComponent<UnityEngine.Rendering.Volume>().weight = 0.15f;
        FreePlay.resetCamProfile();

    }

    public void staticStart()
    {
        intro.Stop();
        theRest.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

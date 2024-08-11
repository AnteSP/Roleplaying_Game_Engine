using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bidench1 : MonoBehaviour
{
    float Timer = 0;
    static float BigTimer = 0;
    AudioSource[] aud;
    float interval = 0;
    int ind = 0;
    static int bCount = 1;

    float v = float.NaN;

    // Start is called before the first frame update
    void Start()
    {
        if (float.IsNaN(v))
        {
            v = Progress.getFloat("Volume");
        }
        aud = GetComponents<AudioSource>();
        
        interval = aud[0].clip.length / aud.Length;

        if (!float.IsNaN(v))
        {
            foreach (AudioSource a in aud)
            {
                a.volume *= v;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        BigTimer += Time.deltaTime;

        if(Timer > interval)
        {
            aud[ind].Play();
            ind++;
            ind = ind % aud.Length;
            Timer = 0;
        }

        if(BigTimer > 10)
        {
            Instantiate(this.gameObject, new Vector3(Random.Range(40,120), 20, transform.position.z), transform.rotation);
            bCount++;
            Stats.Debug("Biden Count: " + bCount);
            BigTimer = 0;
        }

        transform.position = Vector3.Lerp(transform.position, Stats.current.Player.transform.position, Time.deltaTime*0.9f);
        //transform.position = Stats.current.Player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            print("GAME CRASHED");
            Progress.markDataAsUnloaded();
            Application.Quit();
        }

        
    }
}

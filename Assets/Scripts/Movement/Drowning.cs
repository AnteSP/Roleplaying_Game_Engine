using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drowning : MonoBehaviour
{

    Rigidbody2D rb;
    Collider2D col;

    Transform water;

    float wFloor;
    float depth = 0;

    GameObject Shadow;

    float drownin = 0;
    [SerializeField] AudioSource Drown,DrownEnd;

    Movement PMove;

    [SerializeField] EdgeCollider2D Shore;

    bool DrownP = false;
    bool AlreadyPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        water = transform.Find("Drowning Bar");
        wFloor = water.localPosition.y;

        Shadow = transform.Find("Shadow").gameObject;

        PMove = GetComponent<Movement>();
    }

    private void Update()
    {
        drownin +=  DrownP ? Time.deltaTime : 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            depth = Shore.Distance(col).distance;
            depth = depth > 10 ? 10 : depth;
            depth = depth * depth;

            PMove.SlowDown = depth;

            water.localPosition = new Vector3(water.localPosition.x, Mathf.Clamp(wFloor + (-wFloor * (depth - 1) / 40), wFloor, 0), water.localPosition.z);

            float temp3 = Mathf.Clamp((2 * (depth - 1) / 40), 0, 2);

            water.localScale = new Vector3(1, temp3, 1);

            bool temp2 = temp3 < 0.02f;
            water.gameObject.SetActive(!temp2);
            Shadow.SetActive(temp2);

            if (depth > 1)
            {

                if (depth > 25 && PMove.enabled)
                {
                    DrownP = true;
                    //print(Drown.time);
                }
                else
                {
                    drownin = 0;
                    Drown.Stop();
                    DrownP = false;
                    AlreadyPlayed = false;
                }

                Stats.current.Filter.color = new Color(0, 0, 1 / Drown.clip.length, 1 / Drown.clip.length) * drownin;

                if (drownin > 0)
                {
                    if (!Drown.isPlaying && !AlreadyPlayed)
                    {
                        Drown.Play();
                        AlreadyPlayed = true;
                    }

                    Drown.volume = drownin / 10;

                    if (drownin > Drown.clip.length + 0.8f)
                    {
                        Stats.GameOver();
                        DrownEnd.Play();
                        DrownP = false;
                        AlreadyPlayed = false;
                    }
                }
            }
        }

        


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            PMove.SlowDown = 1;

            water.localPosition = new Vector3(water.localPosition.x, wFloor, water.localPosition.z);

            water.localScale = new Vector3(1, 0, 1);

            water.gameObject.SetActive(false);
            Shadow.SetActive(true);
        }
    }

}

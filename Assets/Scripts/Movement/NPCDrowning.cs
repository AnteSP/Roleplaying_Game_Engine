using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDrowning : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer spr;
    Color OGCol,Dif;

    Transform water;

    float wFloor;

    GameObject Shadow;

    float drownin = 0;
    float depth = 0;

    NPCMovement Move;
    float OGSPeed = 0;

    bool calculate = false;

    [SerializeField] AudioSource DrownEnd;
    [SerializeField] EdgeCollider2D Shore;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();

        water = transform.Find("Drowning Bar");
        wFloor = water.localPosition.y;

        Shadow = transform.Find("Shadow").gameObject;

        Move = GetComponent<NPCMovement>();

        OGSPeed = Move.Speed;
        OGCol = spr.color;
        Dif = OGCol - Color.blue;
    }

    private void OnWillRenderObject()
    {
        calculate = transform.hasChanged;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (calculate && collision.tag == "Water")
        {
            depth = Shore.Distance(col).distance;
            depth = depth > 10 ? 10 : depth;
            depth = depth * depth;

            Move.Speed = OGSPeed / depth;
            Move.Speed = Move.Speed > OGSPeed ? OGSPeed : Move.Speed;

            water.localPosition = new Vector3(water.localPosition.x, Mathf.Clamp(wFloor + (-wFloor * (depth - 1) / 40), wFloor, 0), water.localPosition.z);

            float temp3 = Mathf.Clamp((2 * (depth - 1) / 40), 0, 2);

            water.localScale = new Vector3(water.localScale.x, temp3, 1);

            bool temp2 = temp3 < 0.02f;
            water.gameObject.SetActive(!temp2);
            Shadow.SetActive(temp2);

            calculate = false;
        }

        if (depth > 25)
        {
            drownin += Time.deltaTime;

            spr.color = OGCol - Dif*(drownin/10);

            if (drownin > 10)
            {
                DrownEnd.Play();
                if(gameObject.name == "Skeptic")
                {
                    Stats.DisplayMessage("A feeling of dread washes over you");
                    Stats.current.MuteSound(true);
                }else if(gameObject.name == "Nazi")
                {
                    Stats.KillNazi();
                }
                gameObject.SetActive(false);
            }
        }
        else
        {
            drownin = 0;
            spr.color = OGCol;
        }


    }
}

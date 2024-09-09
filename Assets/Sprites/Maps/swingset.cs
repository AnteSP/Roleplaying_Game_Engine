using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swingset : MonoBehaviour
{

    Vector2 OGPos;
    Rigidbody2D rb;
    BoxCollider2D bc;
    SpriteRenderer spr;
    float OGRotation;

    public float intensity = 10f,rotationRigidity = 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        spr = GetComponent<SpriteRenderer>();

        OGPos = rb.position;
        OGRotation = rb.rotation;

    }

    float dist,ydist;
    // Update is called once per frame
    void Update()
    {
        dist = Vector2.Distance(OGPos, rb.position);
        if (dist > 1)
        {
            bc.enabled = false;
        }
        else
        {
            bc.enabled = true;
        }

        if (dist > 0.1f)
        {
            rb.AddForce((OGPos - rb.position) * intensity);

            ydist = OGPos.y - rb.position.y;

            if(ydist > 0.1f)
            {
                transform.localScale = new Vector3(1, -ydist + 1.1f, 1);
            }else if(ydist < 0.4f)
            {
                transform.localScale = new Vector3(1, ydist + 1.4f, 1);
            }
            //y = mx + b
            //1 = 0.1m + b 
            //b = 1 - 0.1m
            //0.1 = m + 1 - 0.1m
            //-0.9 = m - 0.1m
            //-0.9 = 0.9m
            //m = -1
            //y = -x + 1.1
                
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Mathf.Abs(rb.rotation - OGRotation) > 0.1f)
        {
            rb.AddTorque((OGRotation - rb.rotation) * rotationRigidity);
        }

    }
}

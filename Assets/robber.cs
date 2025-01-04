using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robber : MonoBehaviour
{
    public float speed = 10f; // Speed of movement
    private Rigidbody2D rb;
    private Vector2 lastNormal; // Store the last collision normal
    Vector2 lastVel;
    AudioSource[] auds;
    SpriteRenderer spr;
    int lives = 3;
    readonly int iFrames = 60;
    int iFramesSpent = 60;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        auds = GetComponents<AudioSource>();
        rb.velocity = new Vector2(-1,1).normalized * speed; // Set random initial direction
    }

    void FixedUpdate()
    {
        // Maintain constant speed
        
        rb.velocity = rb.velocity.normalized * speed;
        lastVel = rb.velocity;

        if(lastVel == Vector2.zero)
        {
            rb.velocity = new Vector2(Random.value < 0.5f ? -1 : 1, Random.value < 0.5f ? -1 : 1);//random diagnol direction
        }
        if (iFramesSpent < iFrames)
        {
            spr.flipY = true;
            iFramesSpent++;
        }
        else
        {
            spr.flipY = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player" && iFramesSpent == iFrames)
        {
            auds[0].Play();
            lives -= 1;
            spr.color = new Color(1, 1f+ (lives-3f)/3f, 1f + (lives - 3f)/3f);
            iFramesSpent = 0;
            if (lives == 0)
            {
                auds[1].Play();
                GetComponent<Collider2D>().enabled = false;
                Items.Add(31, 1);
                speed = 100;
            }
        }
        // Get the collision normal
        Vector2 normal = collision.contacts[0].normal;
        lastNormal = normal; // Store normal for gizmo drawing


        // Determine which component of velocity to invert based on collision normal
        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        {
            rb.velocity = new Vector2(-lastVel.x, lastVel.y);
        }
        else
        {
            rb.velocity = new Vector2(lastVel.x, -lastVel.y);
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        rb.velocity = new Vector2(Random.value < 0.5f ? -1 : 1, Random.value < 0.5f ? -1 : 1);//random diagnol direction if we're staying on a surface
    }

    void OnDrawGizmos()
    {
        // Draw normal direction when a collision occurs
        if (rb != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position, (Vector2)transform.position + lastNormal);
        }
    }

}

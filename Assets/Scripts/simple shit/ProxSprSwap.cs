using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxSprSwap : MonoBehaviour
{
    Transform player;
    float dist;
    [SerializeField] float outterDist;
    [SerializeField] Sprite outterSprite, innerSprite;
    Sprite innactiveSprite;
    SpriteRenderer spr;
    // Start is called before the first frame update
    void Start()
    {
        player = Stats.current.Player.transform;
        spr = GetComponent<SpriteRenderer>();
        innactiveSprite = spr.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInside)
        {
            dist = Vector2.Distance(player.position, transform.position);
            spr.sprite = (dist <= outterDist) ? outterSprite : innactiveSprite;
        }

    }

    bool playerInside = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerInside = true;
            spr.sprite = innerSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInside = false;
        }
    }
}

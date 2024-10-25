using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class seamlessScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f;  // Speed of scrolling
    private Vector3 startPos;
    private SpriteRenderer spriteRenderer;
    private float textureUnitSizeX;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;

        // Calculate the size of the texture based on the sprite's size in world units
        textureUnitSizeX = spriteRenderer.sprite.bounds.size.x * transform.lossyScale.x;
    }

    void Update()
    {
        // Scroll the texture to the right by changing the position over time
        float newPosX = Mathf.Repeat(Time.time * scrollSpeed, textureUnitSizeX);
        transform.position = startPos + new Vector3(newPosX, 0,0) ;
    }
}
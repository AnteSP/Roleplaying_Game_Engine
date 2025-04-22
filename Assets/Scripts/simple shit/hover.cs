using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hover : MonoBehaviour
{
    float ogY;
    void Start()
    {
        ogY = transform.position.y;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, ogY + (Stats.secSin * 0.3f), transform.position.z);
    }
}

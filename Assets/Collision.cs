using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public string checkFor = "";
    public GameObject turnInto;
    public AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    } 


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.name == checkFor)
        {
            print("GOT HIM");
            turnInto.SetActive(true);
            sound.Play();

            gameObject.SetActive(false);
        }
    }
}

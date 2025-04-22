using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSanctum : MonoBehaviour
{
    [SerializeField] AudioSource DingNoise;
    [SerializeField] List<GameObject> LoadOnEntry;
    [SerializeField] List<GameObject> LoadOffEntry;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player" && !collision.isTrigger)
        {
            Stats.StartStopTime(false, "sanctum");
            DingNoise.pitch = 2;
            DingNoise.Play();

            foreach(GameObject g in LoadOnEntry)
            {
                g.SetActive(true);
            }

            foreach (GameObject g in LoadOffEntry)
            {
                g.SetActive(false);
            }
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !collision.isTrigger)
        {
            Stats.StartStopTime(true, "sanctum");
            DingNoise.pitch = 3;
            DingNoise.Play();

            foreach (GameObject g in LoadOnEntry)
            {
                g.SetActive(false);
            }

            foreach (GameObject g in LoadOffEntry)
            {
                g.SetActive(true);
            }
        }
            
    }
}

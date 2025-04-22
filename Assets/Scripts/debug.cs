using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{

    [SerializeField] string inp;
    [SerializeField] Sprite sp1, sp2;
    //UPGRADE!: Bristol Board Sign
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Animation>().Play("TEST");
        }
    }

    public void click()
    {
        Items.ShiftAnim(null, inp, "");
    }

    public void SPR1(int frame)
    {
        if(frame == 1)
        {
            GetComponent<SpriteRenderer>().sprite = sp1;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sp2;
        }
        
    }
}

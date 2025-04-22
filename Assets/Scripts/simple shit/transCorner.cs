using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transCorner : MonoBehaviour
{
    float timer = 0;
    bool inside = false;
    Animator replacer;
    [SerializeField] Sprite pic;
    //[SerializeField] Sprite spr;
    // Start is called before the first frame update
    void Start()
    {
        replacer = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inside) return;
        timer += Time.deltaTime;
        if (timer > 10)
        {
            timer = 0;
            Animator anim = Stats.current.Player.GetComponent<Animator>();
            RuntimeAnimatorController temp = anim.runtimeAnimatorController;
            anim.runtimeAnimatorController = replacer.runtimeAnimatorController;
            GetComponent<AudioSource>().Play();
            replacer.runtimeAnimatorController = temp;
            anim.Play("IdleDown");
            StartCoroutine( Stats.current.fadeFilterColor(new Color(1, 145f/255f, 181f/255f,1), new Color(255, 145f/255f, 181f/255f, 0), 2,pic));
            Progress.switchInPlay("ch1Catboy", true);
            //Stats.current.Player.GetComponent<SpriteRenderer>().sprite = spr;
            //Stats.PlaySpecialSound();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inside = false;
    }
}

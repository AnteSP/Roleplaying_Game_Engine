using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class card : MonoBehaviour
{
    char kind = 'c';
    Rigidbody2D rb = null;
    Collider2D col;
    public int power = 1000000;
    bool counting = false;
    float t = 0;
    public static bool allowClick = true;
    [SerializeField] string ID;
    [SerializeField] bool launchOnAwake = false;
    Vector3 OGPos;

    private void OnEnable()
    {
        OGPos = transform.position;
        if (launchOnAwake)
        {
            launch();
        }
    }

    private void OnDisable()
    {
        if(launchOnAwake)
            transform.position = OGPos;
    }
    //0.733

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            t += Time.deltaTime;
            if(t > 0.5f)
            {
                if(gameObject.name == "1")
                {
                    Stats.current.CurrentCS.enabled = false;
                    Stats.current.CurrentCS = Stats.current.CurrentCS.alt;
                    Stats.current.CurrentCS.skipPacking = true;
                    Stats.current.CurrentCS.enabled = true;
                    Progress.setInt(ID, 1);
                    print("GOT HERE 1" + ID);
                }
                else
                {
                    Progress.setInt(ID, 2);
                    print("GOT HERE 2" + ID);
                }
                transform.parent.gameObject.SetActive(false);
                Dialogue.forceGoodToGo(true);
                //CamZoom.cz.TempSetSize(-1);
                //Stats.current.CurrentCS
            }
        }
    }

    public void click()
    {
        if (!allowClick) return;

        foreach(Button b in transform.parent.GetComponentsInChildren<Button>())
        {
            b.interactable = false;
        }

        rb.AddForce(new Vector2(0, -power));
        col.enabled = false;
        counting = true;
        allowClick = false;

        if(gameObject.name == "1")//negative
        {
            switch (ID)
            {
                case "Are you scared of me?":
                    Progress.setInt("MRespect", -1);
                    break;

            }
        }
        else//positive
        {
            switch (ID)
            {
                case "Are you scared of me?":
                    Progress.setInt("MRespect", -2);
                    break;
            }
        }


    }

    public void launch()
    {
        print("launched");
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
        }
        rb.AddForce(new Vector2(-power / 10, power));
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-20, 20));
    }
}

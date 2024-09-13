using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDepth : MonoBehaviour
{

    static public GameObject Selected;

    List<GameObject> Nearby = new List<GameObject>();
    List<Color> Cols = new List<Color>();

    [SerializeField] Transform SpaceBarPrompt;
    [SerializeField] float SpaceBarPromptHeight;
    static public Transform Space;

    static Vector3 Innactive;
    
    // Start is called before the first frame update
    void Start()
    {
        Selected = gameObject;
        Innactive = SpaceBarPrompt.position;
        Space = SpaceBarPrompt;
    }
    /*
    private void LateUpdate()
    {
        string h = "LIST: ";
        
        for(int i = 0; i < Nearby.Count; i++)
        {
            h += Nearby[i].name;

        }
        print(h);
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //SetDepth(collision.gameObject);
        Interactable(collision.gameObject, true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //SetDepth(collision.gameObject);
        if (transform.hasChanged && Stats.current.AllowSelecting)
        {
            NearestSelect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable(collision.gameObject, false);
    }

    void SetDepth(GameObject g)
    {
        if(g.tag == "Machine" || g.tag == "SodaMachine" || g.tag == "GenericObj")
        {
            try
            {
                g.GetComponent<SpriteRenderer>().sortingOrder = g.transform.Find("Orientor").position.y > transform.position.y ? 0 : 2;



            } catch(System.Exception e)
            {
                g.GetComponent<SpriteRenderer>().sortingOrder = g.transform.position.y > transform.position.y ? 0 : 2;

            }

            
        }
    }
    /*
    public void ToggleSelection(bool b)
    {

        Stats.current.AllowSelecting = b;
        SpaceBarPrompt.position = Innactive;


    }
    */
    public static void yeetSpaceBar()
    {
        if(Space != null)Space.position = Innactive;
    }

    void removeFromNearby(GameObject g)
    {
        Selected = g == Selected ? gameObject : Selected;

        if (Nearby.IndexOf(g) != -1)
        {
            g.GetComponent<SpriteRenderer>().color = Cols[Nearby.IndexOf(g)];
            Cols.RemoveAt(Nearby.IndexOf(g));
        }
        SpaceBarPrompt.position = Innactive;
        Nearby.Remove(g);
    }

    void Interactable(GameObject g, bool enter)
    {
        bool objSelectable = (g.tag == "Machine" || g.tag == "SodaMachine");

        if (objSelectable)
        {
            if (Stats.current.AllowSelecting)
            {
                //Add to list
                if (enter)
                {
                    if (!Nearby.Contains(g))
                    {
                        Nearby.Add(g);
                        Cols.Add(g.GetComponent<SpriteRenderer>().color);
                    }
                }
                else
                    removeFromNearby(g);

                NearestSelect();
            }
            else
            {
                removeFromNearby(g);
            }
        }


    }

    void NearestSelect()
    {
        float dist = Mathf.Infinity;
        bool activeNearbyExists = false;

        for (int i = 0; i < Nearby.Count; i++)
        {
            if (!Nearby[i].activeSelf) continue;
            activeNearbyExists = true;
            Nearby[i].GetComponent<SpriteRenderer>().color = Cols[i];

            bool temp = Vector2.Distance(transform.position, Nearby[i].transform.position) < dist;

            dist = temp ? Vector2.Distance(transform.position, Nearby[i].transform.position) : dist;
            Selected = temp ? Nearby[i] : Selected;

        }
        if (!activeNearbyExists)
        {
            yeetSpaceBar();
        }
        else
        {
            bool IsPlayer = Selected == gameObject;
            Selected.GetComponent<SpriteRenderer>().color = IsPlayer ? Color.white : (Cols[Nearby.IndexOf(Selected)] - new Color(0.4f, 0.4f, 0.4f, 0));
            SpaceBarPrompt.position = IsPlayer ? Innactive : new Vector3(Selected.transform.position.x, Selected.transform.position.y + SpaceBarPromptHeight, SpaceBarPrompt.position.z);
        }

    }

}

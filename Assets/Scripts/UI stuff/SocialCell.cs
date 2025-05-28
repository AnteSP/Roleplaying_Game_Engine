using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialCell : MonoBehaviour
{
    public string friendshipVar = "";
    static Transform par;

    [SerializeField] RectTransform scale, index;
    Tooltip t;
    bool started = false;

    private void Start()
    {
        ForceStart();
    }

    public void ForceStart()
    {
        if (started) return;
        started = true;
        par = transform.parent;
        t = GetComponent<Tooltip>();
    }

    public void refresh()
    {

        if (!Progress.doesFieldExist(friendshipVar))
        {
            gameObject.SetActive(false);
        } else{
            gameObject.SetActive(true);
        }

        int f = Progress.getInt(friendshipVar);
        //This is intended to go from -10 to 10

        index.localPosition = new Vector3(f* scale.sizeDelta.x/20, index.localPosition.y, index.localPosition.z);
        t.tooltip = "Friendship: " + f + "/10";
    }

    public static void refreshSocial()
    {
        //print("PAR NULL? " + (par == null));
        if(par != null)
        {
            foreach (SocialCell cell in par.GetComponentsInChildren<SocialCell>(includeInactive:true))
            {
                cell.refresh();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

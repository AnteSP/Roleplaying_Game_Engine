using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class outfit : MonoBehaviour
{
    public static readonly int[] outfitItemIDs = new int[] { 26, 27,28,29,30 };
    readonly static int noItemNeeded = 30 ;

    static List<int> outfitsOwned = new List<int>() { noItemNeeded };
    static int outfitInd = 0;

    public static GameObject current;

    [SerializeField]TextMeshProUGUI title;
    [SerializeField] Image img;
    Tooltip desc = null;

    public List<RuntimeAnimatorController> playerOutfitAnims;

    public List<Animator> animsToAlsoChange = new List<Animator>();

    private void Awake()
    {
        current = gameObject;
        
        desc = img.GetComponent<Tooltip>();

        //checkOutfits();
    }

    public static List<int> getOutfitsOwned()
    {
        return outfitsOwned;
    }

    public static int getActiveOutfit()
    {
        print("Giving outfit ind " + outfitInd);
        return outfitsOwned[outfitInd];
    }

    public static void ResetOutfitStuff()
    {
        //outfitInd = 0;
        current = null;
    }

    static void resetOutfitsOwned()
    {
        outfitsOwned = new List<int>() { noItemNeeded };
    }

    public static void checkOutfits()
    {
        if (current == null) return;
        int count = 0;
        resetOutfitsOwned();
        foreach(int id in outfitItemIDs)
        {
            if (Items.Contains(id))
            {
                if(noItemNeeded != id)count++;

                if(!outfitsOwned.Contains(id))
                    outfitsOwned.Add(id);
            }
        }

        current.SetActive(count > 0);

        if (count > 0)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        //Stats.Debug("Ind: " + outfitInd);
    }

    public void incrementOutfit(bool decrement = false)
    {
        outfitInd = outfitInd + ( decrement ? -1 : 1);
        if (outfitInd < 0) outfitInd = outfitsOwned.Count-1;
        outfitInd = outfitInd % outfitsOwned.Count;
        SetOutfit(outfitsOwned[outfitInd]);

    }

    public void SetOutfit(int itemInd)
    {
        print("BEFORE: " + outfitInd);
        for(int i = 0; i < outfitsOwned.Count;i++)
        {
            print("CHECK: " + outfitInd + " " + i + " " + outfitsOwned[i] + " " + outfitsOwned[outfitInd]);
            if (outfitsOwned[i] == itemInd)
            {
                outfitInd = i;
                continue;
            }
        }
        print("AFTER: " + outfitInd);
        print("ITEMID: " + itemInd);
        Item selectedOutfit = Items.ITEMS_DB[itemInd];
        //print("OUTFIT GOT " + selectedOutfit);

        img.sprite = selectedOutfit.icon;
        title.text = selectedOutfit.Name;
        if(desc != null)desc.tooltip = selectedOutfit.description;
        NameIndic.Indicate("");
        //print("ID = " + outfitInd + " AND " + itemInd);

        if (Stats.current.Player != null)
        {
            Animator anim = Stats.current.Player.GetComponent<Animator>();
            //print("X IN Y " + Items.IndexOfXinY(itemInd, outfitItemIDs));
            anim.runtimeAnimatorController = playerOutfitAnims[Items.IndexOfXinY(itemInd, outfitItemIDs)];
        }

        foreach (Animator a in animsToAlsoChange)
        {
            if (a.gameObject.activeSelf)
            {
                a.runtimeAnimatorController = playerOutfitAnims[Items.IndexOfXinY(itemInd, outfitItemIDs)];
                a.SetInteger("Vertical", 1);
            }
            else
            {
                a.gameObject.SetActive(true);
                a.runtimeAnimatorController = playerOutfitAnims[Items.IndexOfXinY(itemInd, outfitItemIDs)];
                a.SetInteger("Vertical", 1);
                a.gameObject.SetActive(false);
            }
            //a.gameObject.SetActive(true);

            //a.gameObject.SetActive(false);
        }
    }
}

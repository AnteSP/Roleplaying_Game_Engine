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

    static GameObject current;

    [SerializeField]TextMeshProUGUI title;
    [SerializeField] Image img;
    Tooltip desc;

    public List<RuntimeAnimatorController> playerOutfitAnims;

    // Start is called before the first frame update
    void Start()
    {
        current = gameObject;
        checkOutfits();
        desc = img.GetComponent<Tooltip>();
    }

    static void resetOutfitsOwned()
    {
        outfitsOwned = new List<int>() { noItemNeeded };
    }

    public static void checkOutfits()
    {
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
        
    }

    public void incrementOutfit(bool decrement = false)
    {
        outfitInd = outfitInd + ( decrement ? -1 : 1);
        outfitInd = outfitInd % outfitsOwned.Count;
        int itemInd = outfitsOwned[outfitInd];

        Item selectedOutfit = Items.ITEMS_DB[itemInd];

        img.sprite = selectedOutfit.icon;
        title.text = selectedOutfit.Name;
        desc.tooltip = selectedOutfit.description;
        NameIndic.Indicate("");

        if(Stats.current.Player != null)
        {
            Animator anim = Stats.current.Player.GetComponent<Animator>();
            print("X IN Y " + Items.IndexOfXinY(itemInd, outfitItemIDs));
            anim.runtimeAnimatorController = playerOutfitAnims[ Items.IndexOfXinY(itemInd, outfitItemIDs) ];
        }
        
    }
}

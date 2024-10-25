using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseSellSoda : Resource
{
    [SerializeField] GameObject Menu;

    //[SerializeField] int[] ItemsForSale;
    [SerializeField] List<int> ItemsForSale = new List<int>();

    [SerializeField] GameObject ItemPrefab;
    [SerializeField] GameObject StatPrefab;

    [SerializeField] Scrollbar HorizScroll;

    float Ratio;

    [SerializeField] GameObject EmptyNotif;
    [SerializeField] GameObject StatsEmptyNotif;

    [SerializeField] Transform Content;
    [SerializeField] Transform StatsContent;

    public AudioSource CashSound;

    public TextMeshProUGUI Money, Time, StatFinalResult;
    /// <summary>
    /// MLevel and SLevel are based on the enviornment. TimeMult and SMult are based on upgrades
    /// </summary>
    public float MLevel = 1f, SLevel = 0.1f;
    /// <summary>
    /// TimeMult and SMult are based on upgrades. MLevel and SLevel are based on the enviornment
    /// </summary>
    public float TimeMult = 1f,SMult = 1f;

    List<SellSodas> BeingSold = new List<SellSodas>();
    public static ChooseSellSoda example = null;

    public Sprite Upgrade, Midgrade, Downgrade;

    //GET THIS SHIT OUTTA HERE! THis is getting replaced by SellUptrade.cs instances attached to the actual object
    /*
    public static Upgrade[] upgrades = {
        new Upgrade("Foldable Table","Cheap Plastic foldable table you can put stuff on",1.5f,2f),
        new Upgrade("Cardboard Sign","You'll look homeless with this, but it helps",1.5f,1f),
        new Upgrade("Bristol Board Sign","Cheap quick sign to help get people's attention",1.2f,1f),
        new Upgrade("Letter-Block Sign","A professional looking sign that attracts customers",2f,0.95f),
    };
    */

    private void Start()
    {
        if(example == null) example = this;
    }

    void NewCell(int ItemID)
    {
        if (ItemsForSale.Contains(ItemID))
        {
            print("SOMETHING WENT VERY WRONG. Tried to create a cell for a soda thats already displayed");
        }
        else
        {
            GameObject temp = Instantiate(ItemPrefab, Content);
            temp.SetActive(true);
            Transform obj = temp.transform.GetChild(0);
            obj.GetComponent<Image>().sprite = Items.ITEMS_DB[ItemID].icon;
            obj.GetComponent<Tooltip>().tooltip = Items.ITEMS_DB[ItemID].Name;
            obj.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + Items.ITEMQUANTITY[Items.IndexOfXinY(ItemID, Items.ITEMS)];

            SellSodas S = obj.GetComponent<SellSodas>();
            S.ID = ItemID;
            S.List = this;
            S.SodaInfo = Items.SodaInfo[Items.IndexOfXinY(ItemID, Items.Sodas)];
            ItemsForSale.Add(ItemID);
            BeingSold.Add(S);
            S.StartOverride();
        }
    }

    void DeleteCell(GameObject obj,int ind)
    {
        ItemsForSale.RemoveAt(ind);
        BeingSold.RemoveAt(ind);
        Destroy(obj);
    }

    SellSodas GetThis(int ItemID)
    {
        foreach (SellSodas s in BeingSold)
        {
            if (s.ID == ItemID)
            {
                return (s);
            }
        }
        print("SOMETHINGS GONE TERRIBLY WRONG. NO SELLSODA WITH THIS ID EXISTS");
        return (null);
    }

    public void InitializeShop()
    {
        ItemsForSale.Clear();
        BeingSold.Clear();

        for(int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(0).gameObject);
        }
        bool showEmptyNotif = true;
        for (int i = 0; i < NewInv.Frames; i++)
        {
            for(int j = 0; j < Items.Sodas.Length; j++)
            {
                if (Items.ITEMS[i] == Items.Sodas[j])
                {
                    NewCell(Items.ITEMS[i]);
                    showEmptyNotif = false;
                }
            }
        }
        EmptyNotif.SetActive(showEmptyNotif);

        //Amount of cells that can fit into 1 frame (should be 6 if noone changed anything)
        Ratio = HorizScroll.transform.parent.GetComponent<RectTransform>().sizeDelta.x / ItemPrefab.GetComponent<RectTransform>().sizeDelta.x;
        HorizScroll.size = Ratio / (ItemsForSale.Count > Ratio ? ItemsForSale.Count : 1);
        //print(" did " + (ItemsForSale.Length - 2f + (Ratio / 2f)) );
    }

    public void UpdateNumbers()
    {
        bool showEmptyNotif = true;
        for (int i = 0; i < BeingSold.Count; i++)
        {
            print(Items.Contains(BeingSold[i].ID) + BeingSold[i].TimeLeft.ToString());
            if ( Items.Contains(BeingSold[i].ID) || BeingSold[i].TimeLeft != 0)
            {
                if(BeingSold[i].TimeLeft != 0)
                {
                    BeingSold[i].text.text = (Items.Contains(BeingSold[i].ID)) ? "" + (Items.ITEMQUANTITY[Items.IndexOfXinY(ItemsForSale[i], Items.ITEMS)] + 1) : "1";
                }
                else
                {
                    BeingSold[i].text.text = "" + Items.ITEMQUANTITY[Items.IndexOfXinY(ItemsForSale[i], Items.ITEMS)];
                }
                

                showEmptyNotif = false;
            }
            else
            {
                NameIndic.Indicate("");
                Time.text = "Sell Time (mins):";
                Money.text = "Sell Price:";
                DeleteCell(BeingSold[i].transform.parent.gameObject, i);
                i -= 1;
            }
        }
        EmptyNotif.SetActive(showEmptyNotif);
    }

    void AddMissingItems()
    {
        List<int> Temp = new List<int>();
        for (int i = 0; i < NewInv.Frames; i++)
        {
            for (int j = 0; j < Items.Sodas.Length; j++)
            {
                if (Items.ITEMS[i] == Items.Sodas[j]) //if item is a soda
                {
                    if (!ItemsForSale.Contains(Items.ITEMS[i])) //if item is not included in the itemsforsale list
                    {
                        Temp.Add(Items.ITEMS[i]);
                    }
                }
            }
        }

        for(int i = 0; i < Temp.Count; i++)
        {
            NewCell(Temp[i]);
        }
    }

    public void MoveScroll()
    {
        float move = -((1 / (HorizScroll.size)) * HorizScroll.transform.parent.GetComponent<RectTransform>().sizeDelta.x * HorizScroll.value);
        move *= (float)(ItemsForSale.Count - Ratio) / (float)ItemsForSale.Count;

        ItemPrefab.transform.parent.GetComponent<RectTransform>().localPosition = new Vector2(move, 0);
    }

    public override void Use(float Amount)
    {
        if(ItemsForSale.Count == 0)
        {
            InitializeShop();
        }
        else
        {
            AddMissingItems();
            UpdateNumbers();
        }
        
        
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        bool On = (Menu.transform.localPosition.y > -100);
        Menu.transform.localPosition = On ? new Vector3(0, -999999, 0) : new Vector3(100,0,0);
        Stats.StartStopPlayerMovement(On,"ChooseSellSoda");

        if (On)
        {
            //Stats.StopStartTime(true);
            SellSodas.SLevel = float.NaN;
            Stats.StartStopTime(true, "ChooseSellSoda");
            
        }
        else
        {
            SellSodas.SLevel = SLevel;
            //Stats.StopStartTime(false);
            loadStats();
            Stats.StartStopTime(false, "ChooseSellSoda");
        }
        
    }

    float lastM = 0;
    float lastS = 0;
    void NewStat(int itemID, float M, float S, bool isHeader = false)
    {
        GameObject temp = Instantiate(StatPrefab, StatsContent);
        temp.SetActive(true);

        Transform header = temp.transform.Find("Header");
        header.Find("Icon").GetComponent<Image>().sprite = isHeader ? Midgrade : Items.ITEMS_DB[itemID].icon;
        header.Find("Text").GetComponent<TextMeshProUGUI>().text = isHeader ? "Sell Spot Base Stats" : Items.ITEMS_DB[itemID].Name;

        Transform sellTime = temp.transform.Find("Sell Time").GetChild(0);
        sellTime.Find("Status").GetComponent<Image>().sprite = isHeader ? null : (M < lastM ? Downgrade : (M > lastM ? Upgrade : Midgrade));
        sellTime.Find("Text").GetComponent<TextMeshProUGUI>().text = (int)(M * 100) + "%";

        Transform stealChance = temp.transform.Find("Steal Chance").GetChild(0);
        stealChance.Find("Status").GetComponent<Image>().sprite = isHeader ? null : (S < lastS ? Downgrade : (S > lastS ? Upgrade : Midgrade));
        stealChance.Find("Text").GetComponent<TextMeshProUGUI>().text = (int)(S * 100) + "%";

        lastM = M;
        lastS = S;
    }

    public void loadStats()
    {
        for(int i = StatsContent.childCount-1; i >= 0; i-=1)
        {
            Destroy(StatsContent.GetChild(i).gameObject);
        }

        int activeCount = 0;
        TimeMult = MLevel;
        SMult = SLevel;
        NewStat(0, TimeMult, SMult,true);
        foreach (SellUpgrade u in SellUpgrade.getAllActive())
        {
            activeCount++;
            TimeMult *= u.Mmod;
            SMult *= u.Smod;
            NewStat(u.ItemID,TimeMult,SMult);
        }

        int outfitID = outfit.getActiveOutfit();
        Item oFit = Items.ITEMS_DB[outfitID];
        TimeMult *= ((float)oFit.MLevel)/100f;
        SMult *= ((float)oFit.SodaPChange) / 100f;
        NewStat(outfitID, TimeMult, SMult);


        StatsEmptyNotif.SetActive(activeCount <= 0);

        StatFinalResult.text = "Sell Time (mins): " + (int)(TimeMult * 100) + "%. Theft Chance: " + (int)(SMult * 100) + "%";


    }
}

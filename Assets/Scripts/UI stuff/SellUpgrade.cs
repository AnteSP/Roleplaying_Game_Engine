using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SellUpgrade : MonoBehaviour
{

    static List<SellUpgrade> allInstances = new List<SellUpgrade>();
    static List<SellUpgrade> templateInstances = new List<SellUpgrade>();

    public static void FlushUpgrades()
    {
        print("UPGRADES FLUSHED");
        allInstances.Clear();
        templateInstances.Clear();
    }

    public static SellUpgrade getUpgrade(int itemID)
    {
        foreach(SellUpgrade upgrade in templateInstances)
        {
            //print("ID " + upgrade.ItemID + "   name " + upgrade.Name);
        }
        return templateInstances.Where(a => a.ItemID == itemID).First();
    }

    public static List<SellUpgrade> getAllActive()
    {
        return templateInstances.Where(a => a.gameObject.activeSelf).ToList();
    }

    public static bool isUpgradeOn(string name)
    {
        return allInstances.Where(a => a.Name == name).First().gameObject.activeSelf;
    }

    public static void addToAllInstances(SellUpgrade[] upgs)
    {
        //print("==ADDING TO ALL==" + upgs.Length);
        //printAllInstances();
        foreach (SellUpgrade upgrade in upgs) allInstances.Add(upgrade);
        //printAllInstances();
        //print("==DONE ADDING TO ALL==" + upgs.Length);
    }

    public static void addToAllInstances(SellUpgrade upg)
    {
        if (upg == null) return;
        //print("==ADDING TO ALL==" + upgs.Length);
        //printAllInstances();
        allInstances.Add(upg);
        //printAllInstances();
        //print("==DONE ADDING TO ALL==" + upgs.Length);
    }

    public static void printAllInstances()
    {
        foreach (SellUpgrade upgrade in allInstances) print(upgrade.Name + " " + upgrade.gameObject.name);
    }

    static public void enableUpgrade(string s)
    {
        print("ENABLIND UPGRADE " + s);
        printAllInstances();
        foreach (SellUpgrade u in allInstances.Where(a => a.Name == s))
        {
            print("WOW: " + u.gameObject.name);
            u.gameObject.SetActive(true);
            if (allInstances.Exists(a => a.replacedBy != null && a.replacedBy.Name == s))
            {
                foreach(SellUpgrade uu in allInstances.Where(a => a.replacedBy != null && a.replacedBy.Name == s))
                {
                    uu.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }

        specialFunc(s);
        Progress.turnOnUpgrade(s);
    }

    static public void enableUpgrade(int itemID)
    {
        enableUpgrade(Items.ITEMS_DB[itemID].Name );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns>Null if fully upgraded. Otherwise, returns highest upgrade in upgrade chain</returns>
    static public SellUpgrade getHighestUpgradeInChain(int itemID)
    {
        if (allInstances.Count() <= 0) return null;
        SellUpgrade upg = allInstances.Where(a => a.ItemID == itemID).First();

        if (upg.replacedBy == null) return (upg.gameObject.activeSelf ? null : upg);

        while (upg.gameObject.activeSelf)
        {
            upg = upg.replacedBy;
            if (upg == null) return null;
        }

        return upg;
    }

    // Start is called before the first frame update
    void Start()
    {
        //print("STARTING " + this.Name);
        foreach (SellUpgrade upgrade in allInstances)
        {
            //print("ID " + upgrade.ItemID + "   name " + upgrade.Name);
        }
        if (!templateInstances.Exists(a => a.Name == this.Name))
        {
            templateInstances.Add(this);
            //print("ADDED TO TEMPLATEINSTANCES");
        }
        allInstances.Add(this);
        
        if (Items.ITEMS_DB[ItemID].Name != Name) throw new System.Exception("EY! DICKHEAD! MAKE THE FUCKIN " + Name + " UPGRADE HAVE THE SAME NAME AS THE FUCKIN ITEM ID");

        if (!Progress.isUpgradeOn(Name))
        {
            gameObject.SetActive(false);
        }
    }

    static void specialFunc(string Name)
    {
        switch (Name)
        {
            case "Professional Sign":
                //destroy cardboard sign
                return;

        }
    }

    public string Name; //must be an existing item name
    public int ItemID;
    public string Description;
    public float Mmod;
    /// <summary>
    /// higher = more stealing
    /// </summary>
    public float Smod;
    public SellUpgrade replacedBy = null;

    public SellUpgrade(string Name, string Description, float MLevel, float SLevel)
    {
        this.Name = Name; this.Description = Description; this.Mmod = MLevel; this.Smod = SLevel;
    }
}

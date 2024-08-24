using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class Progress : MonoBehaviour
{
    //ALL INSTANCES OF THIS CLASS NEED TO BE ON THE SAME OBJECT

    public string Id;
    public bool on = false;

    public List<GameObject> enable = new List<GameObject>();
    public List<GameObject> disable = new List<GameObject>();

    static bool loadedItems = false;

    class Node
    {
        public Node(string id,bool on)
        {
            this.Id = id;
            this.On = on;
        }

        public string Id;
        public bool On = false;
    }

    static Progress[] progressComps = null;
    static JObject data = null;
    static string SName = "U";
    static bool loaded = false;

    // Start is called before the first frame update
    void Start()
    {
        SName = Application.dataPath + "/saveData.kurger";
        progressComps = GameObject.FindGameObjectWithTag("STATS").GetComponents<Progress>();
        //readData();
    }

    public static void markDataAsUnloaded() { loaded = false; progressComps = null; loadedItems = false; }

    public static bool wasDataLoaded() => loaded;

    public static void deleteData()
    {
        if (SName == "U") SName = Application.dataPath + "/saveData.kurger";
        if (!File.Exists(SName)) return;

        File.Delete(SName);
        readData();
        MainMenu.ForceStart();
    }

    public static void readData()
    {
        print("READING DATA");
        if (SName == "U") SName = Application.dataPath + "/saveData.kurger";
        if (!File.Exists(SName)) File.WriteAllText(SName, "{}");

        string content = File.ReadAllText(SName);

        try
        {
            data = JObject.Parse(content);
            if (!data.ContainsKey("FUN")) data.Add("FUN", (int)Random.Range(1, 100));
            //if (!data.ContainsKey("FUN")) data.Add("FUN", 14);
            print(data.ToString(Newtonsoft.Json.Formatting.Indented,null));
        }
        catch
        {
            Stats.DisplayMessage("ERROR: You fucked with the save file didn't you? It's ok, I won't tell Croggs. You should probably fix it though");
            return;
        }
    }

    public static bool doesPickUpExist(string name)
    {
        //if (data == null) readData();
        if (data == null) loadData();
        if (data == null)
        {
            print("HOW TF IS DATA NULL");
            return false;
        }
        if (!data.ContainsKey("PickUps")) data.Add("PickUps", new JObject());
        return ((JObject)data["PickUps"]).ContainsKey(name);
    }
    /// <summary>
    /// Returns true if succesful, false if failed (the pickup already exists)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool savePickUp(string name)
    {
        if (doesPickUpExist(name)) return false;

        ((JObject)data["PickUps"]).Add(name,"");

        return true;
    }

    static Progress[] getProgress() => GameObject.FindGameObjectWithTag("STATS").GetComponents<Progress>();

    public static void checkProgress()
    {
        foreach(Progress p in progressComps)
        {
            foreach(GameObject g in p.enable) g.SetActive(p.on);
            foreach (GameObject g in p.disable) g.SetActive(!p.on);
        }
    }

    static public void setFloat(string Id,float num)
    {
        if (data.ContainsKey(Id))
            data[Id] = num;
        else
        {
            print("ERROR: While setting float. Missing field [" + Id + "] in save data. The field will be created");
            data.Add(Id, num);
        }
    }

    static public void setInt(string Id, int num)
    {
        print("Setting int " + Id + " contains: " + data);
        if (data.ContainsKey(Id))
            data[Id] = num;
        else
        {
            print("ERROR: While setting int. Missing field [" + Id + "] in save data. The field will be created");
            data.Add(Id, num);
        }
    }

    static public int getInt(string Id)
    {
        if (data.ContainsKey(Id))
            return( data[Id].Value<int>());
        else
        {
            print("ERROR: While getting Int. Missing field [" + Id + "] in save data");
            return 0;
        }
    }

    static public float getFloat(string Id)
    {
        if (data.ContainsKey(Id))
            return (data[Id].Value<float>() );
        else
        {
            print("ERROR: While getting Float. Missing field [" + Id + "] in save data");
            return float.NaN;
        }
    }

    static public bool getBool(string Id)
    {
        if (data.ContainsKey(Id))
            return (data[Id].Value<bool>());
        else
        {
            print("ERROR: While getting Bool. Missing field [" + Id + "] in save data");
            return false;
        }
    }

    static public void switchInPlay(string switchID,bool on)
    {
        try
        {
            if (data.ContainsKey(switchID))
            {
                foreach (Progress p in progressComps.Where(a => a.Id == switchID))
                {
                    data[p.Id] = on;
                    p.on = on;
                    foreach (GameObject g in p.enable) g.SetActive(p.on);
                    foreach (GameObject g in p.disable) g.SetActive(!p.on);
                }
            }
            else
            {
                print("ERROR: Switch [" + switchID + "] does not exist. Adding Now");
                data.Add(switchID, on);
                foreach (Progress p in progressComps.Where(a => a.Id == switchID))
                {
                    p.on = on;
                    foreach (GameObject g in p.enable) g.SetActive(p.on);
                    foreach (GameObject g in p.disable) g.SetActive(!p.on);
                }
            }
        }
        catch
        {
            print("ERROR: Switch [" + switchID + "] IDK WHAT HAPPENED. Adding Now");
            data.Add(switchID, on);
        }
    }

    static void switchInData(string switchID,bool on)
    {
        if (data.ContainsKey(switchID))
            data[switchID] = on;
        else
        {
            print("ERROR: While flipping switch. Missing field [" + switchID + "] in save data. The field will be created");
            data.Add(switchID, on);
        }
    }

    public static void saveData()
    {
        if(Stats.current != null) setInt("MONEY", Stats.current.Money);

        if(progressComps != null)
        foreach (Progress p in progressComps)
        {
            switchInData(p.Id, p.on);
        }

        print("SAVING DATA");

        if (!data.ContainsKey("Items")) data.Add("Items", new JObject());
        data["Items"] = new JObject();
        if(Items.ITEMS != null)
        for (int i = 0; i < Items.ITEMS.Length; i++)
        {
            if (Items.ITEMS[i] != 0 && Items.ITEMQUANTITY[i] != 0)
            {
                ((JObject)data["Items"]).Add(Items.ITEMS[i] + "", Items.ITEMQUANTITY[i]);
            }
        }

        if (!data.ContainsKey("Recipes")) data.Add("Recipes", new JObject());
        data["Recipes"] = new JObject();

        foreach(Recipe r in SodaMachine.getUnlockedRecipes())
        {
            //print("GOT " + r + " AND " + r.Name);
            ((JObject)data["Recipes"]).Add(r.Name, true);
        }

        if (!data.ContainsKey("Upgrades")) data.Add("Upgrades", new JObject());
        data["Upgrades"] = new JObject();

        foreach (SellUpgrade u in SellUpgrade.getAllActive())
        {
            ((JObject)data["Upgrades"]).Add(u.Name, true);
        }

        File.WriteAllText(SName,data.ToString());
    }

    public static bool isUpgradeOn(string s)
    {
        if (!data.ContainsKey("Upgrades")) data.Add("Upgrades", new JObject());
        //print("CHECKING UPGRADE");
        //print(data["Upgrades"]);

        return ((JObject)data["Upgrades"]).ContainsKey(s);
    }

    public static void turnOnUpgrade(string s)
    {
        if (!data.ContainsKey("Upgrades")) data.Add("Upgrades", new JObject());

        print("BEFORE UPGRADE");
        print(data["Upgrades"]);
        ((JObject)data["Upgrades"]).Add(s, true);
        print(data["Upgrades"]);
    }

    public static void loadData(bool excludeItems = false)
    {
        print("LOADING DATA");
        loaded = true;
        readData();

        if(Stats.current != null)
        if(data.ContainsKey("MONEY"))
            Stats.ChangeMoney(data["MONEY"].Value<int>() - Stats.current.Money);
        else data.Add("MONEY", Stats.current.Money);

        if (progressComps == null) progressComps = GameObject.FindGameObjectWithTag("STATS").GetComponents<Progress>();

        foreach (Progress p in progressComps)
        {
            //print("P = " + p.Id);
            if (data.ContainsKey(p.Id))
                p.on = data[p.Id].Value<bool>();
            else print("ERROR: Missing field [" + p.Id + "] in save data");
        }

        if (!excludeItems)
        {
            if (!loadedItems && data["Items"] != null)
            {
                foreach (var child in data["Items"].Children())
                {
                    // 'child' is of type JProperty, which represents a key-value pair
                    string key = ((JProperty)child).Name;
                    int value = (int)((JProperty)child).Value;

                    // Now you can use 'key' and 'value' as needed
                    Items.AddNoAnim(int.Parse(key), value);
                }
                loadedItems = true;
            }
        }

        checkProgress();
    }

    public static bool checkFUN(int min,int max)
    {
        if (data == null || data["FUN"] == null)
        {
            //print("GOT HERE");
            loadData();
            //print("THEN HERE");
            if (data["FUN"] == null) return false;
        }
        return data["FUN"].Value<int>() <= max && data["FUN"].Value<int>() >= min;
    } 
}

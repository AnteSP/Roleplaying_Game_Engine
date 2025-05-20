using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
//using System;

public class Progress : MonoBehaviour
{
    //ALL INSTANCES OF THIS CLASS NEED TO BE ON THE SAME OBJECT

    public string Id;
    public bool on = false;

    public List<GameObject> enable = new List<GameObject>();
    public List<GameObject> disable = new List<GameObject>();

    static bool loadedItems = false;
    static bool loadedDeadlines = false;
    static bool loadedSellSpots = false;

    static List<Progress> progressComps = new List<Progress>();
    static JObject data = null;
    static string SName = "U";
    static bool loaded = false;

    static string chPattern = @"^Ch\d+[a-zA-Z]+";
    static string chPatternShort = @"^Ch\d+";

    private void Awake()
    {
        SName = Application.dataPath + "/saveData.kurger";
        progressComps.Add(this);
    }

    public static void DEBUG_printData()
    {
        print("UPDATE TEST:\n"+data);
    }

    public static void markDataAsUnloaded() { loaded = false; progressComps = new List<Progress>(); print("progressComps set to null"); loadedItems = false;loadedDeadlines = false;loadedSellSpots = false; }

    public static bool wasDataLoaded() => loaded;
    public static bool wasItemDataLoaded() => loadedItems;

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
        //print("READING DATA");
        if (SName == "U") SName = Application.dataPath + "/saveData.kurger";
        if (!File.Exists(SName)) File.WriteAllText(SName, "{}");

        string content = File.ReadAllText(SName);

        try
        {
            data = JObject.Parse(content);
            if (!data.ContainsKey("FUN")) data.Add("FUN", (int)Random.Range(1, 100));
            //if (!data.ContainsKey("FUN")) data.Add("FUN", 14);
            //print(data.ToString(Newtonsoft.Json.Formatting.Indented,null));
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

    //static Progress[] getProgress() => GameObject.FindGameObjectWithTag("STATS").GetComponents<Progress>();

    static Dictionary<GameObject, bool> gameObjsToSet = new Dictionary<GameObject, bool>();
    public static void checkProgress()
    {
        gameObjsToSet.Clear();
        //ensureProgressCompsGood();
        foreach (Progress p in progressComps.Where(a=> a.on))
        {
            foreach(GameObject g in p.enable) gameObjsToSet[g] = true;
            foreach (GameObject g in p.disable) gameObjsToSet[g] = false;
        }

        foreach (KeyValuePair<GameObject, bool> kvp in gameObjsToSet)
        {
            //print("DOING " + kvp.Key.name);
            kvp.Key.SetActive(kvp.Value);
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
        print("Setting int " + Id + " to " + num + " contains: " + data);
        if (data.ContainsKey(Id))
            data[Id] = num;
        else
        {
            print("ERROR: While setting int. Missing field [" + Id + "] in save data. The field will be created");
            data.Add(Id, num);
        }
    }

    static public bool doesFieldExist(string Id) => data.ContainsKey(Id);

    static public bool isFieldNumber(string Id) => data.ContainsKey(Id) && data[Id].Type != JTokenType.Boolean;

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

    static public bool getBool(string switchID)
    {
        if (Regex.IsMatch(switchID, chPattern))
        {
            Match match = Regex.Match(switchID, chPatternShort);
            string subName = switchID.Substring(match.Length);
            string chString = switchID.Substring(0, match.Length);

            if (!data.ContainsKey(chString)) data[chString] = new JObject();

            if ( ((JObject)data[chString]).ContainsKey(subName) )
                return (data[chString][subName].Value<bool>());
            else
            {
                print("ERROR: While getting Bool. Missing field [" + switchID + "] in save data");
                return false;
            }
        }

        if (data.ContainsKey(switchID))
            return (data[switchID].Value<bool>());
        else
        {
            print("ERROR: While getting Bool. Missing field [" + switchID + "] in save data");
            return false;
        }
    }

    static void ensureProgressCompsGood()//SHOULDNT NEED THIS
    {
        if (progressComps == null)
        {
            //progressComps = GameObject.FindGameObjectWithTag("STATS").GetComponents<Progress>();
            print("RESET progressComps " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    static public void switchInPlay(string switchID,bool on)
    {
        print("Setting int " + switchID + " to " + on + " contains: " + data);
        try
        {
           // ensureProgressCompsGood();
            switchInData(switchID, on);

            foreach (Progress p in progressComps.Where(a => a.Id == switchID))
            {
                p.on = on;
                foreach (GameObject g in p.enable) g.SetActive(p.on);
                foreach (GameObject g in p.disable) g.SetActive(!p.on);
            }
        }
        catch(System.Exception e)
        {
            print("ERROR: Switch [" + switchID + "] IDK WHAT HAPPENED (" + e+")(" + e.StackTrace +")");
            //data.Add(switchID, on);
        }
    }

    static void switchInData(string switchID,bool on)
    {
        if (Regex.IsMatch(switchID, chPattern))
        {
            Match match = Regex.Match(switchID, chPatternShort);
            string subName = switchID.Substring(match.Length);
            string chString = switchID.Substring(0, match.Length);

            if (!data.ContainsKey(chString)) data[chString] = new JObject();

            if ( ( (JObject)data[chString] ).ContainsKey(subName) )
                data[chString][subName] = on;
            else
            {
                print("ERROR: While flipping switch. Missing field [" + switchID + "] in save data. The field will be created");
                ((JObject)data[chString]).Add(subName, on);
            }
        }
        else
        {
            if (data.ContainsKey(switchID))
                data[switchID] = on;
            else
            {
                print("ERROR: While flipping switch. Missing field [" + switchID + "] in save data. The field will be created");
                data.Add(switchID, on);
            }
        }
    }

    public static void saveData(string toFile = "")
    {
        if (Stats.current != null)
        {
            setInt("MONEY", Stats.current.Money);
            setInt("TIME", (int)Stats.current.getTime());
        }

        if(progressComps != null)
        foreach (Progress p in progressComps)
        {
            switchInData(p.Id, p.on);
        }

        print("SAVING DATA");

        if (!data.ContainsKey("Items")) data.Add("Items", new JObject());
        if(Items.ITEMS != null)
        {
            data["Items"] = new JObject();
            for (int i = 0; i < Items.ITEMS.Length; i++)
            {
                if (Items.ITEMS[i] != 0 && Items.ITEMQUANTITY[i] != 0)
                {
                    ((JObject)data["Items"]).Add(Items.ITEMS[i] + "", Items.ITEMQUANTITY[i]);
                }
            }
        }


        if (!data.ContainsKey("Recipes")) data.Add("Recipes", new JObject());
        //data["Recipes"] = new JArray();
        foreach(int i in SodaMachine.getUnlockedRecipes())
        {
            if(!data["Recipes"].Contains(i)) ((JArray)data["Recipes"]).Add(i);
            //print("GOT " + r + " AND " + r.Name);
        }

        if (!data.ContainsKey("Upgrades")) data.Add("Upgrades", new JObject());
        //data["Upgrades"] = new JObject();

        foreach (SellUpgrade u in SellUpgrade.getAllActive())
        {
            if(!data["Upgrades"].Contains(u.Name))
                ((JObject)data["Upgrades"]).Add(u.Name, true);
        }

        if (!data.ContainsKey("SellSpots")) data.Add("SellSpots", new JObject());
        //data["SellSpots"] = new JObject();

        foreach (string s in SellSpot.sellSpots)
        {
            if (!data["SellSpots"].Contains(s))
                ((JObject)data["SellSpots"]).Add(s, true);
        }

        string chapterString = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Split('-')[0];

        if (!data.ContainsKey("Deadlines")) data.Add("Deadlines", new JObject());
        data["Deadlines"] = new JObject();

        
        Deadline[] deds = Stats.current?.GetComponents<Deadline>();
        if(deds != null)
        {
            foreach (Deadline d in deds.Where(a => a.enabled))
            {
                ((JObject)data["Deadlines"]).Add(d.ID + "-" + chapterString, true);
            }
        }

        File.WriteAllText(SName, data.ToString());
        if(toFile != "") File.WriteAllText(Application.dataPath + "/" + toFile + ".kurger", data.ToString());
    }

    public static void saveOnlyThis(int ia,int ib,string s,int type)
    {
        readData();

        if(type == 0)
        {
            ((JObject)data["Items"]).Add(Items.ITEMS_DB[ia].ItemID + "", ib);
            switchInData(s, true);
            print(data);
        }


        File.WriteAllText(SName, data.ToString());
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
        print(excludeItems ? "LOADING JUST DATA" : "LOADING ITEMS + DATA");
        loaded = true;
        readData();

        if(Stats.current != null)
        {
            if (data.ContainsKey("MONEY"))
                Stats.ChangeMoney(data["MONEY"].Value<int>() - Stats.current.Money);
            else data.Add("MONEY", Stats.current.Money);

            if(data.ContainsKey("TIME"))
                Stats.ChangeTime(data["TIME"].Value<uint>() - Stats.current.getTime());
            else data.Add("TIME", (int)Stats.current.getTime());
        }

        //ensureProgressCompsGood();

       // print("PROGRESSCOMPS.Count = " + progressComps.Count);
        foreach (Progress p in progressComps)
        {
            //print("P = " + p.Id + " b = " + data[p.Id].Value<bool>());
            p.on = getBool(p.Id);
            //if (data.ContainsKey(p.Id))
            //    p.on = data[p.Id].Value<bool>();
            //else print("ERROR: Missing field [" + p.Id + "] in save data");
        }

        if (data["Recipes"] != null)
        {
            foreach(int i in (JArray)data["Recipes"])
            {
                SodaMachine.UnlockRecipe(i);
            }
        }

        //print("LOADING DEDS");
        if (!loadedDeadlines && data["Deadlines"] != null)
        {
            List<char> deadlines = new List<char>();

            foreach (var child in data["Deadlines"].Children())
            {
                // 'child' is of type JProperty, which represents a key-value pair
                string key = ((JProperty)child).Name;
                string sceneStr = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                Deadline[] scenesDeds = Stats.current.GetComponents<Deadline>();

                if ( scenesDeds.Length > 0 && (sceneStr.StartsWith(key.Remove(0, 2) + "-") || sceneStr == key.Remove(0, 2)) )
                {
                    scenesDeds.Where(a => a.ID == key.ToCharArray()[0]).First().enabled = true;
                }
            }
            loadedDeadlines = true;
        }
        //print("DONE LOADING DEDS");

        if (!loadedSellSpots && data["SellSpots"] != null)
        {
            List<SellSpot> sellSpots = SellSpot.possibleSellSpots;
            foreach (JProperty child in data["SellSpots"].Children().Where(s => ((JProperty)s).Name.StartsWith(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name) ))
            {
                SellSpot curSellSpot = sellSpots.Where(s => child.Name.EndsWith(s.gameObject.name)).Last();

                curSellSpot.setAsMarket();
            }
            loadedSellSpots = true;
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

    public static string getRandomUpgrade()
    {
        if (data["Upgrades"].Count() <= 0) return null;
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, data["Upgrades"].Count() ); // Generate random index

        // Get the key-value pair at the random index
        JProperty randomElement = (JProperty)data["Upgrades"].ElementAt(randomIndex);
        //JToken value = randomElement.Value<string>();

        return randomElement.Name;
    }
}

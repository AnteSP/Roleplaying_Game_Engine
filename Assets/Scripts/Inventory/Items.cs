using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
/*
    public static string[] NAMES = new string[50];
    public static Sprite[] PICS = new Sprite[50];
    public static string[] DESCRIPTS = new string[50];
    public static int[] Price = new int[50];
    public static int[] MLevel = new int[50];

    [SerializeField] string[] NAMESI = new string[50];
    [SerializeField] Sprite[] PICSI = new Sprite[50];
    [SerializeField] string[] DESCRIPTSI = new string[50];
    [SerializeField] int[] PRICE = new int[50];
    [SerializeField] int[] MLEVEL = new int[50];
*/
    [SerializeField] TextMeshProUGUI Title, Descp;
    public static TextMeshProUGUI TITLE, DESCP;

    [SerializeField] int[] items = new int[70];
    [SerializeField] int[] q = new int[70];

    public static int SELECTED;

    public static int[] ITEMS,ITEMQUANTITY;

    public static Image[] Pictures,TxtBack;
    public static Text[] Quantities;

    public static Color TxtBackBase;

    [SerializeField] Animator[] NewItemAnim;
    static Animator[] NewItem;

    static int Curser;
    static float Inity;

    //public static int[] Sodas = { 3 , 7,12,15,34};

    //[SerializeField] float[] SODAPCHANGE = new float[50];
    //[SerializeField] float[] SODATCHANGE = new float[50];

    //public static Soda[] SodaInfo = new Soda[50];

    [SerializeField] SodaMachine SodaMach;
    public static Item[] ITEMS_DB = new Item[1];
    public static RecipeAsset[] RECIPES_DB = new RecipeAsset[1];

    public static void EnsureItemsAreInstantiated()
    {
        if(ITEMS_DB.Length == 1)
        {
            print("THIS IS BAD");
        }
    }

    void StartOverride()
    {
        /*
                NAMES = NAMESI;
                DESCRIPTS = DESCRIPTSI;
                PICS = PICSI;
                Price = PRICE;
                MLevel = MLEVEL;
        */
        TITLE = Title;
        DESCP = Descp;

        NewItem = NewItemAnim;
        Inity = NewItem[0].transform.localPosition.y;

        string[] guids = AssetDatabase.FindAssets("t:Item");
        ITEMS_DB = new Item[guids.Length];

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fName = Path.GetFileNameWithoutExtension(path);
            int index = fName.IndexOf('-');

            int id = int.Parse(fName.Substring(0, index));

            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            item.ItemID = id;

            ITEMS_DB[id] = item;
        }

        guids = AssetDatabase.FindAssets("t:RecipeAsset");
        RECIPES_DB = new RecipeAsset[guids.Length];

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fName = Path.GetFileNameWithoutExtension(path);
            int index = fName.IndexOf('-');

            int id = int.Parse(fName.Substring(1, index-1));

            RecipeAsset recipe = AssetDatabase.LoadAssetAtPath<RecipeAsset>(path);
            recipe.RecipeID = id;

            RECIPES_DB[id] = recipe;
        }

        SodaMach?.StartOverride();
        
        if(!Progress.wasItemDataLoaded())Progress.loadData();
        UpdatePics();
    }


    private void OnEnable()
    {
        StartOverride();
    }

    // Update is called once per frame
    void Update()
    {
        items = ITEMS;
        q = ITEMQUANTITY;
    }

    public static void SELECT(int ID)
    {
        SELECTED = ID;
        //print("NAME: " + ITEMS_DB[ID].Name + " DESC: " + ITEMS_DB[ID].description + " name: " + ITEMS_DB[ID].name);
        TITLE.text = ITEMS_DB[ID].Name;
        DESCP.text = ITEMS_DB[ID].description;
    }
    /// <summary>
    /// negative quantity removes item
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="Quantity"></param>
    /// <returns></returns>
    public static bool Add(int ID,int Quantity)
    {
        if (ITEMS[0] == 0 && Quantity != 0) Stats.EnableInventory();//If there's no items. This is the first item. Enable inventory

        print("Fuckin with Item #" + ID);
        bool outp = AddNoAnim(ID, Quantity);
        if(outp && Quantity > 0) ItemAnim(ID, Quantity);
        return outp;
    }

    public static bool AddNoAnim(int ID, int Quantity)
    {
        if (ID == 0) return true;
        bool Giving = Quantity > 0;

        //Check if player already has item specified
        for (int i = 0; i < ITEMS.Length; i++)
        {
            if (ID == ITEMS[i])
            {
                if (Giving)
                {
                    ITEMQUANTITY[i] += Quantity;
                    UpdatePics();

                    return (true);
                }
                else
                {
                    if (Quantity + ITEMQUANTITY[i] < 0)
                    {
                        UpdatePics();
                        return (false);
                    }

                    ITEMQUANTITY[i] += Quantity;


                    ITEMS[i] = (ID != 0 && ITEMQUANTITY[i] == 0) ? 0 : ITEMS[i];
                    UpdatePics();
                    return (true);

                }
            }
        }

        if (!Giving) return (false);

        //Check if there's an open space to put the item
        for (int i = 0; i < ITEMS.Length; i++)
        {
            if (0 == ITEMS[i])
            {
                ITEMS[i] = ID;
                ITEMQUANTITY[i] = Quantity;
                UpdatePics();

                return (true);
            }
        }

        //This only happens if player got a new item and has no room for it in the inventory
        UpdatePics();
        return (false);
    }

    public static void UpdatePics()
    {
        for (int i = 0; i < ITEMS.Length; i++)
        {
            //print("is this out of bounds? " + ITEMS[i] + " at i " + i);
            bool temp = ITEMQUANTITY[i] == 0 || ITEMS[i] == 0;

            Pictures[i].sprite = temp ? ITEMS_DB[0].icon : ITEMS_DB[ITEMS[i]].icon;
            Quantities[i].text = temp ? "" : ITEMQUANTITY[i] + "";
            TxtBack[i].color = temp ? Color.clear : TxtBackBase;
            //print(i + " - " + temp + ITEMQUANTITY[i] + " " + ITEMS[i]);
        }
        if(SodaMachine.SodaMenu != null)SodaMachine.UpdateRecipeAvailability();

    }

    static void ItemAnim(int ItemID,int Amount)
    {
        Item item = ITEMS_DB[ItemID];
        ShiftAnim(item.icon, item.Name + " x" + Amount, item.description);
    }

    static public void ShiftAnim(Sprite s,string name,string description)
    {
        Curser = Curser == NewItem.Length - 1 ? 0 : Curser + 1;

        NewItem[Curser].SetTrigger("Go");

        Transform T = NewItem[Curser].transform;
        float H = T.GetComponent<RectTransform>().sizeDelta.y;

        for (int i = 0; i < NewItem.Length; i++)
        {
            NewItem[i].transform.localPosition = i == Curser ? new Vector3(NewItem[i].transform.localPosition.x, Inity, NewItem[i].transform.localPosition.z) : NewItem[i].transform.localPosition + new Vector3(0, H, 0);
        }

        T.Find("Image").GetComponent<Image>().sprite = s;
        T.Find("Title").GetComponent<TextMeshProUGUI>().text = name;
        T.Find("Text").GetComponent<Text>().text = description;
    }

    public static int IndexOfXinY(int X,int[] Y)
    {
        for(int i = 0; i < Y.Length; i++)
        {
            if(Y[i] == X)
            {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// Check if the player has an item of this ID
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static bool Contains(int a)
    {
        foreach(int b in ITEMS)
            if(b == a)  return true;
        return false;
    }

    public static bool Contains(int Item,int Amount)
    {
        for(int i = 0; i < ITEMS.Length; i++)
            if (ITEMS[i] == Item && ITEMQUANTITY[i] >= Amount) return true;
        return false;
    }
}
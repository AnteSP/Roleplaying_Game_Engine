using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SodaMachine : MonoBehaviour
{
    Animator A;
    [SerializeField] GameObject Menu;
    static public GameObject SodaMenu;

    [SerializeField] GameObject RecipePrefab;
    static GameObject RePrefab;

    [SerializeField] Image SodaPreview;
    static Image SodaPrev;
    static TextMeshProUGUI SodaPr;

    [SerializeField] bool Open;

    public static Item[] Ings = new Item[0];
    //public static List<RecipeAsset> Recipes = new List<RecipeAsset>();
    static List<int> RecipesU = new List<int>();

    //static public bool DoingBottle = false;
    static public int ActiveSoda = -1;

    [SerializeField] Button SellButton;
    static Button SellBut;

    static Transform GridParent;
    bool started = false;
    static bool staticStarted = false;

    static SodaMachine activeSM = null;

    private void Start()
    {
        StartOverride();
    }

    public static void UnlockRecipe(int recipeID)
    {
        if (!RecipesU.Contains(recipeID)) RecipesU.Add(recipeID);
    }

    public static List<int> getUnlockedRecipes()
    {
        return RecipesU;
    }

    public static void resetStarted() => staticStarted = false;

    public void StartOverride()
    {
        if (started) return;
        if( !(RecipePrefab != null && Menu == null && SodaPreview == null && SellButton == null))//when all this is null, the soda machine is just in the scene to load recipes and nothing else. So skip this stuff
        {
            started = true;
            SodaPrev = SodaPreview;
            SodaPr = SodaPreview.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();

            A = GetComponent<Animator>();
            SodaMenu = Menu;

            SellBut = SellButton;
        }
        GridParent = RecipePrefab.transform.parent;
        RecipePrefab.SetActive(false);
        RePrefab = RecipePrefab;

        if (!staticStarted)
        {
            staticStarted = true;
            if (!RecipesU.Contains(0)) CreateRecipe(Items.RECIPES_DB[0]);
            foreach (int i in RecipesU)
            {
                updateRecipeList(Items.RECIPES_DB[i]);
            }
        }


        

        //CreateRecipe(Recipes[1]);
        //CreateRecipe(Recipes[2]);
    }

    public static void UpdateRecipeAvailability()
    {
        for (int i = 1; i < GridParent.childCount; i++)
        {
            SelectRec Sr = GridParent.GetChild(i).GetComponent<SelectRec>();
            int Q = 1;
            for (int j = 0; j < Sr.transform.GetChild(1).childCount; j++)
            {
                if(j == 0)
                {
                    Transform temp = Sr.transform.GetChild(1).GetChild(j);
                    temp.GetComponent<Image>().color = Items.Contains(Items.RECIPES_DB[Sr.RecipeIndex].Ingredients[j].ItemID) ? Color.white : new Color(1, 1, 1, 0.3f);
                }
                else
                {
                    Q = Items.RECIPES_DB[Sr.RecipeIndex].Ingredients[j] == Items.RECIPES_DB[Sr.RecipeIndex].Ingredients[j - 1] ? Q + 1 : 1;
                    //print("Q: " + Q + "   REC: " + Recipes[Sr.RecipeIndex].Ingredients[j] + "  RECBEF: " + Recipes[Sr.RecipeIndex].Ingredients[j - 1]);
                    Transform temp = Sr.transform.GetChild(1).GetChild(j);
                    temp.GetComponent<Image>().color = Items.Contains(Items.RECIPES_DB[Sr.RecipeIndex].Ingredients[j].ItemID,Q) ? Color.white : new Color(1, 1, 1, 0.3f);
                }
                
            }
        }
    }

    public static void ToggleMenu()
    {
        activeSM.Open = !activeSM.Open;

        Stats.StartStopPlayerMovement(!activeSM.Open,"SodaMachine");

        SelectRec.Selecting = !SelectRec.Selecting;
        activeSM.A.SetBool("Open", activeSM.Open);
        activeSM.A.SetTrigger("Go");
        Slider.ForceBack();
        NameIndic.Indicate("");

        if (!activeSM.Open)
        {
            activeSM.Open = false;
            activeSM = null;
        }
    }

    public void MakeActiveSM()
    {
        activeSM = this;
    }

    public void ForceMenu(bool state)
    {
        throw new System.Exception("SHOULD NOT HAVE REACHED THIS");
        A.SetBool("Open", state);
        A.SetTrigger("Go");
        //Open = state;

    }

    public static void ChooseRecipe(int Ind)
    {
        if (Ind == -1)
        {
            SodaPrev.sprite = Items.ITEMS_DB[0].icon;
            SodaPrev.GetComponent<Tooltip>().tooltip = "";
            SodaPr.text = "";
            Ings = new Item[0];
            SellBut.GetComponent<Tooltip>().tooltip = "Select recipe first";
            SellBut.interactable = false;
        }
        else
        {
            SodaPrev.sprite = Items.RECIPES_DB[Ind].Soda.icon;
            SodaPrev.GetComponent<Tooltip>().tooltip = Items.RECIPES_DB[Ind].Soda.Name;
            SodaPr.text = "Base Price: " + Items.RECIPES_DB[Ind].SodaPChange;
            print("Tried to change prev image");

            Ings = Items.RECIPES_DB[Ind].Ingredients;

            SellBut.interactable = true;
            SellBut.GetComponent<Tooltip>().tooltip = "";
        }
        ActiveSoda = Ind;
    }
    /// <summary>
    /// R MUST EXIST WITHIN SodaMachine.Recipes
    /// </summary>
    /// <param name="R"></param>
    public static bool CreateRecipe(RecipeAsset R)
    {
        if (RecipesU.Contains(R.RecipeID))
        {
            //updateRecipeList(R);
            return false;
        }
        RecipesU.Add(R.RecipeID);
        updateRecipeList(R);
        return true;
    }

    static void updateRecipeList(RecipeAsset R)
    {
        print("Recipes updated");
        void SetToItem(Transform G, Sprite S, string T)
        {
            G.GetComponent<Image>().sprite = S;
            G.GetComponent<Tooltip>().tooltip = T;
        }

        GameObject temp = Instantiate(RePrefab, RePrefab.transform.parent);
        temp.GetComponent<SelectRec>().RecipeIndex = R.RecipeID;

        temp.SetActive(true);

        Transform Prev = temp.transform.GetChild(0);
        Transform IngPar = temp.transform.GetChild(1);

        SetToItem(Prev, R.Soda.icon, R.Soda.Name + "\nSellTime: " + R.SodaTChange + "s" + "\nSellPrice: " + R.SodaPChange );
        //print(R.Pic.name + " " + R.Name);

        SetToItem(IngPar.GetChild(0), R.Ingredients[0].icon, R.Ingredients[0].Name);

        for (int i = 1; i < R.Ingredients.Length; i++)
        {
            Instantiate(IngPar.GetChild(0), IngPar);
            SetToItem(IngPar.GetChild(i), R.Ingredients[i].icon, R.Ingredients[i].Name);
        }
        UpdateRecipeAvailability();
    }
    /*
    public static void Subtract(string name)
    {
        int contains = -1;
        for (int i = 0; i < Ings.Count; i++)
        {

            contains = Ings[i].name == name ? i : contains;

        }

        if(contains != -1)
        {
            Ings.RemoveAt(contains);

            Destroy(SodaMenu.transform.Find("Component Parent").Find(name).gameObject);
            UpdateTexts();
        }

        
    }

    public static void SubtractBottle(string name)
    {
        int contains = -1;
        for (int i = 0; i < Ings.Count; i++)
        {

            contains = Ings[i].name == name ? i : contains;

        }

        if (contains != -1)
        {
            Ings.RemoveAt(contains);

            Destroy(SodaMenu.transform.Find("Component Parent").Find(name).gameObject);

            GameObject temp = Instantiate(TempSto, SodaMenu.transform.Find("Component Parent"));
            temp.transform.SetAsFirstSibling();
            temp.name = "Add Bottle";

            SellBut.interactable = false;
            SellBut.GetComponent<Tooltip>().tooltip = "Requires a bottle/can";

            UpdateTexts();
        }

        



    }

    public static void Insert(Ingredient I)
    {
        if (DoingBottle)
        {
            InsertBottle(I);
        }
        else
        {
            bool contains = false;
            for (int i = 0; i < Ings.Count; i++)
            {

                contains = Ings[i].name == I.name ? true : contains;

            }

            

            if (SodaMenu.active && new Ingredient(0).name != I.name && !contains && !IsBottle(I.name))
            {
                Transform Parent = SodaMenu.transform.Find("Component Parent");

                GameObject temp = Instantiate(CPref, Parent);
                temp.name = I.name;
                temp.GetComponent<Image>().sprite = I.Pic;
                temp.GetComponent<Tooltip>().tooltip = I.name;

                Parent.Find("Add Something").SetAsLastSibling();


                Slider.ForceBack();

                Ings.Add(I);
                UpdateTexts();

            }
        }

        
    }

    public static bool IsBottle(string name)
    {

        bool isBottle = false;
        switch (name)
        {
            case "Recycled Bottle":
                isBottle = true;
                break;


        }

        return (isBottle);
    }

    public static void InsertBottle(Ingredient I)
    {
        bool contains = false;
        for (int i = 0; i < Ings.Count; i++)
        {

            contains = Ings[i].name == I.name ? true : contains;

        }

        bool isBottle = false;
        switch (I.name)
        {
            case "Recycled Bottle":
                isBottle = true;
                break;


        }

        if (SodaMenu.active && new Ingredient(0).name != I.name && !contains && IsBottle(I.name))
        {
            Transform Parent = SodaMenu.transform.Find("Component Parent");

            GameObject temp = Instantiate(CPref, Parent);
            temp.name = I.name;
            temp.GetComponent<Image>().sprite = I.Pic;
            temp.GetComponent<Tooltip>().tooltip = I.name;
            temp.transform.SetAsFirstSibling();
            temp.transform.GetChild(0).GetComponent<RemoveSComp>().Bottle = true;

            Destroy(Parent.Find("Add Bottle").gameObject);
            SellBut.interactable = true;
            SellBut.GetComponent<Tooltip>().tooltip = "";


            Slider.ForceBack();

            Ings.Add(I);
            
            UpdateTexts();

        }
    }

    static void UpdateTexts()
    {
        float price = 0, sph = 0, rph = 0, ml = 1;
        for (int i = 0; i < Ings.Count; i++)
        {
            price += Ings[i].price;
            ml += (float)Ings[i].Mlevel / 10;

        }

        sph = ml;
        rph = sph * price;

        Price.text = "[" + price;
        SoldPH.text = sph + "";
        RPH.text = "[" + rph;
        ML.text = "ML: " + ml;

        
        ChangeActionTime[] temp = SellS.transform.parent.GetComponentsInChildren<ChangeActionTime>();
        temp[0].ChangeValues((int)price, SellS.Energy, SellS.Time);
        temp[1].ChangeValues((int)price, SellS.Energy, SellS.Time);

        print("CHANGED MONEY " + (int)(price * 100));
    }
    */

}

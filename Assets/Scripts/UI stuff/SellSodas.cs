using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SellSodas : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler 
{

    public int ID;//itemID
    public RecipeAsset recipe;
    public ChooseSellSoda List;
    public float TimeLeft;
    bool go;
    public Text text;
    Button But;
    public static float SLevel;
    static bool firstSell = false;

    public void StartOverride()
    {
        TimeLeft = 0;

        But = GetComponent<Button>();

        text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        firstSell = Progress.getBool("Ch2FirstSell");
    }

    bool justGotStolenFrom = true;
    void finishSell()
    {
        TimeLeft = 0;
        if (Random.Range(0f, 1f) < SLevel && !( SLevel < 0.5f && justGotStolenFrom ))//if stolen AND (The following is false: Stealing should be rare And we just got stolen from)
        {
            Stats.DisplayMessage("Oh no! The soda you were selling got randomly stolen. ( " + (int)(SLevel*100) + "% chance of happening).\n\nHOW TO STOP THIS: find a spot with a lower thief chance",true);
            justGotStolenFrom = true;
        }
        else
        {
            justGotStolenFrom = false;
            Stats.ChangeMoney((int)recipe.SodaPChange);
            List.CashSound.Play();
        }
        
        
        if (!Items.Contains(ID))
        {
            List.UpdateNumbers();
            //execution stops here if this type of soda has quantity 0
        }
        else
        {
            text.text = Items.ITEMQUANTITY[Items.IndexOfXinY(ID, Items.ITEMS)].ToString();
        }
        But.interactable = true;
    }

    public void SELL()
    {
        if (!Items.Add(ID, -1))
        {
            Stats.DisplayMessage("OWo Croggs made a fucky wucky bwut I dont knwo what happen :(");
        }
        else
        {
            //print("FIRSTSELL: " + firstSell);
            Stats.ChangeTimeAnim(Mathf.CeilToInt(recipe.SodaTChange * (1f / List.TimeMult)) );
            if (!firstSell)
            {
                Progress.switchInPlay("Ch2FirstSell",true);
                firstSell = true;
                Stats.DisplayMessage("WHOA!\n\nCheck the time! Your first soda took a long time to sell. Way too long. Go explore, talk to people, and find upgrades to make sodas sell faster");
            }

            finishSell();
            
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData p)
    {
        List.Time.text = "Sell Time: ";
        List.Money.text = "Sell Price: ";
        go = false;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData p)
    {
        updateText();
        go = true;
    }

    void updateText()
    {
        float total = recipe.SodaTChange * (1f/List.TimeMult);
        
        if(total < 5)
        {
            List.Time.text = "Sell Time: " + total.ToString("F1") + " mins   base(" + recipe.SodaTChange + ")";
        }
        else
        {
            List.Time.text = "Sell Time: " + Stats.allTimeInGameToString((int)total) + "   base(" + Stats.allTimeInGameToString((int)recipe.SodaTChange) + ")";
        }
        

        List.Money.text = "Sell Price: " + recipe.SodaPChange + " p";
    }
}

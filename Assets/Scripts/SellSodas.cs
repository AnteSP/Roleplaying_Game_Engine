using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SellSodas : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler 
{

    public int ID;
    public ChooseSellSoda List;
    public Soda SodaInfo;
    public float TimeLeft;
    bool go;
    public Text text;
    Button But;
    public static float SLevel;

    public void StartOverride()
    {
        TimeLeft = 0;

        But = GetComponent<Button>();

        text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }

    bool justGotStolenFrom = true;
    void finishSell()
    {
        TimeLeft = 0;
        if (Random.Range(0f, 1f) < SLevel && !( SLevel < 0.5f && justGotStolenFrom ))//if stolen AND (The following is false: Stealing should be rare And we just got stolen from)
        {
            Stats.DisplayMessage("Oh no! The soda you were selling got randomly stolen. ( " + (int)(SLevel*100) + "% chance of happening). You can move to a different spot or buy upgrades to decrease these chances",true);
            justGotStolenFrom = true;
        }
        else
        {
            justGotStolenFrom = false;
            Stats.ChangeMoney((int)SodaInfo.PriceChange);
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
            Stats.DisplayMessage("OWo I made a fucky wucky bwut I dont knwo what happen :(");
        }
        else
        {
            Stats.ChangeTimeAnim(Mathf.CeilToInt(SodaInfo.TimeChange * (1f / List.TimeMult)) );
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
        float total = SodaInfo.TimeChange * (1f/List.TimeMult);
        
        if(total < 5)
        {
            List.Time.text = "Sell Time (mins): +" + total.ToString("F1") + "   base(+" + SodaInfo.TimeChange + ")";
        }
        else
        {
            List.Time.text = "Sell Time: +" + Stats.allTimeInGameToString((int)total) + "   base(+" + Stats.allTimeInGameToString((int)SodaInfo.TimeChange) + ")";
        }
        

        List.Money.text = "Sell Price: " + SodaInfo.PriceChange ;
    }
}

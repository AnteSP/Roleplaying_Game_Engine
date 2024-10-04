using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreItem : MonoBehaviour
{

    public int Cost,ItemID,RecipeNum,UpgradeNum;

    public StoreMenu Menu;

    public ChooseSellSoda Cs;

    public void setVars(int Cost, int ItemID, int RecipeNum, ItemType Type)
    {
        this.Cost = Cost;
        this.ItemID = ItemID;
        this.RecipeNum = RecipeNum;
        this.Type = Type;
    }

    public enum ItemType
    {
        Disposable,Recipe,Upgrade,Clothing
    }

    public ItemType Type;

    public void Buy()
    {
        if (Stats.ChangeMoney(Cost))
        {
            Stats.current.KACHING.Play();
            if(Type == ItemType.Disposable)
            {
                Items.Add(ItemID, 1);

            } else if(Type == ItemType.Recipe)
            {
                SodaMachine.CreateRecipe(SodaMachine.Recipes[RecipeNum]);
                Menu.RemoveItem(ItemID);
            }else if(Type == ItemType.Clothing)
            {
                Items.Add(ItemID, 1);
                Menu.RemoveItem(ItemID);
            } else if (Type == ItemType.Upgrade)
            {
                SellUpgrade.enableUpgrade(ItemID);
                SellUpgrade su = SellUpgrade.getUpgrade(ItemID);

                Items.ShiftAnim(ChooseSellSoda.example.Upgrade, "<u>UPGRADE!</u>: " + su.Name, su.Description);
                Menu.UpdateShop();
            }
        }
        else
        {
            Stats.DisplayMessage("Not enough money :(",true);
        }
    }
}

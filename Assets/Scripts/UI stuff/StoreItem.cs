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
        Disposable,Recipe,Upgrade,Clothing,Event
    }

    public ItemType Type;

    public void Buy()
    {

        
        if (Stats.ChangeMoney(Cost))
        {
            Stats.current.KACHING.Play();
            if (Type == ItemType.Disposable)
            {
                if(RecipeNum != 0) Items.Add(ItemID, RecipeNum);
                else Items.Add(ItemID, 1);

            } else if (Type == ItemType.Recipe)
            {
                SodaMachine.CreateRecipe(Items.RECIPES_DB[RecipeNum]);
                Menu.RemoveItem(ItemID);
            } else if (Type == ItemType.Clothing)
            {
                Items.Add(ItemID, 1);
                Menu.RemoveItem(ItemID);

                outfit.checkOutfits();
                if (outfit.getOutfitsOwned().Count == 2)
                {
                    Stats.DisplayMessage("Check your inventory to put on the new clothes you just bought!");
                }
            } else if (Type == ItemType.Upgrade)
            {
                SellUpgrade.enableUpgrade(ItemID);
                SellUpgrade su = SellUpgrade.getUpgrade(ItemID);

                Items.ShiftAnim(ChooseSellSoda.example.Upgrade, "<u>UPGRADE!</u>: " + su.Name, su.Description);
                Menu.UpdateShop();
            }else if (Type == ItemType.Event)
            {
                switch (ItemID)
                {
                    case 16:
                        Stats.ChangeMoney(-Cost);
                        Stats.DisplayMessage("No! You could never buy the product of your sworn enemy");
                        break;
                    case 35:
                        Ch2Events.current.startDrug1();
                        break;
                    case 36:
                        Ch2Events.current.startDrug2();
                        break;
                    case 37:
                        if (Progress.doesFieldExist("ch2HatmanVisited"))
                        {
                            Stats.DisplayMessage("You feel like you've already experienced that in another life... No need to do that again. You're refunded your money");
                            Stats.ChangeMoney(-Cost);
                            return;
                        }
                        else
                            Ch2Events.current.startDrug3();
                        break;
                }
            }

            if (Menu.CloseOnPurchase)
            {
                Menu.Use(0);
                if(Menu.SwitchOnPurchase != null)
                {
                    Menu.swapWithThisOnPurchase.SetActive(true);
                    Menu.gameObject.SetActive(false);
                }
            }
            if (Menu.SwitchOnPurchase != "") Progress.switchInPlay(Menu.SwitchOnPurchase,true);
        }
        else
        {
            Stats.DisplayMessage("Not enough money :(",true);
        }

        if (Menu.inInventory != null)
        {
            int indInInv = Items.IndexOfXinY(ItemID, Items.ITEMS);
            if(indInInv != -1)
            {
                Menu.inInventory.text = Items.ITEMS_DB[ItemID].Name + " in inventory: " + Items.ITEMQUANTITY[indInInv];
            }
            else
            {
                Menu.inInventory.text = "";
            }
            
        }
    }
}

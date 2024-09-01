using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

[Serializable]
public class ItemForSale
{
    public int ID;
    public int RecipeNum;
    public StoreItem.ItemType ItemType;
}

public class StoreMenu : Resource
{

    [SerializeField] GameObject Menu;

    //public List<int> ItemsForSale = new List<int>();
    //public List<StoreItem.ItemType> ItemTypes = new List<StoreItem.ItemType>();
    //public List<int> RecipeNums = new List<int>();

    [SerializeField] GameObject ItemPrefab;

    [SerializeField] Scrollbar HorizScroll;

    float Ratio;

    public List<ItemForSale> ItemsForSale = new List<ItemForSale>();

    public void setItem(GameObject temp, int DisplayItemID, int recipeNum, StoreItem.ItemType type, Transform obj = null, StoreItem SMen = null)
    {
        if(obj == null)obj = temp.transform.GetChild(0);
        if (SMen == null) SMen = obj.GetComponent<StoreItem>();

        Item item = Items.ITEMS_DB[DisplayItemID];
        SMen.setVars(-item.price, DisplayItemID, recipeNum, type);

        obj.GetComponent<Image>().sprite = item.icon;
        if(type == StoreItem.ItemType.Upgrade)
        {
            SellUpgrade upg = SellUpgrade.getUpgrade(DisplayItemID);
            obj.GetComponent<Tooltip>().tooltip = upg.Name + "\n(" + upg.Description + ")";
            
        }
        else
            obj.GetComponent<Tooltip>().tooltip = item.Name + "\n(" + item.description + ")";

        temp.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = -SMen.Cost + "¢";
    }

    public void UpdateShop()
    {
        NameIndic.Indicate("");
        for (int i = 0; i < ItemPrefab.transform.parent.childCount - 1; i++)
        {
            Destroy(ItemPrefab.transform.parent.GetChild(i + 1).gameObject);
        }

        foreach(ItemForSale i in ItemsForSale)
        {
            GameObject temp = Instantiate(ItemPrefab, ItemPrefab.transform.parent);
            temp.SetActive(true);

            Transform obj = temp.transform.GetChild(0);

            StoreItem SMen = obj.GetComponent<StoreItem>();

            SMen.Menu = this;

            if (i.ItemType == StoreItem.ItemType.Upgrade)
            {
                SellUpgrade upgr = SellUpgrade.getHighestUpgradeInChain(i.ID);

                if (upgr == null) Destroy(temp);
                else setItem(temp, upgr.ItemID, 0, StoreItem.ItemType.Upgrade, obj, SMen);
            }
            else
            {
                setItem(temp, i.ID, i.RecipeNum, i.ItemType, obj, SMen);
            }
        }

        //Amount of cells that can fit into 1 frame (should be 6 if noone changed anything)
        Ratio = HorizScroll.transform.parent.GetComponent<RectTransform>().sizeDelta.x / ItemPrefab.GetComponent<RectTransform>().sizeDelta.x;

        HorizScroll.size = Ratio/(  ItemsForSale.Count > Ratio ? ItemsForSale.Count : 1);
        //print(" did " + (ItemsForSale.Length - 2f + (Ratio / 2f)) );

        ItemPrefab.SetActive(false);
    }

    public void MoveScroll()
    {
        float move = -((1 / (HorizScroll.size)) * HorizScroll.transform.parent.GetComponent<RectTransform>().sizeDelta.x * HorizScroll.value);
        move *= (float)(ItemsForSale.Count - Ratio) / (float)ItemsForSale.Count;

        ItemPrefab.transform.parent.GetComponent<RectTransform>().localPosition = new Vector2(move, 0);

    }

    public override void Use(float Amount)
    {
        Stats.StartStopPlayerMovement(Menu.activeSelf);

        Menu.SetActive(!Menu.activeSelf);
        UpdateShop();
    }

    public void RemoveItem(int ItemForSaleID)
    {
        ItemsForSale.Remove(ItemsForSale.Where(a => a.ID == ItemForSaleID).First());
        NameIndic.Indicate("");
        HorizScroll.value = 0;
        UpdateShop();

    }
}

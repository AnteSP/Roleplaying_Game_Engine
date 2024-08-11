using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemList : Resource
{

    [SerializeField] GameObject Menu;

    //[SerializeField] int[] ItemsForSale;
    public List<int> ItemsForSale = new List<int>();

    [SerializeField] GameObject ItemPrefab;

    [SerializeField] Scrollbar HorizScroll;

    float Ratio;

    [SerializeField] GameObject EmptyNotif;
    [SerializeField] Button ForItems;

    [SerializeField] Transform Content;

    public void UpdateShop()
    {
        ForItems.interactable = false;
        EmptyNotif.SetActive(true);

        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(0).gameObject);
        }

        for (int i = 0; i < ItemsForSale.Count; i++)
        {
            Item item = Items.ITEMS_DB[ItemsForSale[i]];
            GameObject temp = Instantiate(ItemPrefab, Content);
            temp.SetActive(true);
            temp.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            temp.transform.GetChild(0).GetComponent<Tooltip>().tooltip = item.Name;
            ForItems.interactable = true;
            EmptyNotif.SetActive(false);
        }

        //Amount of cells that can fit into 1 frame (should be 6 if noone changed anything)
        Ratio = HorizScroll.transform.parent.GetComponent<RectTransform>().sizeDelta.x / ItemPrefab.GetComponent<RectTransform>().sizeDelta.x;

        HorizScroll.size = Ratio / (ItemsForSale.Count > Ratio ? ItemsForSale.Count : 1);
        //print(" did " + (ItemsForSale.Length - 2f + (Ratio / 2f)) );

        ItemPrefab.SetActive(false);
    }

    public void MoveScroll()
    {
        float move = -((1 / (HorizScroll.size)) * HorizScroll.transform.parent.GetComponent<RectTransform>().sizeDelta.x * HorizScroll.value);
        move *= (float)(ItemsForSale.Count - Ratio) / (float)ItemsForSale.Count;

        ItemPrefab.transform.parent.GetComponent<RectTransform>().localPosition = new Vector2(move, 0);

    }

    public void AddToList(int ItemID)
    {
        ItemsForSale.Add(ItemID);
        UpdateShop();
    }

    public override void Use(float Amount)
    {

        Menu.SetActive(!Menu.activeSelf);
        UpdateShop();
    }
}

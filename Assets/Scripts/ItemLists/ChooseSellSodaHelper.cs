using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseSellSodaHelper : MonoBehaviour
{

    public int ID;
    public ItemList ListToAddTo;

    public void GiveID()
    {
        bool good = true;
        for(int i = 0; ListToAddTo.ItemsForSale.Count > i; i++)
        {

            good = (ListToAddTo.ItemsForSale[i] == ID ? false : good);

        }

        if (good)
        {
            ListToAddTo.AddToList(ID);
        }
        
        transform.parent.parent.parent.parent.parent.gameObject.SetActive(false);
    }
}
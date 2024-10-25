using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public int Max;
    public float Current;
    public int ItemID,Quantity;

    public float CollectTime;
    [Range(1,100)]public int FUNMin=0,FUNMax=100;
    public bool unfocusAfterUse = true;

    private void Start()
    {
        //I dont think this works unless this is for a pure Resource object
        if (FUNMax != FUNMin && !Progress.checkFUN(FUNMin, FUNMax)) Destroy(gameObject);
    }

    public virtual void Use(float Amount)
    {

        Current = Current - Amount < 0 ? 0 : Current - Amount;

        if (TryGetComponent<Animator>(out Animator anim))
        {
            anim.SetTrigger("Go");
        }

        if(!Items.Add(ItemID, Quantity))
        {

            Stats.DisplayMessage("Not enough Inventory space",true);

        }
    }

    public void Add(float Amount)
    {
        Current = Current + Amount > Max ? Max : Current + Amount;
    }
}

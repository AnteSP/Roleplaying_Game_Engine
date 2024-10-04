using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandResource : Resource
{

    public int[] ItemPool;

    public override void Use(float Amount)
    {
        if (TryGetComponent<Animator>(out Animator anim))
        {
            anim.SetTrigger("Go");

        }

        if (!Items.Add(ItemPool[Random.Range(0, ItemPool.Length)], Quantity))
        {

            Stats.DisplayMessage("Not enough Inventory space",true);

        }


    }
}

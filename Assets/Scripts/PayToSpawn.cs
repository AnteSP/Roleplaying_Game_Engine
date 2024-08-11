using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayToSpawn : Resource
{
    [SerializeField] GameObject[] Pref;
    [SerializeField] Vector2 Displacement;
    [SerializeField] int[] Its;
    bool Got;
    public int Am { get; set; }
    public override void Use(float Amount)
    {
        Got = false;
        for(int i = 0; i < Pref.Length; i++)
        {
            if (Items.Add(Its[i], -1))
            {
                Instantiate(Pref[i], transform.position + (Vector3)Displacement, new Quaternion(0, 0, 0, 0));
                Got = true;
            }
        }

        if (!Got)
        {
            Stats.DisplayMessage("There's no children in your inventory :(");
        }

    }
}

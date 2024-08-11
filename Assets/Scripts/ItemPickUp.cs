using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : Resource
{

    public int Money;

    string generatePickUpName() => "*" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "/" + gameObject.name;
    public bool savePickUp = true;

    void Start()
    {
        if (Progress.doesPickUpExist(generatePickUpName()))
            Destroy(gameObject);

        if (FUNMax != FUNMin && !Progress.checkFUN(FUNMin, FUNMax)) Destroy(gameObject);
    }

    public override void Use(float Amount)
    {
        if (Items.Add(ItemID, Quantity))
        {
            Stats.ChangeMoney(Money);
            if(Money != 0) Stats.current.KACHING.Play();

            Destroy(gameObject);
            if(savePickUp)Progress.savePickUp(generatePickUpName());
        }
        else
        {
            Stats.DisplayMessage("Not enough room for this item");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRec : MonoBehaviour
{
    public int RecipeIndex;
    static public bool Selecting = false;

    public void doThing()
    {

        
        if (Selecting)
        {
            SodaMachine.ChooseRecipe(RecipeIndex);
            Slider.ForceBack();
        }
        

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SodaMake : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    bool failed = false;
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        failed = false;
        List<int> missingItems = new List<int>();
        for (int i = 0; i < SodaMachine.Ings.Length; i++)
        {
            //Debug.Log("Taking Item " + SodaMachine.Ings[i] + " at " + i);

            if(!Items.Add(SodaMachine.Ings[i].ItemID, -1))
            {
                failed = true;
                missingItems.Add(i);
                //Stats.DisplayMessage("Inventory Error! You don't have enough stuff to do that",true);
            }
        }

        if(!failed)
        {
            if (!Items.Add(Items.RECIPES_DB[SodaMachine.ActiveSoda].Soda.ItemID, 1))
            {

                Stats.DisplayMessage("Inventory Error! you don't have enough space for this!",true);

                //return used up items to player
                for (int i = 0; i < SodaMachine.Ings.Length; i++)
                {
                    Items.Add(SodaMachine.Ings[i].ItemID, 1);
                }
                //SodaMachine.ChooseRecipe(-1);

            }
        }
        else//we failed. Return everything
        {
            for (int i = 0; i < SodaMachine.Ings.Length; i++)
            {
                if (missingItems.Contains(i)) continue;
                Items.AddNoAnim(SodaMachine.Ings[i].ItemID, 1);
            }

            Stats.DisplayMessage("Inventory Error! Missing these item(s):\n\n" + string.Join(',', missingItems.Select(index => SodaMachine.Ings[index].Name)), true);

        }
        
       
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

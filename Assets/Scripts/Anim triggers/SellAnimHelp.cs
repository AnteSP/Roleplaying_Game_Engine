using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellAnimHelp : StateMachineBehaviour
{

    Button SellBut;
    SellSodas S;
    /*
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SellBut = animator.transform.parent.GetComponent<Button>();
        S = SellBut.GetComponent<SellSodas>();
        animator.speed = 1f/(S.SodaInfo.GetTime() * S.List.MLevel);
        SellBut.interactable = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        S.TimeLeft = S.SodaInfo.GetTime() * S.List.MLevel * (1f  - stateInfo.normalizedTime);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Items.Add( S.ID, -1))
        {
            Stats.ChangeMoney( (int)  S.SodaInfo.GetPrice() );
            S.List.UpdateNumbers();
            S.List.CashSound.Play();
        }
        else
        {
            SellBut.gameObject.SetActive(false);
        }

        SellBut.interactable = true;
    }
    */

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

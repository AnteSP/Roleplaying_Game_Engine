using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setCSDiaGoodToGo : StateMachineBehaviour
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
    //public bool KeepAnimAlive = false;
    public RuntimeAnimatorController next;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if(!KeepAnimAlive) animator.name += "[DONE_ANIM]";
        animator.enabled = false;

        if(next != null)
        {
            animator.runtimeAnimatorController = next;
        }
        else
            animator.name += "[DONE_ANIM]";

        if (Dialogue.d != null && Dialogue.d.CS != null)
        {
            Dialogue.d.CS.Next();
            Dialogue.d.CS.goodtoGo = true;
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

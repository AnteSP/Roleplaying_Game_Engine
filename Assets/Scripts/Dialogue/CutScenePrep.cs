using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScenePrep : StateMachineBehaviour
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


    //}
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool Ending;
        CutSceneTalker cs = Stats.current.CurrentCS;
        if (cs.EndingChapter)
        {
            Stats.Debug("OPT 1");
            Ending = true;
            cs.enabled = false;
            animator.transform.parent.Find("End screen").gameObject.SetActive(true);
        }
        else if (cs.Ending)
        {
            Stats.Debug("OPT 4");
            Ending = true;
            if (cs.GoToNextScene())
            {
                MonoBehaviour.print("Switching to scene ");

                return;
                //Stats.current.AllowSelecting = false;
                //Stats.current.FilterColor(new Color(0, 0, 0, 1));
            }
            else
            {
                MonoBehaviour.print("No scene switching");
                Stats.doSelecting(true);
            }
            cs.PackUp(true);
            Stats.StartStopTime(true, "Cutscene");
            //Stats.current.PassTime = true;

            Stats.StartStopPlayerMovement(true, "Transition");
            //Stats.current.Player.GetComponent<Movement>().enabled = true;
        }
        else//CS Starting
        {
            Ending = false;
            cs.gameObject.SetActive(true);
            cs.enabled = true;

            Dialogue.d.showDisplay(true);
            if (cs.musicsQueue.Count > 0)
            {
                AudioSource a = cs.musicsQueue[0];
                cs.musicsQueue.RemoveAt(0);
                Stats.changeBackgroundMusic(a.clip);
            }

            Dialogue.forceGoodToGo(true);
        }

        if(Ending) Stats.current.CurrentCS = null;

        Dialogue.d.forceStopSounds();
        //Progress.DEBUG_printData();
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

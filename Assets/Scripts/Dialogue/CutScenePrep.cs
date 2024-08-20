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
        if (Stats.current.CurrentCS.EndingChapter)
        {
            Stats.Debug("OPT 1");
            Ending = true;
            Stats.current.CurrentCS.enabled = false;
            animator.transform.parent.Find("End screen").gameObject.SetActive(true);
        }
        else if (!Stats.current.CurrentCS.gameObject.activeInHierarchy)
        {
            Stats.Debug("OPT 2");
            Ending = false;
            Stats.current.CurrentCS.gameObject.SetActive(true);
            Stats.current.CurrentCS.enabled = true;
        }else if (Stats.current.CurrentCS.gameObject.activeInHierarchy && !Stats.current.CurrentCS.enabled)
        {
            Stats.Debug("OPT 3");
            Ending = false;
            Stats.current.CurrentCS.enabled = true;
        }
        else
        {
            if (Stats.current.CurrentCS.Ending)//finish cutscene. Back to normal gameplay
            {
                Stats.Debug("OPT 4");
                Ending = true;
                if (Stats.current.CurrentCS.GoToNextScene())
                {
                    MonoBehaviour.print("Switching to scene ");
                    System.Console.WriteLine("CS");
                    Stats.current.AllowSelecting = false;
                    Stats.current.FilterColor(new Color(0,0,0,1));
                }
                else
                {
                    MonoBehaviour.print("No scene switching");
                    Stats.doSelecting(true);
                }
                Stats.current.CurrentCS.PackUp(true);
                Stats.StartStopTime(true, "Cutscene");
                //Stats.current.PassTime = true;
                
                Stats.current.Player.GetComponent<Movement>().enabled = true;
            }
            else//we are doing just a simple transition
            {
                Stats.Debug("OPT 5");
                Ending = false;
                Stats.current.CurrentCS.NextCamPos(0);
                Stats.current.FilterColor(Color.black);
                //play next music track
                AudioSource a = Stats.current.CurrentCS.musicsQueue[0];
                Stats.current.CurrentCS.musicsQueue.RemoveAt(0);
                Stats.changeBackgroundMusic(a.clip);
                Dialogue.forceGoodToGo(true);
            }

        }

        if(Ending) Stats.current.CurrentCS = null;

        Dialogue.d.forceStopSounds();
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

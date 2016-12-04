﻿using UnityEngine;

public class ScreenTransitionBehaviour : StateMachineBehaviour
{
    [SerializeField]
    public bool transitionIn;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1)
        {
            string message = transitionIn? "TransitionedIn" : "TransitionedOut";
            animator.SendMessage(message);
        }
    }
}

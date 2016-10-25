using System;
using UnityEngine;

public enum GUITransitionProgress
{
    Waiting,
    InProgress,
    Done,
}

public abstract class GUITransition : CustomYieldInstruction
{
    public abstract GUITransitionProgress Progress { get; }

    public override bool keepWaiting
    {
        get
        {
            return Progress != GUITransitionProgress.Done;
        }
    }
}

#pragma warning disable 0649

using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
public class GUIElement : MonoBehaviour
{
    private static readonly int ActiveParamName = Animator.StringToHash("Active");

    public Animator Animator { get; private set; }

    public event Action OnTransitionedIn;
    public event Action OnTransitionedOut;

    public bool Activated
    {
        get { return GetComponent<Animator>().GetBool(ActiveParamName); }
        set { GetComponent<Animator>().SetBool(ActiveParamName, value); }
    }

    private void OnEnable()
    {
        Activated = true;
    }

    private void TransitionedOut()
    {
        if (!Activated && OnTransitionedOut != null)
        {
            OnTransitionedOut.Invoke();
        }
    }

    private void TransitionedIn()
    {
        if (Activated && OnTransitionedIn != null)
        {
            OnTransitionedIn.Invoke();
        }
    }

    public void Activate(bool active)
    {
        Activated = active;
    }

    public void Dismiss()
    {
        Activated = false;
    }
}

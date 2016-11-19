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
        get { return Animator.GetBool(ActiveParamName); }
        set { Animator.SetBool(ActiveParamName, value); }
    }
    
    private void OnEnable()
    {
        Animator = GetComponent<Animator>();         
        Activated = true;
    }
    
    private void TransitionedOut()
    {
        if (!Activated)
        {
            OnTransitionedOut.Invoke();
        }
    }

    private void TransitionedIn()
    {
        if (Activated)
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

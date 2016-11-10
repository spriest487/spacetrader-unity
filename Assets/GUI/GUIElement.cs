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
    
    protected bool ActiveParam
    {
        get { return Animator.GetBool(ActiveParamName); }
        set { Animator.SetBool(ActiveParamName, value); }
    }
    
    private void OnEnable()
    {
        Animator = GetComponent<Animator>();         
        ActiveParam = true;
    }
    
    private void TransitionedOut()
    {
        if (!ActiveParam)
        {
            OnTransitionedOut.Invoke();
        }
    }

    private void TransitionedIn()
    {
        if (ActiveParam)
        {
            OnTransitionedIn.Invoke();
        }
    }
    
    public void SetActive(bool active)
    {
        ActiveParam = active;
    }

    public void Dismiss()
    {
        ActiveParam = false;
    }
}

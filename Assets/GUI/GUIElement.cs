#pragma warning disable 0649

using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
public class GUIElement : MonoBehaviour
{
    private static readonly int ActiveParamName = Animator.StringToHash("Active");

    public Animator Animator { get; private set; }

    public event Action OnTransitioningIn;
    public event Action OnTransitionedIn;

    public event Action OnTransitioningOut;
    public event Action OnTransitionedOut;

    public bool Activated
    {
        get
        {
            if (!gameObject.activeSelf)
            {
                return false;
            }
            return GetComponent<Animator>().GetBool(ActiveParamName);
        }
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

    private void TransitioningIn()
    {
        if (Activated && OnTransitioningIn != null)
        {
            OnTransitioningIn.Invoke();
        }
    }

    private void TransitioningOut()
    {
        if (Activated && OnTransitioningOut != null)
        {
            OnTransitioningOut.Invoke();
        }
    }

    public void Activate(bool active)
    {
        if (active && !gameObject.activeSelf)
        {
            //will set activated already in OnEnable()
            gameObject.SetActive(true);
        }
        else if (gameObject.activeSelf)
        {
            Activated = active;
        }
    }

    public void Dismiss()
    {
        Activate(false);
    }
}

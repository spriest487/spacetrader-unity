#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GUIScreen : MonoBehaviour
{
    private static int ActiveParamName = Animator.StringToHash("Active");

    [SerializeField]
    private ScreenID id;

    private Animator animator;

    public ScreenID ID { get { return id; } }

    private bool ActiveParam
    {
        get { return animator.GetBool(ActiveParamName); }
        set { animator.SetBool(ActiveParamName, value); }
    }
    
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.Rebind();
        ActiveParam = true;
    }

    public void Dismiss()
    {
        ActiveParam = false;
    }

    private void OnTransitionedOut()
    {
        if (!ActiveParam)
        {
            SendMessageUpwards("OnScreenTransitionedOut", this);
        }
    }

    private void OnTransitionedIn()
    {
        if (ActiveParam)
        {
            SendMessageUpwards("OnScreenTransitionedIn", this);
        }
    }
}

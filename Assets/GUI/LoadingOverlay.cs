using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    private Animator animator;

    private static readonly int LoadingParamName = Animator.StringToHash("Loading");
    private bool LoadingParam
    {
        get { return animator.GetBool(LoadingParamName); }
        set { animator.SetBool(LoadingParamName, value); }
    }

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.Rebind();
        LoadingParam = true;
    }

    private void TransitionedIn()
    {
        if (LoadingParam)
        {
            SendMessageUpwards("OnLoadingTransitionedIn");
        }
    }

    private void TransitionedOut()
    {
        if (!LoadingParam)
        {
            SendMessageUpwards("OnLoadingTransitionedOut");
        }
    }

    public void Dismiss()
    {
        LoadingParam = false;
    }
}
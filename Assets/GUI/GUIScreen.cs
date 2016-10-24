#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GUIScreen : MonoBehaviour
{
    private static int ActiveParam = Animator.StringToHash("Active");

    [SerializeField]
    private ScreenID id;

    private Animator animator;

    public ScreenID ID { get { return id; } }
    
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(ActiveParam, true);
    }

    public void Dismiss()
    {
        animator.SetBool(ActiveParam, false);
    }

    private void OnTransitionedOut()
    {
        bool screenActive = animator.GetBool(ActiveParam);
        if (!screenActive)
        {
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using System;

[Serializable]
public class ScreenMapping
{
    [SerializeField]
    private ScreenID screenId;

    [SerializeField]
    private GameObject root;

    [HideInInspector]
    [SerializeField]
    private GameObject overlayInstance;

    [SerializeField]
    [HideInInspector]
    private CanvasGroup canvasGroup;

    [Header("Transition")]
    [SerializeField]
    private ScreenTransition transitionIn = ScreenTransition.FadeOutAlpha;

    [SerializeField]
    private ScreenTransition transitionOut = ScreenTransition.FadeInAlpha;

    [SerializeField]
    private string hotkeyButton;

    public GameObject Root { get { return overlayInstance; } }
    public CanvasGroup CanvasGroup { get { return canvasGroup; } }

    public ScreenID ScreenID { get { return screenId; } }
    
    public ScreenTransition TransitionIn { get { return transitionIn; } }
    public ScreenTransition TransitionOut { get { return transitionOut; } }
    public string HotkeyButton { get { return hotkeyButton; } }

    public void Init()
    {
        if (!overlayInstance)
        {
            overlayInstance = UnityEngine.Object.Instantiate(root);
            overlayInstance.gameObject.SetActive(false);

            canvasGroup = overlayInstance.GetComponent<CanvasGroup>();
            if (!canvasGroup)
            {
                canvasGroup = overlayInstance.AddComponent<CanvasGroup>();
            }

            UnityEngine.Object.DontDestroyOnLoad(overlayInstance.gameObject);
        }
    }
}

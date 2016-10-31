#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(GUIElement))]
public class GUIScreen : MonoBehaviour
{
    [SerializeField]
    private ScreenID id;

    [SerializeField]
    private bool showStatusBar;

    [SerializeField]
    private bool showHeader;

    [SerializeField]
    private string headerText;
    
    public GUIElement Element { get; private set; }

    public ScreenID ID { get { return id; } }

    public bool ShowStatusBar
    {
        get { return showStatusBar; }
        set { showStatusBar = value; }
    }

    public bool ShowHeader
    {
        get { return showHeader; }
        set { showHeader = value; }
    }

    public string HeaderText
    {
        get { return headerText; }
        set { headerText = value; }
    }
        
    private void OnEnable()
    {
        Element = GetComponent<GUIElement>();
        
        Element.OnTransitionedIn += TransitionedInHandler;
        Element.OnTransitionedOut += TransitionedOutHandler;
    }

    private void OnDisable()
    {
        Element.OnTransitionedIn -= TransitionedInHandler;
        Element.OnTransitionedOut -= TransitionedOutHandler;
    }

    private void TransitionedInHandler()
    {
        SendMessageUpwards("OnScreenTransitionedIn", this);
    }

    private void TransitionedOutHandler()
    {
        SendMessageUpwards("OnScreenTransitionedOut", this);
    }
}

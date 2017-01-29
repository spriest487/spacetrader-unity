#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIElement))]
public class HeaderBar : MonoBehaviour
{
    [SerializeField]
    private Button backButton;

    [SerializeField]
    private Text headerLabel;

    private GUIController gui;

    public GUIElement Element { get; private set; }
    
    private void Awake()
    {
        gui = GetComponentInParent<GUIController>();
        Element = GetComponent<GUIElement>();

        foreach (var screen in gui.GetComponentsInChildren<GUIScreen>(true))
        {
            screen.OnNavigationChanged += RefreshNavigation;
        }
    }

    private void Update()
    {
        if (gui.HasTransition)
        {
            headerLabel.text = null;
            backButton.gameObject.SetActive(false);
            return;
        }

        var activeScreen = gui.ActiveScreen;

        var headerText = activeScreen.HeaderText;
        if (string.IsNullOrEmpty(headerText))
        {
            headerText = activeScreen.name.ToUpper();
        }

        headerLabel.text = headerText;
        backButton.gameObject.SetActive(activeScreen.IsBackEnabled);
    }

    private void RefreshNavigation()
    {
        var activeScreen = gui.ActiveScreen;

        var backNavigation = backButton.navigation;
        if (activeScreen.TopSelectable || activeScreen.BottomSelectable)
        {
            backNavigation.mode = Navigation.Mode.Explicit;

            backNavigation.selectOnUp = activeScreen.BottomSelectable;
            backNavigation.selectOnDown = activeScreen.TopSelectable;

            if (!backNavigation.selectOnUp)
            {
                backNavigation.selectOnUp = activeScreen.TopSelectable;
            }

            if (!backNavigation.selectOnDown)
            {
                backNavigation.selectOnDown = activeScreen.BottomSelectable;
            }
        }
        else
        {
            backNavigation.mode = Navigation.Mode.None;
        }
        backButton.navigation = backNavigation;
    }

    public void GoBack()
    {
        if (!gui.HasTransition)
        {
            gui.DismissActive();
        }
    }
}

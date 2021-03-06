﻿#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(GUIElement))]
public class GUIScreen : MonoBehaviour
{
    [SerializeField]
    private Selectable topSelectable;

    [SerializeField]
    private Selectable bottomSelectable;

    [SerializeField]
    private ScreenID id;

    [SerializeField]
    private bool showStatusBar;

    [SerializeField]
    private bool showHeader = true;

    [SerializeField]
    private string headerText;

    [SerializeField]
    private string shortcutButton;

    [SerializeField]
    private bool isBackEnabled = true;

    public CanvasGroup CanvasGroup { get; private set; }

    public GUIElement Element { get; private set; }

    public ScreenID ID { get { return id; } }

    public string ShortcutButton { get { return shortcutButton; } }

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

    public bool IsBackEnabled
    {
        get { return isBackEnabled; }
        set { isBackEnabled = value; }
    }

    public Selectable TopSelectable
    {
        get { return topSelectable; }
    }

    public Selectable BottomSelectable
    {
        get { return bottomSelectable; }
    }

    public event Action OnNavigationChanged;

    private void OnEnable()
    {
        Element = GetComponent<GUIElement>();
        CanvasGroup = GetComponent<CanvasGroup>();

        Element.OnTransitionedIn += TransitionedInHandler;
        Element.OnTransitionedOut += TransitionedOutHandler;
        
        Element.OnTransitioningIn += TransitioningInHandler;
        Element.OnTransitioningOut += TransitioningOutHandler;
        AutoFocus();
    }

    private void AutoFocus()
    {
        var autoFocus = topSelectable;
        if (!autoFocus)
        {
            autoFocus = bottomSelectable;
        }

        var focusedObj = autoFocus? autoFocus.gameObject : null;
        EventSystem.current.SetSelectedGameObject(focusedObj);
    }

    private void OnDisable()
    {
        Element.OnTransitionedIn -= TransitionedInHandler;
        Element.OnTransitionedOut -= TransitionedOutHandler;

        Element.OnTransitioningIn -= TransitioningInHandler;
        Element.OnTransitioningOut -= TransitioningOutHandler;
    }

    private void TransitioningOutHandler()
    {
        if (CanvasGroup)
        {
            CanvasGroup.interactable = false;
        }
    }

    private void TransitioningInHandler()
    {
        if (CanvasGroup)
        {
            CanvasGroup.interactable = false;
        }
    }

    private void TransitionedInHandler()
    {
        SendMessageUpwards("OnScreenTransitionedIn", this);

        if (CanvasGroup)
        {
            CanvasGroup.interactable = true;
        }
    }

    private void TransitionedOutHandler()
    {
        SendMessageUpwards("OnScreenTransitionedOut", this);
    }

    public void SetNavigation(Selectable topSelectable, Selectable bottomSelectable)
    {
        this.topSelectable = topSelectable;
        this.bottomSelectable = bottomSelectable;

        AutoFocus();

        if (OnNavigationChanged != null)
        {
            OnNavigationChanged.Invoke();
        }
    }
}

using UnityEngine;

public class HeaderBar : MonoBehaviour
{
    private GUIController gui;

    private void Awake()
    {
        gui = GetComponentInParent<GUIController>();
    }

    public void BackButton()
    {
        if (!gui.HasTransition)
        {
            gui.DismissActive();
        }
    }
}
